module CertificateUsage.Dto.Events.PlanSelection

open System

open CertificateUsage.Dto.Events.Volume

type PlanSelectionDto =
    { LineGroup : string
      Selection : string
      ProductLine : string
      Coverage : string option
      PricePer : decimal
      Volume : VolumeDto
      CarrierRate : string
      TaxRate : string
      TaxProvince : string }
