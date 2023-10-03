module CertificateUsage.Tests.Stubs.ExcludedCertificate

open CertificateUsage.Domain

let excludedCertificateStub effective =
    let certificateNumber = "CertificateNumber"
    let policyNumber = "PolicyNumber"
    let scbPolicyNumber = "ScbPolicyNumber"
    let carrier = "Carrier"
    let division = "Division"
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
    let taxProvince = "TaxProvince"

    CertificateUsage.ExclusionEvent
        { CertificateNumber = certificateNumber
          Carrier = carrier
          ClientName = clientName
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = policyNumber
          Effective = effective
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
