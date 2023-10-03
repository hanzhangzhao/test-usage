module CertificateUsage.Tests.Stubs.Certificate

open System

let certificateNumber = "CertificateNumber"
let carrierName = "CarrierName"
let clientName = "ClientName"
let scbPolicyNumber = "ScbPolicyNumber"
let policyNumber = "PolicyNumber"
let startDate = DateTime(2022, 05, 15)
let endDate = None
let division = "Division"
let productLine = "ProductLine"
let productLineGroup = "productLineGroup"
let coverage = "Coverage"
let option = "Option"
let ratePer = 1.1m
let volume = 1.2m
let volumeUnit = "CAD"
let carrierRate = 0.8m
let taxRate = 1.0m
let taxProvince = "Manitoba"

module Domain =
    open CertificateUsage.Domain

    let domain =
        { Certificate.CertificateNumber = CertificateNumber.create certificateNumber
          CarrierName = CarrierName.create carrierName
          ClientName = ClientName.create clientName
          ScbPolicyNumber = PolicyNumber.create scbPolicyNumber
          PolicyNumber = PolicyNumber.create policyNumber
          StartDate = startDate
          EndDate = endDate
          Division = Division.create division
          PlanSelections =
            [ { ProductLine = productLine
                ProductLineGroup = productLineGroup
                Coverage = Some coverage
                Option = option
                RatePer = ratePer
                Volume = { Amount = volume; Unit = volumeUnit }
                CarrierRate = carrierRate
                TaxRate = taxRate
                TaxProvince = taxProvince } ]
          CertificateStatus = CertificateStatus.Active }

module Dao =
    open CertificateUsage.Dao
    let dao =
        { CertificateUsage.Dao.CertificateDao.CertificateNumber = certificateNumber
          CarrierName = carrierName
          ClientName = clientName
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = policyNumber
          StartDate = startDate
          EndDate = endDate
          Division = division
          PlanSelections = [
              { ProductLine = productLine
                ProductLineGroup = productLineGroup
                Coverage = Some coverage
                Option = option
                RatePer = ratePer
                Volume = { Amount = volume; Unit = volumeUnit }
                CarrierRate = carrierRate
                TaxRate = taxRate
                TaxProvince = taxProvince }]
          CertificateStatus = "active" }
