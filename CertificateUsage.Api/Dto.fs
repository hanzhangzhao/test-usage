module CertificateUsage.Api.Dto

open System
open System.Text.Json.Serialization
open CertificateUsage.Domain

type BillingReadModelDto =
    { CarrierCode : string
      CertificateNumber : string
      ClientName : string
      PolicyNumber : string
      ProductLine : string
      Coverage : string option
      [<JsonName("option")>]
      ProductOption : string
      Volume : decimal
      Lives : decimal
      TaxRate : decimal
      TaxProvince : string
      Year : int
      Month : int
      CarrierRate : decimal
      ClientRate : decimal
      Division : string }

type UsagePreviewDto =
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

// TODO: need to fill out, Lives & ClientRate
module BillingReadModelDto =
    let toDto (domain : CertificateUsage) : BillingReadModelDto list =
        match domain with
        | CoveredEvent event ->
            event.PlanSelections
            |> List.map (fun p ->
                { CarrierCode = event.Carrier
                  CertificateNumber = event.CertificateNumber
                  ClientName = event.ClientName
                  PolicyNumber = event.PolicyNumber
                  ProductLine = p.ProductLine
                  ProductOption = p.Option
                  Coverage = p.Coverage
                  Volume = p.Volume.Amount
                  Lives = 0.0m
                  TaxRate = p.TaxRate
                  Year = event.Effective.Year
                  Month = event.Effective.Month
                  CarrierRate = p.CarrierRate
                  ClientRate = 0.0m
                  TaxProvince = p.TaxProvince
                  Division = event.Division })
        | ExclusionEvent event ->
            event.PlanSelections
            |> List.map (fun p ->
                { CarrierCode = event.Carrier
                  CertificateNumber = event.CertificateNumber
                  ClientName = event.ClientName
                  PolicyNumber = event.PolicyNumber
                  ProductLine = p.ProductLine
                  ProductOption = p.Option
                  Coverage = p.Coverage
                  Volume = p.Volume.Amount
                  Lives = 0.0m
                  TaxRate = p.TaxRate
                  Year = event.Effective.Year
                  Month = event.Effective.Month
                  CarrierRate = p.CarrierRate
                  ClientRate = 0.0m
                  TaxProvince = p.TaxProvince
                  Division = event.Division })
