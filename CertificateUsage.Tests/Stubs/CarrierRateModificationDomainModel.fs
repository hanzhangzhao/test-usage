module CertificateUsage.Tests.Stubs.CarrierRateModificationDomainModel

open System

open CertificateUsage.Domain.CarrierRate

let effective = DateTime(2023, 07, 19)
let carrierRate = 1.23m
let changedById = 1

let domain =
    { CarrierRateModification.Carrier = "Carrier"
      PolicyNumber = "PolicyNumber"
      Option = "Option"
      Coverage = Some "Coverage"
      ProductLine = "ProductLine"
      Effective = effective
      CarrierRate = carrierRate
      ChangedBy =
        { Id = changedById
          Name = "ChangedBy.Name" } }
