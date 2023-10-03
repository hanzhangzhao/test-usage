module CertificateUsage.Tests.Stubs.CarrierRateModifiedDto

open System
open CertificateUsage.Dto.Events.CarrierRateModified

let effectiveDate = DateTime(2023, 07, 19)

let dto =
    { CarrierRateModifiedDto.Carrier = "Carrier"
      PolicyNumber = "PolicyNumber"
      Option = "Option"
      Coverage = Some "Coverage"
      ProductLine = "ProductLine"
      EffectiveDate = effectiveDate
      CarrierRate = "1.23"
      ChangedBy = { Id = 2; Name = "ChangedBy.Name" }
      DcOption = Some "DcOption"
      PricePer = 3 }
