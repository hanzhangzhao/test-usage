module CertificateUsage.Tests.Stubs.UsagePreviewDao

open System

open CertificateUsage.Api.Dao

let benefitStartDate = DateTime(2023, 8, 18)
let benefitEndDate = DateTime(2024, 8, 18)
let ratePer = 1.23m
let volumeAmount = 2.34m
let carrierRate = 3.45m
let taxRate = 4.56m

let dao =
    { CertificateNumber = "CertificateNumber"
      CarrierName = "CarrierName"
      ClientName = "ClientName"
      PolicyNumber = "PolicyNumber"
      ScbPolicyNumber = "ScbPolicyNumber"
      BenefitStartDate = benefitStartDate
      BenefitEndDate = Some benefitEndDate
      Division = "Division"
      ProductLine = "ProductLine"
      ProductLineGroup = Some "ProductLineGroup"
      Coverage = Some "Coverage"
      Option = "Option"
      RatePer = Some ratePer
      VolumeAmount = volumeAmount
      VolumeUnit = "VolumeUnit"
      CarrierRate = carrierRate
      TaxRate = Some taxRate
      TaxProvince = "TaxProvince" }
