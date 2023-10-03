module CertificateUsage.Api.Dao

open System

open CertificateUsage.Api.Dto

type UsagePreviewDao =
    { CertificateNumber : string
      CarrierName : string
      ClientName : string
      PolicyNumber : string
      ScbPolicyNumber : string
      BenefitStartDate : DateTime
      BenefitEndDate : DateTime option
      Division : string
      ProductLine : string
      ProductLineGroup : string option
      Coverage : string option
      Option : string
      RatePer : decimal option
      VolumeAmount : decimal
      VolumeUnit : string
      CarrierRate : decimal
      TaxRate : decimal option
      TaxProvince : string }

type UsageLineDao =
    { UsageType : string
      CarrierCode : string
      CertificateNumber : string
      ClientName : string
      PolicyNumber : string
      ProductLine : string
      Coverage : string option
      ProductOption : string
      Volume : decimal
      VolumeUnit : string
      Lives : decimal
      TaxRate : decimal
      TaxProvince : string
      Year : int
      Month : int
      CarrierRate : decimal
      ClientRate : decimal
      Division : string }

let usagePreviewDaoToUsagePreviewDto (dao : UsagePreviewDao) =
    { UsagePreviewDto.CertificateNumber = dao.CertificateNumber
      CarrierName = dao.CarrierName
      ClientName = dao.ClientName
      PolicyNumber = dao.PolicyNumber
      ScbPolicyNumber = dao.ScbPolicyNumber
      BenefitStartDate = dao.BenefitStartDate
      BenefitEndDate = dao.BenefitEndDate
      Division = dao.Division
      ProductLine = dao.ProductLine
      ProductLineGroup = dao.ProductLineGroup
      Coverage = dao.Coverage
      Option = dao.Option
      RatePer = dao.RatePer
      VolumeAmount = dao.VolumeAmount
      VolumeUnit = dao.VolumeUnit
      CarrierRate = dao.CarrierRate
      TaxRate = dao.TaxRate
      TaxProvince = dao.TaxProvince }

let usageLineDaoToBillingReadModelDto (dao : UsageLineDao) =
    { BillingReadModelDto.CarrierCode = dao.CarrierCode
      CertificateNumber = dao.CertificateNumber
      ClientName = dao.ClientName
      PolicyNumber = dao.PolicyNumber
      ProductLine = dao.ProductLine
      Coverage = dao.Coverage
      ProductOption = dao.ProductOption
      Volume = dao.Volume
      Lives = dao.Lives
      TaxRate = dao.TaxRate
      TaxProvince = dao.TaxProvince
      Year = dao.Year
      Month = dao.Month
      CarrierRate = dao.CarrierRate
      ClientRate = dao.ClientRate
      Division = dao.Division }
