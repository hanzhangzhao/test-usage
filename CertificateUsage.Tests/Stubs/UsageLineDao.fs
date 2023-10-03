module CertificateUsage.Tests.Stubs.UsageLineDao

open System

open CertificateUsage.Dao
open CertificateUsage.Api.Dao

let benefitStartDate = DateTime(2023, 8, 18)
let benefitEndDate = DateTime(2024, 8, 18)
let ratePer = 1.23m
let volumeAmount = 2.34m
let carrierRate = 3.45m
let taxRate = 4.56m
let clientRate = 5.67m
let year = 2023
let month = 7
let lives = 0m

let dao =
    { UsageLineDao.UsageType = "usage"
      CertificateNumber = "CertificateNumber"
      CarrierCode = "CarrierName"
      ClientName = "ClientName"
      PolicyNumber = "PolicyNumber"
      Year = year
      Month = month
      Division = "Division"
      ProductLine = "ProductLine"
      Coverage = Some "Coverage"
      ProductOption = "Option"
      Volume = volumeAmount
      Lives = lives
      CarrierRate = carrierRate
      ClientRate = clientRate
      TaxRate = taxRate
      TaxProvince = "TaxProvince"
      VolumeUnit = "quantity" }
