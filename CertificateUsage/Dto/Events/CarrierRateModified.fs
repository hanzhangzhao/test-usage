module CertificateUsage.Dto.Events.CarrierRateModified

open System

open CertificateUsage.Dao
open CertificateUsage.Dto.Events.ChangedBy

type CarrierRateModifiedDto =
    { Carrier: string
      PolicyNumber: string
      Option: string
      Coverage: string option
      ProductLine: string
      EffectiveDate: DateTime
      CarrierRate: string
      ChangedBy: ChangedByDto
      DcOption: string option
      PricePer: int }

type CarrierAdjustmentDto =
    { AffectedMembers: CertificateUsageChangeDao
      EffectiveDate: DateTime
      CarrierRate: string
      ChangeBy: ChangedByDto }

let carrierRateModifiedDtoToCarrierAdjustmentDto (dto: CarrierRateModifiedDto) (dao: CertificateUsageChangeDao) =
    { AffectedMembers = dao
      EffectiveDate = dto.EffectiveDate
      CarrierRate = dto.CarrierRate
      ChangeBy = dto.ChangedBy }
