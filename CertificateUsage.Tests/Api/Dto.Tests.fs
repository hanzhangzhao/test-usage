namespace CertificateUsage.UnitTests.Api

open System
open Xunit
open Expecto

open CertificateUsage.Api.Dto
open CertificateUsage.Domain

module Dto =
    module Stubs =
        let benefitStartDate = DateTime(2023, 6, 14)
        let taxProvince = "TaxProvince"
        let certificateNumber = "CertificateNumber"
        let policyNumber = "PolicyNumber"
        let scbPolicyNumber = "ScbPolicyNumber"
        let carrier = "Carrier"
        let division = "Division"
        let carrierMapping = Map [ "division", division ]
        let productLine = "ProductLine"
        let lineGroup = "LineGroup"
        let option = "Option"
        let coverage = "Coverage"
        let displayPricePer = 1.1m
        let volumeAmount = 1.2m
        let volumeUnits = "CAD"
        let clientName = "ClientName"
        let carrierRate = 0.8m
        let taxRate = 1.3m

        let coveredCertificate =
            { CoveredCertificate.CertificateNumber = certificateNumber
              Carrier = carrier
              ClientName = clientName
              ScbPolicyNumber = scbPolicyNumber
              PolicyNumber = policyNumber
              Effective = benefitStartDate
              Division = division
              PlanSelections =
                [ { ProductLine = productLine
                    ProductLineGroup = lineGroup
                    Coverage = Some coverage
                    Option = option
                    RatePer = displayPricePer
                    Volume =
                      { Amount = volumeAmount
                        Unit = volumeUnits }
                    CarrierRate = carrierRate
                    TaxRate = taxRate
                    TaxProvince = taxProvince } ] }

        let coveredEvent = coveredCertificate |> CoveredEvent

        let excludedCertificate =
            { ExcludedCertificate.CertificateNumber = certificateNumber
              Carrier = carrier
              ClientName = clientName
              ScbPolicyNumber = scbPolicyNumber
              PolicyNumber = policyNumber
              Effective = benefitStartDate
              Division = division
              PlanSelections =
                [ { ProductLine = productLine
                    ProductLineGroup = lineGroup
                    Coverage = Some coverage
                    Option = option
                    RatePer = displayPricePer
                    Volume =
                      { Amount = volumeAmount
                        Unit = volumeUnits }
                    CarrierRate = carrierRate
                    TaxRate = taxRate
                    TaxProvince = taxProvince } ] }

        let excludedEvent = excludedCertificate |> ExclusionEvent

    [<Fact>]
    let ``should covered domain -> response`` () =
        let expected =
            [ { CarrierCode = "Carrier"
                CertificateNumber = "CertificateNumber"
                ClientName = "ClientName"
                PolicyNumber = "PolicyNumber"
                ProductLine = "ProductLine"
                ProductOption = "Option"
                Coverage = Some "Coverage"
                Volume = Stubs.volumeAmount
                Lives = 0.0m
                TaxRate = Stubs.taxRate
                Year = 2023
                Month = 6
                CarrierRate = Stubs.carrierRate
                ClientRate = 0.0m
                TaxProvince = Stubs.taxProvince
                Division = Stubs.division } ]

        let actual = Stubs.coveredEvent |> BillingReadModelDto.toDto

        Expect.equal actual expected ""

    // TODO: fix this test https://sterlingcb.atlassian.net/browse/PLT-714
    [<Fact>]
    let ``should excluded domain -> response`` () =
        let expected =
            [ { CarrierCode = "Carrier"
                CertificateNumber = "CertificateNumber"
                ClientName = "ClientName"
                PolicyNumber = "PolicyNumber"
                ProductLine = "ProductLine"
                ProductOption = "Option"
                Coverage = Some "Coverage"
                Volume = Stubs.volumeAmount
                Lives = 0.0m
                TaxRate = Stubs.taxRate
                Year = 2023
                Month = 6
                CarrierRate = Stubs.carrierRate
                ClientRate = 0.0m
                TaxProvince = Stubs.taxProvince
                Division = Stubs.division } ]

        let actual = Stubs.excludedEvent |> BillingReadModelDto.toDto

        Expect.equal actual expected ""
