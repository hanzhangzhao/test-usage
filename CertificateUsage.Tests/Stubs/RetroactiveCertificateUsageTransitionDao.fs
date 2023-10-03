module CertificateUsage.Tests.Stubs.RetroactiveCertificateUsageTransitionDao

open System

let certificateNumber = "CertificateNumber"
let carrierName = "CarrierName"
let clientName = "ClientName"
let scbPolicyNumber = "ScbPolicyNumber"
let policyNumber = "PolicyNumber"
let benefitStartDate = DateTime(2022, 5, 15)
let division = "Division"
let productLine = "ProductLine"
let productLineGroup = "ProductLineGroup"
let coverage = "Coverage"
let option = "Option"

let billingDate = DateTime(2023, 08, 31)
let dateIncurred = DateTime(2023, 05, 31)
let backDate = DateTime(2023, 06, 15)
let ratePer = 1.2m
let volumeAmount = 2.3m
let volumeUnit = "CAD"
let carrierRate = 3.4m
let taxRate = 4.5m
let taxProvince = "Manitoba"

module Dao =
    open CertificateUsage.Dao

    let certificate =
        { CertificateDao.CertificateNumber = certificateNumber
          CarrierName = carrierName
          ClientName = clientName
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = policyNumber
          StartDate = benefitStartDate
          EndDate = None
          Division = division
          PlanSelections =
            [ { ProductLine = productLine
                ProductLineGroup = productLineGroup
                Coverage = Some coverage
                Option = option
                RatePer = ratePer
                Volume =
                  { Amount = volumeAmount
                    Unit = volumeUnit }
                CarrierRate = carrierRate
                TaxRate = taxRate
                TaxProvince = taxProvince } ]
          CertificateStatus = "active" }

    let stub id causationId correlatedUsageId =
        { RetroactiveCertificateUsageTransitionDao.RetroCertificateUpdateId = causationId
          Usage =
            { CertificateUsageDao.Id = id
              CorrelatedUsageId = Some correlatedUsageId
              CausationId = causationId
              UsageType = CertificateUsageType.Charge
              CertificateNumber = certificateNumber
              CarrierName = carrierName
              ClientName = clientName
              PolicyNumber = policyNumber
              ScbPolicyNumber = scbPolicyNumber
              BenefitStartDate = benefitStartDate
              BenefitEndDate = None
              Division = division
              ProductLine = productLine
              ProductLineGroup = productLineGroup
              Coverage = Some coverage
              Option = option
              RatePer = ratePer
              VolumeAmount = volumeAmount
              VolumeUnit = volumeUnit
              CarrierRate = carrierRate
              TaxRate = taxRate
              TaxProvince = taxProvince
              BillingEndDate = billingDate
              DateIncurred = dateIncurred }
            |> Some
          Certificate = certificate
          BillingEnd = billingDate
          BillingStart = DateTime(2023, 8, 01)
          Backdate = backDate
          ProductLine = productLine
          Coverage = Some coverage
          Option = option }

module Domain =
    open CertificateUsage.Domain

    let stub id =
        { Reversal =
            { Usage.Id = id
              UsageType = CertificateUsageType.Reversal
              CertificateNumber = CertificateNumber.create certificateNumber
              CarrierName = CarrierName.create carrierName
              ClientName = ClientName.create clientName
              ScbPolicyNumber = PolicyNumber.create scbPolicyNumber
              PolicyNumber = PolicyNumber.create policyNumber
              BenefitStartDate = benefitStartDate
              BenefitEndDate = None
              Division = Division.create division
              ProductLine = ProductLine.create productLine
              ProductLineGroup = ProductLineGroup.create productLineGroup
              Coverage = Some(Coverage.create coverage)
              Option = BenefitOption.create option
              RatePer = RatePer.create ratePer
              VolumeAmount = VolumeAmount.create volumeAmount
              VolumeUnit = VolumeUnit.create volumeUnit
              CarrierRate = Rate.create carrierRate
              TaxRate = Rate.create taxRate
              TaxProvince = TaxProvince.create taxProvince
              BillingEndDate = billingDate
              DateIncurred = dateIncurred }
            |> Some
          Correction =
            { Usage.Id = id
              UsageType = CertificateUsageType.Correction
              CertificateNumber = CertificateNumber.create certificateNumber
              CarrierName = CarrierName.create carrierName
              ClientName = ClientName.create clientName
              ScbPolicyNumber = PolicyNumber.create scbPolicyNumber
              PolicyNumber = PolicyNumber.create policyNumber
              BenefitStartDate = benefitStartDate
              BenefitEndDate = None
              Division = Division.create division
              ProductLine = ProductLine.create productLine
              ProductLineGroup = ProductLineGroup.create productLineGroup
              Coverage = Some(Coverage.create coverage)
              Option = BenefitOption.create option
              RatePer = RatePer.create ratePer
              VolumeAmount = VolumeAmount.create volumeAmount
              VolumeUnit = VolumeUnit.create volumeUnit
              CarrierRate = Rate.create carrierRate
              TaxRate = Rate.create taxRate
              TaxProvince = TaxProvince.create taxProvince
              BillingEndDate = billingDate
              DateIncurred = dateIncurred }
            |> Some }
