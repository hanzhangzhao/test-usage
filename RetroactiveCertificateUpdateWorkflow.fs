module CertificateUsage.RetroactiveCertificateUpdateWorkflow

open System

open FsToolkit.ErrorHandling

open CertificateUsage.Errors
open CertificateUsage.Domain
open CertificateUsage.Dao

let isInThePastFrom (origin : DateTime) (certificateUsage : CertificateUsage) = certificateUsage.Effective < origin

let getRetroactiveUpdatesForCertificate
    (created : DateTime)
    (updateType : Domain.RetroactiveCertificateUpdateType)
    (certificate : CertificateUsage)
    : Validation<RetroactiveCertificateUpdate list option, Errors> =

    if isInThePastFrom created certificate then
        certificate.PlanSelections
        |> List.map (fun ps ->
            { RetroactiveCertificateUpdate.Type = updateType
              CertificateNumber = CertificateNumber.create certificate.CertificateNumber
              CarrierName = CarrierName.create certificate.Carrier
              ClientName = ClientName.create certificate.ClientName
              PolicyNumber = PolicyNumber.create certificate.PolicyNumber
              ProductLine = ProductLine.create ps.ProductLine
              Coverage = Option.map Coverage.create ps.Coverage
              Option = BenefitOption.create ps.Option
              UpdateDate = created
              Backdate = certificate.Effective })
        |> Some
        |> Ok
    else
        Ok None

let private findSelectionInCertificate (certificate : CertificateDao) (usage : CertificateUsageDao) =
    certificate.PlanSelections
    |> List.tryFind (fun ps ->
        ps.ProductLine = usage.ProductLine
        && ps.Coverage = usage.Coverage
        && ps.Option = usage.Option)

let private findSelectionInTransition (certificate : CertificateDao) (dao : RetroactiveCertificateUsageTransitionDao) =
    certificate.PlanSelections
    |> List.tryFind (fun ps ->
        ps.ProductLine = dao.ProductLine
        && ps.Coverage = dao.Coverage
        && ps.Option = dao.Option)

let private findSelectionInUpdate (certificate : CertificateDao) (dao : RetroactiveCertificateUsageUpdateDao) =
    certificate.PlanSelections
    |> List.tryFind (fun ps ->
        ps.ProductLine = dao.ProductLine
        && ps.Coverage = dao.Coverage
        && ps.Option = dao.Option)

let private formatCorrection reversal correction =
    { Correction.Reversal = reversal
      Correction = correction }

let private getUsageReversal id billingEndDate (usage : CertificateUsageDao) : Usage =
    { Id = id
      UsageType = Domain.CertificateUsageType.Reversal
      CertificateNumber = CertificateNumber.create usage.CertificateNumber
      CarrierName = CarrierName.create usage.CarrierName
      ClientName = ClientName.create usage.ClientName
      PolicyNumber = PolicyNumber.create usage.PolicyNumber
      ScbPolicyNumber = PolicyNumber.create usage.ScbPolicyNumber
      BenefitStartDate = usage.BenefitStartDate
      BenefitEndDate = usage.BenefitEndDate
      Division = Division.create usage.Division
      ProductLine = ProductLine.create usage.ProductLine
      ProductLineGroup = ProductLineGroup.create usage.ProductLineGroup
      Coverage = Option.map Coverage.create usage.Coverage
      Option = BenefitOption.create usage.Option
      RatePer = RatePer.create usage.RatePer
      VolumeAmount = VolumeAmount.create usage.VolumeAmount
      VolumeUnit = VolumeUnit.create usage.VolumeUnit
      CarrierRate = Rate.create usage.CarrierRate
      TaxRate = Rate.create usage.TaxRate
      TaxProvince = TaxProvince.create usage.TaxProvince
      BillingEndDate = billingEndDate
      DateIncurred = usage.DateIncurred }

let private formatUsageCorrection
    id
    dateIncurred
    billingEndDate
    (certificate : CertificateDao)
    (selection : PlanSelectionDao)
    =
    { Id = id
      UsageType = Domain.CertificateUsageType.Correction
      CertificateNumber = CertificateNumber.create certificate.CertificateNumber
      CarrierName = CarrierName.create certificate.CarrierName
      ClientName = ClientName.create certificate.ClientName
      PolicyNumber = PolicyNumber.create certificate.PolicyNumber
      ScbPolicyNumber = PolicyNumber.create certificate.ScbPolicyNumber
      BenefitStartDate = certificate.StartDate
      BenefitEndDate = certificate.EndDate
      Division = Division.create certificate.Division
      ProductLine = ProductLine.create selection.ProductLine
      ProductLineGroup = ProductLineGroup.create selection.ProductLineGroup
      Coverage = Option.map Coverage.create selection.Coverage
      Option = BenefitOption.create selection.Option
      RatePer = RatePer.create selection.RatePer
      VolumeAmount = VolumeAmount.create selection.Volume.Amount
      VolumeUnit = VolumeUnit.create selection.Volume.Unit
      CarrierRate = Rate.create selection.CarrierRate
      TaxRate = Rate.create selection.TaxRate
      TaxProvince = TaxProvince.create selection.TaxProvince
      BillingEndDate = billingEndDate
      DateIncurred = dateIncurred }

let private getAllCorrectionInDateRange
    generateId
    (startDate : DateTime)
    (endDate : DateTime)
    (billingDate : DateTime)
    (dao : RetroactiveCertificateUsageTransitionDao)
    =

    let totalMonths =
        ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month

    let monthIncrements = [ 0 .. totalMonths ]

    monthIncrements
    |> List.map (fun month ->
        findSelectionInTransition dao.Certificate dao
        |> Option.map (fun planSelection ->
            formatUsageCorrection
                (generateId ())
                (startDate.AddMonths(month))
                billingDate
                dao.Certificate
                planSelection
            |> (fun x -> [ x ]))
        |> Option.defaultValue [])
    |> List.collect id
    |> List.map Some
    |> List.map (formatCorrection None)

module Updates =
    let getCorrectionsFromUsage
        generateId
        (dao : RetroactiveCertificateUsageUpdateDao)
        =

        let usageDao = dao.Usage
        let certificateDao = dao.Certificate
        let reversal = getUsageReversal (generateId ()) usageDao.BillingEndDate usageDao

        let correction =
            findSelectionInUpdate certificateDao dao
            |> Option.map (
                formatUsageCorrection (generateId ()) usageDao.DateIncurred usageDao.BillingEndDate certificateDao
            )

        formatCorrection (Some reversal) correction

    let getReversalsAndCorrection = getCorrectionsFromUsage Guid.NewGuid

module Terminations =
    let getReversalsFromTermination' generateId (dao : RetroactiveCertificateUsageTransitionDao) =
        // a usage needs to be reversed if it comes after a termination billing period (after the backdate)
        let usageNeedsToBeReversed usage =
            dao.Backdate.AddMonths(1) <= usage.DateIncurred

        dao.Usage
        |> Option.map (fun usage ->
            if usageNeedsToBeReversed usage then
                let reversal, correction =
                    (Some(getUsageReversal (generateId ()) dao.BillingEnd usage), None)

                [ formatCorrection reversal correction ]
            else
                [])
        |> Option.defaultValue []

    let getReversalsFromTermination = getReversalsFromTermination' Guid.NewGuid

    let getCorrectionsForTerminationGaps' generateId (billingEndDate : DateTime) (dao : RetroactiveCertificateUsageTransitionDao) =
        let newTerminationDate = dao.Backdate.AddMonths(1)

        dao.Usage
        |> Option.map (fun usage ->
            let dateIncurred = usage.DateIncurred.AddMonths(1)

            let originalTerminationDate =
                DateTime(
                    dateIncurred.Year,
                    dateIncurred.Month,
                    DateTime.DaysInMonth(dateIncurred.Year, dateIncurred.Month)
                )

            let hasGaps = newTerminationDate > originalTerminationDate

            if hasGaps then
                let monthAgo = billingEndDate.AddMonths(-1)
                let endDate = DateTime(monthAgo.Year, monthAgo.Month, DateTime.DaysInMonth(monthAgo.Year, monthAgo.Month));

                getAllCorrectionInDateRange generateId originalTerminationDate newTerminationDate endDate dao
            else
                [])
        |> Option.defaultValue []

    let getCorrectionsInDateRange = getAllCorrectionInDateRange Guid.NewGuid
    let getCorrectionsForTerminationGaps =
        getCorrectionsForTerminationGaps' Guid.NewGuid

module Enrollments =
    // if the usage date comes before the new enrollment date then
    // the usage is no longer valid and needs to be reversed
    let isReversalOnly usageDate newEnrollmentDate = usageDate < newEnrollmentDate

    let getCorrectionsForEnrollments' generateId (dao : RetroactiveCertificateUsageTransitionDao) =
        dao.Usage
        |> Option.map (fun usage ->
            let usageDate = usage.DateIncurred
            let newEnrollmentDate = dao.Backdate

            let reversal, correction =
                if isReversalOnly usageDate newEnrollmentDate then
                    (Some(getUsageReversal (generateId ()) dao.BillingEnd usage), None)
                else
                    let reversal = (getUsageReversal (generateId ()) dao.BillingEnd usage) |> Some

                    let correction =
                        findSelectionInCertificate dao.Certificate usage
                        |> Option.map (
                            formatUsageCorrection (generateId ()) usage.DateIncurred dao.BillingEnd dao.Certificate
                        )

                    (reversal, correction)

            [ formatCorrection reversal correction ])
        |> Option.defaultValue []

    let getCorrectionsForEnrollments = getCorrectionsForEnrollments' Guid.NewGuid
    let getCorrectionsInDateRange = getAllCorrectionInDateRange Guid.NewGuid
