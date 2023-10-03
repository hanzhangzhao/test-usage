module CertificateUsage.Tests.CertificateStateWorkflow_Tests

open System

open Xunit
open Expecto

open CertificateUsage.Errors
open CertificateUsage.Domain
open CertificateUsage.CertificateStateWorkflow
open CertificateUsage.Tests
open CertificateUsage.Tests.ToDao

module CoveredEventToCertificate =
    [<Fact>]
    let ``a covered certificate and dao merges into a certificate`` () =
        let coveredCertificate =
            DateTime(2023, 08, 14)
            |> Stubs.CoveredCertificate.coveredCertificateStub
            |> (Stubs.CoveredCertificate.withPlanSelections [ Stubs.PlanSelection.domain ])

        let dao =
            { Stubs.CertificateRecordDao.dao with
                Certificate =
                    { Stubs.CertificateRecordDao.dao.Certificate with
                        EndDate = DateTime(2024, 8, 14) |> Some } }

        let actual = executeCertificateWorkflow (Some dao) coveredCertificate

        let expected =
            { Certificate.CertificateNumber = CertificateNumber.create "CertificateNumber"
              CarrierName = CarrierName.create "Carrier"
              ClientName = ClientName.create "ClientName"
              ScbPolicyNumber = PolicyNumber.create "Number"
              PolicyNumber = PolicyNumber.create "policyNumber"
              StartDate = DateTime(2023, 08, 14)
              EndDate = Some(DateTime(2024, 08, 14))
              Division = Division.create "Division"
              PlanSelections =
                [ { ProductLine = "ProductLine"
                    ProductLineGroup = "ProductLineGroup"
                    Coverage = Some "Coverage"
                    Option = "Option"
                    RatePer = 1.0m
                    Volume = { Amount = 1.23m; Unit = "CAD" }
                    CarrierRate = 1.23m
                    TaxRate = 2.34m
                    TaxProvince = "TaxProvince" } ]
              CertificateStatus = CertificateStatus.Active }

        Expect.equal actual (Ok expected) "should equal"

    [<Fact>]
    let ``a covered certificate without dao transforms to certificate`` () =
        let coveredCertificate =
            DateTime(2023, 08, 14)
            |> Stubs.CoveredCertificate.coveredCertificateStub
            |> (Stubs.CoveredCertificate.withPlanSelections [ Stubs.PlanSelection.domain ])

        let actual = executeCertificateWorkflow None coveredCertificate

        let expected =
            { Certificate.CertificateNumber = CertificateNumber.create "CertificateNumber"
              CarrierName = CarrierName.create "Carrier"
              ClientName = ClientName.create "ClientName"
              ScbPolicyNumber = PolicyNumber.create "Number"
              PolicyNumber = PolicyNumber.create "policyNumber"
              StartDate = DateTime(2023, 08, 14)
              EndDate = None
              Division = Division.create "Division"
              PlanSelections =
                [ { ProductLine = "ProductLine"
                    ProductLineGroup = "ProductLineGroup"
                    Coverage = Some "Coverage"
                    Option = "Option"
                    RatePer = 1.0m
                    Volume = { Amount = 1.23m; Unit = "CAD" }
                    CarrierRate = 1.23m
                    TaxRate = 2.34m
                    TaxProvince = "TaxProvince" } ]
              CertificateStatus = CertificateStatus.Active }

        Expect.equal actual (Ok expected) "should equal"

    [<Fact>]
    let ``excluded certificate and a dao merges to a certificate`` () =
        let excludedCertificate =
            DateTime(2024, 08, 14)
            |> Stubs.ExcludedCertificate.excludedCertificateStub
            |> (Stubs.CoveredCertificate.withPlanSelections [ Stubs.PlanSelection.domain ])

        let dao =
            { Stubs.CertificateRecordDao.dao with
                Certificate =
                    { Stubs.CertificateRecordDao.dao.Certificate with
                        StartDate = DateTime(2023, 8, 14) } }

        let actual = executeCertificateWorkflow (Some dao) excludedCertificate

        let expected =
            { Certificate.CertificateNumber = CertificateNumber.create "CertificateNumber"
              CarrierName = CarrierName.create "Carrier"
              ClientName = ClientName.create "ClientName"
              ScbPolicyNumber = PolicyNumber.create "ScbPolicyNumber"
              PolicyNumber = PolicyNumber.create "PolicyNumber"
              StartDate = DateTime(2023, 08, 14)
              EndDate = Some(DateTime(2024, 08, 14))
              Division = Division.create "Division"
              PlanSelections =
                [ { ProductLine = "ProductLine"
                    ProductLineGroup = "ProductLineGroup"
                    Coverage = Some "Coverage"
                    Option = "Option"
                    RatePer = 1.0m
                    Volume = { Amount = 1.23m; Unit = "CAD" }
                    CarrierRate = 1.23m
                    TaxRate = 2.34m
                    TaxProvince = "TaxProvince" } ]
              CertificateStatus = CertificateStatus.Terminated }

        Expect.equal actual (Ok expected) "should equal"

    [<Fact>]
    let ``excluded certificate without a dao fails (returns Error) to transform`` () =
        let excludedCertificate =
            DateTime(2024, 08, 14)
            |> Stubs.ExcludedCertificate.excludedCertificateStub
            |> (Stubs.CoveredCertificate.withPlanSelections [ Stubs.PlanSelection.domain ])

        let actual = executeCertificateWorkflow None excludedCertificate
        let expected = Error(Errors.MissingCertificate "CertificateNumber")

        Expect.equal actual expected "should equal"
