module CertificateUsage.Tests.RetroactiveCertificateUpdateWorkflow_Tests

open System

open FsToolkit.ErrorHandling

open Xunit
open Expecto

open CertificateUsage
open CertificateUsage.Dao
open CertificateUsage.Domain
open CertificateUsage.RetroactiveCertificateUpdateWorkflow
open CertificateUsage.Tests

module GetRetroactiveUpdatesForCertificate =
    [<Fact>]
    let ``generates a retroactive update for a backdated CoveredCertificate change`` () =
        let created = DateTime.Now
        let effective = created.AddMonths(-2)

        let coveredCertificate = Stubs.CoveredCertificate.coveredCertificateStub effective

        let actual =
            getRetroactiveUpdatesForCertificate created RetroactiveCertificateUpdateType.Update coveredCertificate

        let expected =
            [ { RetroactiveCertificateUpdate.Type = RetroactiveCertificateUpdateType.Update
                CertificateNumber = CertificateNumber.create "CertificateNumber"
                CarrierName = CarrierName.create "Carrier"
                ClientName = ClientName.create "ClientName"
                PolicyNumber = PolicyNumber.create "policyNumber"
                ProductLine = ProductLine.create "ProductLine"
                Option = BenefitOption.create "Option"
                Coverage = Some(Coverage.create "Coverage")
                UpdateDate = created
                Backdate = effective } ]
            |> Some
            |> Validation.Ok

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``does not generate a retroactive update for normal (not backdated) CoveredCertificate change`` () =
        let created = DateTime.Now
        let effective = created

        let coveredCertificate = Stubs.CoveredCertificate.coveredCertificateStub effective

        let actual =
            getRetroactiveUpdatesForCertificate created RetroactiveCertificateUpdateType.Update coveredCertificate

        let expected = None |> Validation.Ok

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``generates a retroactive update for a backdated ExcludedCertificate change`` () =
        let created = DateTime.Now
        let effective = created.AddMonths(-2)

        let excludedCertificate =
            Stubs.ExcludedCertificate.excludedCertificateStub effective

        let actual =
            getRetroactiveUpdatesForCertificate created RetroactiveCertificateUpdateType.Update excludedCertificate

        let expected =
            [ { RetroactiveCertificateUpdate.Type = RetroactiveCertificateUpdateType.Update
                CertificateNumber = CertificateNumber.create "CertificateNumber"
                CarrierName = CarrierName.create "Carrier"
                ClientName = ClientName.create "ClientName"
                PolicyNumber = PolicyNumber.create "PolicyNumber"
                ProductLine = ProductLine.create "ProductLine"
                Option = BenefitOption.create "Option"
                Coverage = Some(Coverage.create "Coverage")
                UpdateDate = created
                Backdate = effective } ]
            |> Some
            |> Validation.Ok

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``does not generate a retroactive update for normal (not backdated) ExcludedCertificate change`` () =
        let created = DateTime.Now
        let effective = created

        let excludedCertificate =
            Stubs.ExcludedCertificate.excludedCertificateStub effective

        let actual =
            getRetroactiveUpdatesForCertificate created RetroactiveCertificateUpdateType.Update excludedCertificate

        let expected = None |> Validation.Ok

        Expect.equal actual expected "should equal"

module Updates =
    module GetCorrectionsFromUsage =
        [<Fact>]
        let ``return correction`` () =
            let usageDao = Stubs.CertificateUsageDao.dao
            let certificateDao = Stubs.Certificate.Dao.dao

            let parameters =
                { Usage = usageDao
                  Certificate = certificateDao
                  ProductLine = Stubs.Certificate.productLine
                  Coverage = Some Stubs.Certificate.coverage
                  Option = Stubs.Certificate.option }

            let actual = Updates.getCorrectionsFromUsage (fun () -> Guid.Empty) parameters

            let expected =
                { Correction.Correction =
                    { Usage.Id = Guid.Empty
                      UsageType = CertificateUsageType.Correction
                      CertificateNumber = CertificateNumber.create Stubs.Certificate.certificateNumber
                      CarrierName = CarrierName.create Stubs.Certificate.carrierName
                      ClientName = ClientName.create Stubs.Certificate.clientName
                      ScbPolicyNumber = PolicyNumber.create Stubs.Certificate.scbPolicyNumber
                      PolicyNumber = PolicyNumber.create Stubs.Certificate.policyNumber
                      BenefitStartDate = Stubs.Certificate.startDate
                      BenefitEndDate = Stubs.Certificate.endDate
                      Division = Division.create Stubs.Certificate.division
                      ProductLine = ProductLine.create Stubs.Certificate.productLine
                      ProductLineGroup = ProductLineGroup.create Stubs.Certificate.productLineGroup
                      Coverage = Some(Coverage.create Stubs.Certificate.coverage)
                      Option = BenefitOption.create Stubs.Certificate.option
                      RatePer = RatePer.create Stubs.Certificate.ratePer
                      VolumeAmount = VolumeAmount.create Stubs.Certificate.volume
                      VolumeUnit = VolumeUnit.create Stubs.Certificate.volumeUnit
                      CarrierRate = Rate.create Stubs.Certificate.carrierRate
                      TaxRate = Rate.create Stubs.Certificate.taxRate
                      TaxProvince = TaxProvince.create Stubs.Certificate.taxProvince
                      BillingEndDate = Stubs.CertificateUsageDao.billingEndDate
                      DateIncurred = Stubs.CertificateUsageDao.dateIncurred }
                    |> Some
                  Reversal =
                    { Usage.Id = Guid.Empty
                      UsageType = CertificateUsageType.Reversal
                      CertificateNumber = CertificateNumber.create Stubs.CertificateUsageDao.certificateNumber
                      ClientName = ClientName.create Stubs.CertificateUsageDao.clientName
                      CarrierName = CarrierName.create Stubs.CertificateUsageDao.carrierName
                      ScbPolicyNumber = PolicyNumber.create Stubs.CertificateUsageDao.scbPolicyNumber
                      PolicyNumber = PolicyNumber.create Stubs.CertificateUsageDao.policyNumber
                      BenefitStartDate = Stubs.CertificateUsageDao.benefitStartDate
                      BenefitEndDate = Stubs.CertificateUsageDao.benefitEndDate
                      Division = Division.create Stubs.CertificateUsageDao.division
                      ProductLine = ProductLine.create Stubs.CertificateUsageDao.productLine
                      ProductLineGroup = ProductLineGroup.create Stubs.CertificateUsageDao.productLineGroup
                      Coverage = Some(Coverage.create Stubs.CertificateUsageDao.coverage)
                      Option = BenefitOption.create Stubs.CertificateUsageDao.option
                      RatePer = RatePer.create Stubs.CertificateUsageDao.ratePer
                      VolumeAmount = VolumeAmount.create Stubs.CertificateUsageDao.volumeAmount
                      VolumeUnit = VolumeUnit.create Stubs.CertificateUsageDao.volumeUnit
                      CarrierRate = Rate.create Stubs.CertificateUsageDao.carrierRate
                      TaxRate = Rate.create Stubs.CertificateUsageDao.taxRate
                      TaxProvince = TaxProvince.create Stubs.CertificateUsageDao.taxProvince
                      BillingEndDate = Stubs.CertificateUsageDao.billingEndDate
                      DateIncurred = Stubs.CertificateUsageDao.dateIncurred }
                    |> Some }

            Expect.equal actual expected "should equal"

module Enrollments =
    open Stubs.RetroactiveCertificateUsageTransitionDao

    module GetCorrectionsForEnrollments =
        [<Fact>]
        let ``gets reversal only when usage date < new enrollment date`` () =
            let generateId () = Guid.Empty

            let dao' = Dao.stub Guid.Empty Guid.Empty Guid.Empty

            let dao =
                { dao' with
                    Backdate = DateTime(2023, 7, 21)
                    Usage =
                        Some
                            { dao'.Usage.Value with
                                DateIncurred = DateTime(2023, 5, 31) } }

            let actual = Enrollments.getCorrectionsForEnrollments' generateId dao

            let expected =
                [ { (Domain.stub Guid.Empty) with
                      Correction = None } ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``gets reversal and correction`` () =
            let generateId () = Guid.Empty

            let dao' = Dao.stub Guid.Empty Guid.Empty Guid.Empty

            let dao =
                { dao' with
                    Backdate = DateTime(2023, 7, 21)
                    Usage =
                        Some
                            { dao'.Usage.Value with
                                DateIncurred = DateTime(2023, 7, 31) } }

            let actual = Enrollments.getCorrectionsForEnrollments' generateId dao
            let domain = Domain.stub Guid.Empty

            let expected =
                [ { domain with
                      Reversal =
                          Some(
                              { domain.Reversal.Value with
                                  DateIncurred = DateTime(2023, 7, 31) }
                          )
                      Correction =
                          Some(
                              { domain.Correction.Value with
                                  DateIncurred = DateTime(2023, 7, 31) }
                          ) } ]

            Expect.equal actual expected "should equal"

module Terminations =
    open Stubs.RetroactiveCertificateUsageTransitionDao

    module getReversalsFromTermination =
        [<Fact>]
        let ``gets reversal when terminating in the past for the first time`` () =
            let generateId () = Guid.Empty

            let dao' = Dao.stub Guid.Empty Guid.Empty Guid.Empty

            let dao =
                { dao' with
                    Usage =
                        Some(
                            { dao'.Usage.Value with
                                DateIncurred = DateTime(2023, 8, 31)
                                BillingEndDate = DateTime(2023, 8, 31) }
                        )
                    Backdate = DateTime(2023, 7, 21)
                    BillingStart = DateTime(2023, 9, 01)
                    BillingEnd = DateTime(2023, 9, 30) }

            let actual = Terminations.getReversalsFromTermination' generateId dao
            let domain = Domain.stub Guid.Empty

            let expected =
                [ { domain with
                      Correction = None
                      Reversal =
                          Some(
                              { domain.Reversal.Value with
                                  DateIncurred = DateTime(2023, 8, 31)
                                  BillingEndDate = DateTime(2023, 9, 30) }
                          ) } ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``gets nothing when a usage comes before a retro termination`` () =
            let generateId () = Guid.Empty

            let dao' = Dao.stub Guid.Empty Guid.Empty Guid.Empty

            let dao =
                { dao' with
                    Usage =
                        Some(
                            { dao'.Usage.Value with
                                DateIncurred = DateTime(2023, 6, 30)
                                BillingEndDate = DateTime(2023, 6, 30) }
                        )
                    Backdate = DateTime(2023, 7, 1)
                    BillingStart = DateTime(2023, 9, 01)
                    BillingEnd = DateTime(2023, 9, 30) }

            let actual = Terminations.getReversalsFromTermination' generateId dao
            let expected = []

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``gets nothing when there is no usage`` () =
            let generateId () = Guid.Empty

            let dao' = Dao.stub Guid.Empty Guid.Empty Guid.Empty

            let dao =
                { dao' with
                    Usage = None
                    Backdate = DateTime(2023, 7, 1)
                    BillingStart = DateTime(2023, 9, 01)
                    BillingEnd = DateTime(2023, 9, 30) }

            let actual = Terminations.getReversalsFromTermination' generateId dao
            let expected = []

            Expect.equal actual expected "should equal"


// module getCorrectionsForTerminationGaps =
//     [<Fact>]
//     let ``correction for all periods where newTerminationDate > originalTerminationDate`` () =
//         let generateId () = Guid.Empty
//
//         let dao' = Dao.stub Guid.Empty Guid.Empty Guid.Empty
//
//         let dao =
//             { dao' with
//                 Usage =
//                     Some(
//                         { dao'.Usage.Value with
//                             DateIncurred = DateTime(2023, 6, 30)
//                             BillingEndDate = DateTime(2023, 6, 30) }
//                     )
//                 Backdate = DateTime(2023, 8, 15)
//                 BillingStart = DateTime(2023, 9, 01)
//                 BillingEnd = DateTime(2023, 9, 30) }
//
//         let actual =
//             Terminations.getCorrectionsForTerminationGaps' generateId dao.BillingEnd dao
//
//         let domain = Domain.stub Guid.Empty
//
//         let expected =
//             [ { domain with
//                   Reversal = None
//                   Correction =
//                       Some(
//                           { domain.Correction.Value with
//                               DateIncurred = DateTime(2023, 7, 31)
//                               BillingEndDate = DateTime(2023, 9, 30) }
//                       ) }
//               { domain with
//                   Reversal = None
//                   Correction =
//                       Some(
//                           { domain.Correction.Value with
//                               DateIncurred = DateTime(2023, 8, 31)
//                               BillingEndDate = DateTime(2023, 9, 30) }
//                       ) } ]
//
//         Expect.equal actual expected "should equal"
//
//     [<Fact>]
//     let ``no corrections if no usage`` () =
//         let generateId () = Guid.Empty
//
//         let dao' = Dao.stub Guid.Empty Guid.Empty Guid.Empty
//
//         let dao =
//             { dao' with
//                 Usage = None
//                 Backdate = DateTime(2023, 8, 15)
//                 BillingStart = DateTime(2023, 9, 01)
//                 BillingEnd = DateTime(2023, 9, 30) }
//
//         let actual =
//             Terminations.getCorrectionsForTerminationGaps' generateId dao.BillingEnd dao
//
//         let expected = []
//         Expect.equal actual expected "should equal"
//
//     [<Fact>]
//     let ``no correction for all periods where newTerminationDate <= originalTerminationDate`` () =
//         let generateId () = Guid.Empty
//
//         let dao' = Dao.stub Guid.Empty Guid.Empty Guid.Empty
//
//         let dao =
//             { dao' with
//                 Usage =
//                     Some(
//                         { dao'.Usage.Value with
//                             DateIncurred = DateTime(2023, 6, 30)
//                             BillingEndDate = DateTime(2023, 6, 30) }
//                     )
//                 Backdate = DateTime(2023, 6, 30)
//                 BillingStart = DateTime(2023, 9, 01)
//                 BillingEnd = DateTime(2023, 9, 30) }
//
//         let actual =
//             Terminations.getCorrectionsForTerminationGaps' generateId dao.BillingEnd dao
//
//         let expected = []
//
//         Expect.equal actual expected "should equal"
