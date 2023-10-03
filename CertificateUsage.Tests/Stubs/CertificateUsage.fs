module CertificateUsage.Tests.Stubs.CertificateUsageDao

open System

open CertificateUsage.Dao

let id = Guid.Parse("B761D28E-5962-41BF-BD09-06AEF0D5C79F")
let causationId = Guid.Parse("08E53A46-DB22-42E7-ACDC-8415C0CF6292")
let correlatedUsageId = Guid.Parse("13BF5526-E302-46C6-969C-57E6E52EAF80")
let usageType = CertificateUsageType.Charge
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

let dao =
    { CertificateUsageDao.Id = id
      CausationId = causationId
      CorrelatedUsageId = Some correlatedUsageId
      UsageType = usageType
      CertificateNumber = certificateNumber
      CarrierName = carrierName
      ClientName = clientName
      PolicyNumber = policyNumber
      ScbPolicyNumber = scbPolicyNumber
      BenefitStartDate = benefitStartDate
      BenefitEndDate = benefitEndDate
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
      BillingEndDate = billingEndDate
      DateIncurred = dateIncurred }
