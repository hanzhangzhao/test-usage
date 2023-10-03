module CertificateUsage.Tests.Stubs.Usage

open System

let id = Guid.Parse("5640F21F-01E4-4B32-B9D9-61EE22FC9A70")
let causationId = Guid.Parse("08E53A46-DB22-42E7-ACDC-8415C0CF6292")
let correlatedUsageId = Guid.Parse("13BF5526-E302-46C6-969C-57E6E52EAF80")
let certificateNumber = "CertificateNumber"
let carrierName = "CarrierName"
let clientName = "ClientName"
let policyNumber = "PolicyNumber"
let scbPolicyNumber = "ScbPolicyNumber"
let benefitStartDate = DateTime(2022, 5, 15)
let benefitEndDate = None
let division = "Division"
let productLine = "ProductLine"
let productLineGroup = "ProductLineGroup"
let coverage = "Coverage"
let option = "Option"
let ratePer = 1.23m
let volumeAmount = 2.34m
let volumeUnit = "CAD"
let carrierRate = 3.45m
let taxRate = 4.56m
let taxProvince = "Manitoba"
let billingEndDate = DateTime(2023, 8, 31)
let dateIncurred = DateTime(2023, 6, 30)

module Domain =
    open CertificateUsage.Domain

    let usageType = CertificateUsageType.Charge

    let domain =
        { Id = id
          UsageType = usageType
          CertificateNumber = CertificateNumber.create certificateNumber
          CarrierName = CarrierName.create carrierName
          ClientName = ClientName.create clientName
          ScbPolicyNumber = PolicyNumber.create scbPolicyNumber
          PolicyNumber = PolicyNumber.create policyNumber
          BenefitStartDate = benefitStartDate
          BenefitEndDate = benefitEndDate
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
          BillingEndDate = billingEndDate
          DateIncurred = dateIncurred }
