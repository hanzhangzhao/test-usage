module CertificateUsage.Api.Service.CloseOutCorrections

open System

open FsToolkit.ErrorHandling

open CertificateUsage
open CertificateUsage.Period
open CertificateUsage.Dao
open CertificateUsage.Api.Dependencies
open CertificateUsage.RetroactiveCertificateUpdateWorkflow
open CertificateUsage.ToDao
open CertificateUsage.Domain

let getLastDayOfMonth (date : DateTime) =
    DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month))

let getLastDayOfPriorMonth (date : DateTime) =
    let monthAgo = date.AddMonths(-1)
    getLastDayOfMonth (monthAgo.AddDays(-1))

let getLastDayOfNextMonth (date : DateTime) =
    let nextMonth = date.AddMonths(1)
    getLastDayOfMonth nextMonth

// use cases for retro terminations:
//     Terminate certificate in the past before a prior termination:
//       certificate was previously terminated on April 2023.
//       Terminating on March 2023 in the August 2023 billing period
//       The certificate is no longer active in April 2023, and needs
//       a reversal for April 2023
//
//    Terminate certificate in the past after a prior termination:
//       certificate was previously terminated on April 2023.
//       Terminating on July 2023 in the August 2023 billing period
//       The certificate is now active till July 2023, and needs
//       corrections from May 2023 to July 2023
//
//     Terminate certificate in the past with no prior termination:
//       certificate was not previously terminated.
//       Terminating on March 2023 in the August 2023 billing period
//       The certificate is no longer active in April 2023, and needs
//       a reversals for April 2023 till the current billing period
module Terminations =
    let mapCorrectionDomainToCorrectionTransactionDao billingDate usageId id =
        List.map (Correction.fromCorrectionDomainModel billingDate usageId id)

    //  there are gaps that need corrections due to a retro termination if it
    //  has been used and the date incurred ("usage date") is less than
    // the retro backdate (termination date)
    module CorrectionGaps =
        let getOriginalTerminationUsages daos =
            let groupRetroUpdatesByRetroUpdateId =
                List.groupBy (fun (dao : RetroactiveCertificateUsageTransitionDao) -> dao.RetroCertificateUpdateId)

            let tryFindFirstRetroDaoWithUpdate =
                List.filter (fun x -> x.Usage.IsSome)
                >> List.sortByDescending (fun x -> x.Usage.Value.DateIncurred)
                >> List.tryHead

            let hasGap retros =
                retros
                |> List.forall (fun retro ->
                    retro.Usage
                    |> Option.map (fun usage -> usage.DateIncurred < retro.Backdate)
                    |> Option.defaultValue false)

            // get a list of gaps for each retro termination
            daos
            |> groupRetroUpdatesByRetroUpdateId
            |> List.map (fun (_, retros) ->
                if hasGap retros then
                    tryFindFirstRetroDaoWithUpdate retros
                    |> Option.map (fun gap -> (gap, getLastDayOfNextMonth gap.Usage.Value.DateIncurred, gap.Backdate))
                else
                    None)
            |> List.choose id

        let getCorrectionsForTerminationGaps billingEnd (daos : RetroactiveCertificateUsageTransitionDao list) =
            daos
            |> getOriginalTerminationUsages
            |> List.map (fun (dao, startDate, endDate) ->
                Terminations.getCorrectionsInDateRange startDate endDate billingEnd dao
                |> mapCorrectionDomainToCorrectionTransactionDao billingEnd None dao.RetroCertificateUpdateId)
            |> List.collect id

    let getUsageId = Option.map (fun (usage : CertificateUsageDao) -> usage.Id)

    let closeOutRetroactiveTerminations (root : Root.Root) (carrier : string) (billingPeriod : Period) =
        let billingEnd = billingPeriod.End

        task {
            let! retroDaos = root.GetRetroTerminations carrier billingPeriod

            let corrections =
                retroDaos
                |> List.map (fun dao ->
                    Terminations.getReversalsFromTermination dao
                    |> mapCorrectionDomainToCorrectionTransactionDao
                        billingEnd
                        (getUsageId dao.Usage)
                        dao.RetroCertificateUpdateId)
                |> List.collect id

            let gapCorrections =
                CorrectionGaps.getCorrectionsForTerminationGaps billingEnd retroDaos

            let result =
                corrections @ gapCorrections |> List.collect id |> root.InsertCorrections

            return result
        }

// use cases for enrollments:
//     ReEnroll Before an Existing Enrollment
//       enrolled on April 2023, terminate benefits and re-enroll
//       on February 2023 in the August 2023 billing period. Needs
//       corrections for Feb and March and reversals/corrections
//       for April to the billing date (august 2023)
//
//     ReEnroll After an Existing Enrollment
//       enrolled on February 2023, terminate benefits and re-enroll
//       on March 2023 for the august billing period. Needs
//       reversal for Feb 2023 and reversals + corrections
//       for March 2023 to the billing date (August 2023)
//
//     Add Brand New Enrollment:
//       enrolled on April 2023 in the August billing period
//       (not previously enrolled). The certificate is active
//       from April 2023, and needs corrections for April 2023
//       to billing date (August 2023)
module Enrollments =

    //  there are gaps that need corrections due to a retro enrollment if
    //  it is not being used (new enrollment in the past) or
    //  the retro backdate (new enrollment billing period) is less than the
    //  date incurred ("usage date")
    module CorrectionGaps =
        let getGapDates (billingEndDate : DateTime) daos =
            let groupRetroUpdatesByRetroUpdateId =
                List.groupBy (fun (dao : RetroactiveCertificateUsageTransitionDao) -> dao.RetroCertificateUpdateId)

            let tryFindFirstRetroUpdateDao =
                List.sortBy (fun dao -> dao.Usage.Value.DateIncurred) >> List.tryHead

            let tryGetNewEnrollmentGap = List.tryFind (fun retro -> retro.Usage.IsNone)

            let isExistingEnrollmentGap =
                List.forall (fun (retro : RetroactiveCertificateUsageTransitionDao) ->
                    retro.Backdate < retro.Usage.Value.DateIncurred)

            let tryFindGap (retros : RetroactiveCertificateUsageTransitionDao list) =
                retros
                |> tryGetNewEnrollmentGap
                |> Option.map (fun dao -> Some(dao, dao.Backdate, getLastDayOfPriorMonth billingEndDate))
                |> Option.defaultWith (fun () ->
                    if (isExistingEnrollmentGap retros) then
                        tryFindFirstRetroUpdateDao retros
                        |> Option.map (fun dao ->
                            (dao, dao.Backdate, getLastDayOfPriorMonth dao.Usage.Value.DateIncurred))
                    else
                        None)

            daos
            |> groupRetroUpdatesByRetroUpdateId
            |> List.map (fun (_, retros) -> tryFindGap retros)

        let getGapCorrectionsForNewRetroEnrollment
            billingPeriodEnd
            (daos : RetroactiveCertificateUsageTransitionDao list)
            =
            let mapCorrectionDomainToCorrectionTransactionDao retroUpdateId =
                List.map (Correction.fromCorrectionDomainModel billingPeriodEnd None retroUpdateId)

            daos
            |> getGapDates billingPeriodEnd
            |> List.map (fun dates ->
                dates
                |> Option.map (fun (dao, startDate, endDate) ->
                    dao
                    |> Enrollments.getCorrectionsInDateRange startDate endDate billingPeriodEnd
                    |> mapCorrectionDomainToCorrectionTransactionDao dao.RetroCertificateUpdateId)
                |> Option.defaultValue [])
            |> List.collect id

    let closeOutRetroactiveEnrollments (root : Root.Root) (carrierName : string) (billingPeriod : Period) =
        let billingEnd = billingPeriod.End

        task {
            let mapCorrectionDomainToCorrectionTransactionDao dao (corrections : Correction list) =
                corrections
                |> List.map (fun correction ->
                    let usageId = dao.Usage |> Option.map (fun usage -> usage.Id)
                    Correction.fromCorrectionDomainModel billingEnd usageId dao.RetroCertificateUpdateId correction)

            let! daos = root.GetRetroEnrollments carrierName billingPeriod

            let corrections =
                daos
                |> List.map (fun dao ->
                    Enrollments.getCorrectionsForEnrollments dao
                    |> mapCorrectionDomainToCorrectionTransactionDao dao)
                |> List.collect id

            let gapCorrections =
                daos |> CorrectionGaps.getGapCorrectionsForNewRetroEnrollment billingEnd

            let result =
                corrections @ gapCorrections |> List.collect id |> root.InsertCorrections

            return result
        }

let closeOutUpdates workflow (root : Root.Root) (carrier : string) (billingPeriod : Period) =
    let billingEnd = billingPeriod.End

    task {
        let! corrections = root.GetRetroUpdates Dao.RetroactiveCertificateUpdateType.Update carrier billingPeriod

        let mapCorrectionDomainToCorrectionTransactionDao (dao : RetroactiveCertificateUsageUpdateDao) =
            Correction.fromCorrectionDomainModel billingEnd (Some dao.Usage.Id) dao.Usage.CausationId

        let reversalsAndCorrections =
            corrections
            |> List.map (fun dao -> workflow dao |> mapCorrectionDomainToCorrectionTransactionDao dao)
            |> List.collect id
            |> root.InsertCorrections

        return reversalsAndCorrections
    }

let closeOutRetroactiveUpdates = closeOutUpdates Updates.getReversalsAndCorrection
