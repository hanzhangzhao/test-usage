module CertificateUsage.Tests.Stubs.MemberRateChanged

open System

open CertificateUsage.Dto.Events.MemberRateChanged

let benefitStartDate = DateTime(2023, 9, 25)
let taxProvince = "British Columbia"
let planSelections = []
let certificateNumber = "CertificateNumber"
let policyNumber = "PolicyNumber"
let externalPolicyNumber = "ExternalPolicyNumber"
let carrierClientCode = "CarrierClientCode"
let carrier = "Carrier"
let carrierMapping = None
let clientName = "Client.Name"
let Coverages = None

let productLine = "ProductLine"
let productLineGroup = "ProductLineGroup"
let coverage = "Coverage"
let option = "Option"
let ratePer = 1.23m
let volumeAmount = 2.31m
let volumeUnit = "quantity"
let carrierRate = 3.12m
let taxRate = 2.13m

module Dto =
    let stub =
        { MemberRateChangedDto.BenefitsStartDate = benefitStartDate
          TaxProvince = taxProvince
          CertificateNumber = certificateNumber
          PolicyNumber = policyNumber
          ExternalPolicyNumber = Some externalPolicyNumber
          CarrierClientCode = Some carrierClientCode
          Carrier = carrier
          CarrierMapping = carrierMapping
          Client = { Name = Some clientName }
          Coverages = Coverages
          PlanSelections =
            [ { ProductLine = productLine
                LineGroup = productLineGroup
                Selection = option
                Coverage = Some coverage
                PricePer = ratePer
                Volume =
                  { Amount = volumeAmount
                    Units = volumeUnit }
                CarrierRate = carrierRate |> string
                TaxRate = taxRate |> string
                TaxProvince = taxProvince } ] }
