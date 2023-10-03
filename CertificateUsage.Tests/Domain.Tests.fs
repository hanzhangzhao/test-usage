module CertificateUsage.Tests.Domain

open System

open Xunit
open Expecto

open CertificateUsage.Domain
open CertificateUsage.Tests.Stubs.CoveredCertificate
open CertificateUsage.Tests.Stubs.ExcludedCertificate

module CertificateUsage =
    module Effective =
        [<Fact>]
        let ``get correct effective date from a covered event`` () =
            let covered = coveredCertificateStub (DateTime(2023, 08, 14))
            let expected = DateTime(2023, 08, 14)
            let actual = covered.Effective
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``get correct effective date from a excluded event`` () =
            let covered = excludedCertificateStub (DateTime(2023, 08, 14))
            let expected = DateTime(2023, 08, 14)
            let actual = covered.Effective
            Expect.equal actual expected "should equal"

    module CertificateNumber =
        [<Fact>]
        let ``get correct CertificateNumber from a covered event`` () =
            let excluded = coveredCertificateStub (DateTime(2023, 08, 14))
            let expected = "CertificateNumber"
            let actual = excluded.CertificateNumber
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``get correct CertificateNumber from a excluded event`` () =
            let excluded = excludedCertificateStub (DateTime(2023, 08, 14))
            let expected = "CertificateNumber"
            let actual = excluded.CertificateNumber
            Expect.equal actual expected "should equal"

    module ScbPolicyNumber =
        [<Fact>]
        let ``get correct SCB policy number from a covered event`` () =
            let covered = coveredCertificateStub (DateTime(2023, 08, 14))
            let expected = "Number"
            let actual = covered.ScbPolicyNumber
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``get correct SCB policy number from a excluded event`` () =
            let excluded = excludedCertificateStub (DateTime(2023, 08, 14))
            let expected = "ScbPolicyNumber"
            let actual = excluded.ScbPolicyNumber
            Expect.equal actual expected "should equal"

    module Carrier =
        [<Fact>]
        let ``get correct carrier name from a covered event`` () =
            let covered = coveredCertificateStub (DateTime(2023, 08, 14))
            let expected = "Carrier"
            let actual = covered.Carrier
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``get correct carrier name from a excluded event`` () =
            let excluded = excludedCertificateStub (DateTime(2023, 08, 14))
            let expected = "Carrier"
            let actual = excluded.Carrier
            Expect.equal actual expected "should equal"

    module PlanSelections =
        [<Fact>]
        let ``get correct plan selections name from a covered event`` () =
            let covered = coveredCertificateStub (DateTime(2023, 08, 14))

            let expected =
                [ { ProductLine = "ProductLine"
                    ProductLineGroup = "LineGroup"
                    Coverage = Some "Coverage"
                    Option = "Option"
                    RatePer = 1.1m
                    Volume = { Amount = 1.2m; Unit = "CAD" }
                    CarrierRate = 0.8m
                    TaxRate = 1.0m
                    TaxProvince = "TaxProvince" } ]

            let actual = covered.PlanSelections
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``get correct plan selections name from a excluded event`` () =
            let excluded = excludedCertificateStub (DateTime(2023, 08, 14))

            let expected =
                [ { ProductLine = "ProductLine"
                    ProductLineGroup = "LineGroup"
                    Coverage = Some "Coverage"
                    Option = "Option"
                    RatePer = 1.1m
                    Volume = { Amount = 1.2m; Unit = "CAD" }
                    CarrierRate = 0.8m
                    TaxRate = 1.3m
                    TaxProvince = "TaxProvince" } ]

            let actual = excluded.PlanSelections
            Expect.equal actual expected "should equal"

    module ClientName =
        [<Fact>]
        let ``get correct client name from a covered event`` () =
            let covered = coveredCertificateStub (DateTime(2023, 08, 14))

            let expected = "ClientName"
            let actual = covered.ClientName
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``get correct carrier name from a excluded event`` () =
            let excluded = excludedCertificateStub (DateTime(2023, 08, 14))

            let expected = "ClientName"
            let actual = excluded.ClientName
            Expect.equal actual expected "should equal"

module BillingPeriod =
    module Intersects =
        [<Fact>]
        let ``a date intersects a billing period`` () =
            let date = DateTime(2023, 06, 15)
            let billingPeriod = { Year = 2023; Month = 6 }
            let actual = billingPeriod.Intersects date
            Expect.isTrue actual ""

        [<Fact>]
        let ``a date doesn't intersect  a month before a billing period`` () =
            let date = DateTime(2023, 05, 15)
            let billingPeriod = { Year = 2023; Month = 6 }
            let actual = billingPeriod.Intersects date
            Expect.isFalse actual ""

        [<Fact>]
        let ``a date doesn't intersect a year before a billing period`` () =
            let date = DateTime(2022, 06, 15)
            let billingPeriod = { Year = 2023; Month = 6 }
            let actual = billingPeriod.Intersects date
            Expect.isFalse actual ""

        [<Fact>]
        let ``a date doesn't intersect a month after a billing period`` () =
            let date = DateTime(2023, 07, 15)
            let billingPeriod = { Year = 2023; Month = 6 }
            let actual = billingPeriod.Intersects date
            Expect.isFalse actual ""

        [<Fact>]
        let ``a date doesn't intersect a year after a billing period`` () =
            let date = DateTime(2024, 06, 15)
            let billingPeriod = { Year = 2023; Month = 6 }
            let actual = billingPeriod.Intersects date
            Expect.isFalse actual ""

    module Before =
        [<Fact>]
        let ``a date is inside a billing period`` () =
            let date = DateTime(2023, 6, 30)
            let billingPeriod = { Year = 2023; Month = 6 }
            let actual = billingPeriod.Before date
            Expect.isFalse actual ""

        [<Fact>]
        let ``a date before a billing period is in billing period`` () =
            let date = DateTime(2023, 4, 30)
            let billingPeriod = { Year = 2023; Month = 6 }
            let actual = billingPeriod.Before date
            Expect.isTrue actual ""

        [<Fact>]
        let ``a date after a billing period is not in the billing period`` () =
            let date = DateTime(2023, 7, 1)
            let billingPeriod = { Year = 2023; Month = 6 }
            let actual = billingPeriod.Before date
            Expect.isFalse actual ""

module Usage =

    [<Fact>]
    let ``When covered is before the billing period include in usage`` () =
        let billingPeriod = { Year = 2023; Month = 6 }
        let cutoffDate = DateTime(2023, 05, 12)
        let input = [ coveredCertificateStub (cutoffDate) ]
        let expected = 1

        let actual = certificateUsageForPeriod billingPeriod input
        Expect.equal actual.Length expected

    [<Fact>]
    let ``When excluded is the same month as the billing period include in usage`` () =
        let billingPeriod = { Year = 2023; Month = 6 }
        let cutoffDate = DateTime(2023, 06, 12)

        let input = [ excludedCertificateStub (cutoffDate) ]
        let expected = 1

        let actual = certificateUsageForPeriod billingPeriod input
        Expect.equal actual.Length expected

    [<Fact>]
    let ``When covered is the same month as the billing period exclude from usage`` () =
        let billingPeriod = { Year = 2023; Month = 6 }
        let cutoffDate = DateTime(2023, 06, 12)

        let input = [ coveredCertificateStub (cutoffDate) ]
        let expected = 0

        let actual = certificateUsageForPeriod billingPeriod input
        Expect.equal actual.Length expected


    [<Fact>]
    let ``When covered & excluded are in the same month as the billing period include in usage`` () =
        let billingPeriod = { Year = 2023; Month = 6 }
        let cutoffDate = DateTime(2023, 06, 12)

        let input =
            [ coveredCertificateStub (cutoffDate); excludedCertificateStub (cutoffDate) ]

        let expected = 1

        let actual = certificateUsageForPeriod billingPeriod input
        Expect.equal actual.Length expected

    [<Fact>]
    let ``When covered is in the next month as the billing period exclude from usage`` () =
        let billingPeriod = { Year = 2023; Month = 6 }
        let cutoffDate = DateTime(2023, 07, 12)

        let input = [ coveredCertificateStub (cutoffDate) ]
        let expected = 0

        let actual = certificateUsageForPeriod billingPeriod input
        Expect.equal actual.Length expected

    [<Fact>]
    let ``When excluded is in the next month as the billing period exclude from usage`` () =
        let billingPeriod = { Year = 2023; Month = 6 }
        let cutoffDate = DateTime(2023, 07, 12)

        let input = [ excludedCertificateStub (cutoffDate) ]
        let expected = 0

        let actual = certificateUsageForPeriod billingPeriod input
        Expect.equal actual.Length expected

module Rate =
    module SyncCertificateUsageChangeDaoCarrierRates =

        [<Fact>]
        let ``updates a rate effective before a certificate's effective date`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let planSectionForRateChange = certificateUsage.PlanSelections[0]

            let rates =
                [ { Stubs.CarrierRateModificationDomainModel.domain with
                      Carrier = certificateUsage.Carrier
                      PolicyNumber = certificateUsage.ScbPolicyNumber
                      Option = planSectionForRateChange.Option
                      Coverage = planSectionForRateChange.Coverage
                      ProductLine = planSectionForRateChange.ProductLine
                      Effective = certificateUsage.Effective.AddDays(-1)
                      CarrierRate = 1.24m } ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]

            let coveredEvent =
                match certificateUsage with
                | CoveredEvent coveredEvent -> coveredEvent
                | _ -> failwith "expected covered event in test"

            let expected =
                [ { coveredEvent with
                      PlanSelections =
                          [ { planSectionForRateChange with
                                CarrierRate = 1.24m } ] }
                  |> CoveredEvent ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``updates a rate effective on a certificate's effective date`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let planSectionForRateChange = certificateUsage.PlanSelections[0]

            let rates =
                [ { Stubs.CarrierRateModificationDomainModel.domain with
                      Carrier = certificateUsage.Carrier
                      PolicyNumber = certificateUsage.ScbPolicyNumber
                      Option = planSectionForRateChange.Option
                      Coverage = planSectionForRateChange.Coverage
                      ProductLine = planSectionForRateChange.ProductLine
                      Effective = certificateUsage.Effective
                      CarrierRate = 1.24m } ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]

            let coveredEvent =
                match certificateUsage with
                | CoveredEvent coveredEvent -> coveredEvent
                | _ -> failwith "expected covered event in test"

            let expected =
                [ { coveredEvent with
                      PlanSelections =
                          [ { planSectionForRateChange with
                                CarrierRate = 1.24m } ] }
                  |> CoveredEvent ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate effective after a certificate's effective date`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let planSectionForRateChange = certificateUsage.PlanSelections[0]

            let rates =
                [ { Stubs.CarrierRateModificationDomainModel.domain with
                      Carrier = certificateUsage.Carrier
                      PolicyNumber = certificateUsage.ScbPolicyNumber
                      Option = planSectionForRateChange.Option
                      Coverage = planSectionForRateChange.Coverage
                      ProductLine = planSectionForRateChange.ProductLine
                      Effective = certificateUsage.Effective.AddDays(1)
                      CarrierRate = 1.24m } ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]
            let expected = [ certificateUsage ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate when given no plan selections`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25)) |> withPlanSelections []

            let rates = [ Stubs.CarrierRateModificationDomainModel.domain ]
            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]
            let expected = [ certificateUsage ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate when given no rate changes`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let rates = []
            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]
            let expected = [ certificateUsage ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate when given no certificate usages`` () =
            let rates = [ Stubs.CarrierRateModificationDomainModel.domain ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates []
            let expected = []

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate when the carrier differs from a certificates`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let planSectionForRateChange = certificateUsage.PlanSelections[0]

            let rates =
                [ { Stubs.CarrierRateModificationDomainModel.domain with
                      Carrier = "Another Carrier"
                      PolicyNumber = certificateUsage.ScbPolicyNumber
                      Option = planSectionForRateChange.Option
                      Coverage = planSectionForRateChange.Coverage
                      ProductLine = planSectionForRateChange.ProductLine
                      Effective = certificateUsage.Effective.AddDays(-1)
                      CarrierRate = 1.24m } ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]
            let expected = [ certificateUsage ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate when the coverage differs from a certificates`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let planSectionForRateChange = certificateUsage.PlanSelections[0]

            let rates =
                [ { Stubs.CarrierRateModificationDomainModel.domain with
                      Carrier = certificateUsage.Carrier
                      PolicyNumber = certificateUsage.ScbPolicyNumber
                      Option = planSectionForRateChange.Option
                      Coverage = None
                      ProductLine = planSectionForRateChange.ProductLine
                      Effective = certificateUsage.Effective.AddDays(-1)
                      CarrierRate = 1.24m } ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]
            let expected = [ certificateUsage ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate when the option differs from a certificates`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let planSectionForRateChange = certificateUsage.PlanSelections[0]

            let rates =
                [ { Stubs.CarrierRateModificationDomainModel.domain with
                      Carrier = certificateUsage.Carrier
                      PolicyNumber = certificateUsage.ScbPolicyNumber
                      Option = "Another Option"
                      Coverage = planSectionForRateChange.Coverage
                      ProductLine = planSectionForRateChange.ProductLine
                      Effective = certificateUsage.Effective.AddDays(-1)
                      CarrierRate = 1.24m } ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]
            let expected = [ certificateUsage ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate when the policy number differs from a certificates`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let planSectionForRateChange = certificateUsage.PlanSelections[0]

            let rates =
                [ { Stubs.CarrierRateModificationDomainModel.domain with
                      Carrier = certificateUsage.Carrier
                      PolicyNumber = "Another Policy Number"
                      Option = planSectionForRateChange.Option
                      Coverage = planSectionForRateChange.Coverage
                      ProductLine = planSectionForRateChange.ProductLine
                      Effective = certificateUsage.Effective.AddDays(-1)
                      CarrierRate = 1.24m } ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]
            let expected = [ certificateUsage ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``doesn't update a rate when the product line differs from a certificates`` () =
            let certificateUsage =
                coveredCertificateStub (DateTime(2023, 07, 25))
                |> withPlanSelections [ Stubs.PlanSelection.domain ]

            let planSectionForRateChange = certificateUsage.PlanSelections[0]

            let rates =
                [ { Stubs.CarrierRateModificationDomainModel.domain with
                      Carrier = certificateUsage.Carrier
                      PolicyNumber = certificateUsage.ScbPolicyNumber
                      Option = planSectionForRateChange.Option
                      Coverage = planSectionForRateChange.Coverage
                      ProductLine = "Another Product Line"
                      Effective = certificateUsage.Effective.AddDays(-1)
                      CarrierRate = 1.24m } ]

            let actual = CarrierRate.syncCertificateUsageCarrierRates rates [ certificateUsage ]
            let expected = [ certificateUsage ]

            Expect.equal actual expected "should equal"

module CertificateStatus =
    module ToString =

        [<Fact>]
        let ``maps Active to string`` () =
            let actual = CertificateStatus.Active.toString
            let expected = "active"

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``maps Terminated to string`` () =
            let actual = CertificateStatus.Terminated.toString
            let expected = "terminated"

            Expect.equal actual expected "should equal"
