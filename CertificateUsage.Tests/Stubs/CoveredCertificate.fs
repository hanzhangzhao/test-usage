module CertificateUsage.Tests.Stubs.CoveredCertificate

open CertificateUsage.Domain

let coveredCertificateStub effective =
    let certificateNumber = "CertificateNumber"
    let scbPolicyNumber = "Number"
    let policyNumber = "policyNumber"
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
    let taxRate = 1.0m
    let taxProvince = "TaxProvince"

    CertificateUsage.CoveredEvent
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

let withPlanSelections (planSelections: PlanSelection list) (domainModel: CertificateUsage) =
    match domainModel with
    | CoveredEvent covered ->
        { covered with
            PlanSelections = planSelections }
        |> CoveredEvent
    | ExclusionEvent exclusion ->
        { exclusion with
            PlanSelections = planSelections }
        |> ExclusionEvent
