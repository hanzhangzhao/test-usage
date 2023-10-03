module CertificateUsage.Tests.Stubs.RetroactiveUpdate

open System

open CertificateUsage
open CertificateUsage.Domain
open CertificateUsage.Dao

let createdDate = DateTime(2023, 08, 23)
let effectiveDate = createdDate.AddMonths(-2)
let certificateNumber = "CertificateNumber"
let carrierName = "CarrierName"
let clientName = "ClientName"
let policyNumber = "PolicyNumber"
let productLine = "ProductLine"
let option = "Option"
let coverage = "Coverage"

let domainModel =
    { RetroactiveCertificateUpdate.Type = Domain.RetroactiveCertificateUpdateType.Update
      CertificateNumber = CertificateNumber.create certificateNumber
      CarrierName = CarrierName.create carrierName
      ClientName = ClientName.create clientName
      PolicyNumber = PolicyNumber.create policyNumber
      ProductLine = ProductLine.create productLine
      Option = BenefitOption.create option
      Coverage = Some(Coverage.create coverage)
      UpdateDate = createdDate
      Backdate = effectiveDate }

let dao =
    { RetroactiveCertificateUpdateDao.Type = RetroactiveCertificateUpdateType.Update
      CertificateNumber = certificateNumber
      CarrierName = carrierName
      ClientName = clientName
      PolicyNumber = policyNumber
      ProductLine = productLine
      Option = option
      Coverage = Some coverage
      UpdateDate = createdDate
      Backdate = effectiveDate }
