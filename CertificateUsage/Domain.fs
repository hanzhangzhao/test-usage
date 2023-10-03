module CertificateUsage.Domain

open System

open CertificateUsage.Errors

type Volume = { Amount : decimal; Unit : string }

type PlanSelection =
    { ProductLine : string
      ProductLineGroup : string
      Coverage : string option
      Option : string
      RatePer : decimal
      Volume : Volume
      CarrierRate : decimal
      TaxRate : decimal
      TaxProvince : string }

type CoveredCertificate =
    { CertificateNumber : string
      Carrier : string
      ClientName : string
      ScbPolicyNumber : string
      PolicyNumber : string
      Effective : DateTime
      Division : string
      PlanSelections : PlanSelection list }

type ExcludedCertificate =
    { CertificateNumber : string
      Carrier : string
      ClientName : string
      ScbPolicyNumber : string
      PolicyNumber : string
      Division : string
      Effective : DateTime
      PlanSelections : PlanSelection list }

type CertificateUsage =
    | CoveredEvent of CoveredCertificate
    | ExclusionEvent of ExcludedCertificate

    member this.Effective =
        match this with
        | CoveredEvent event -> event.Effective
        | ExclusionEvent event -> event.Effective

    member this.CertificateNumber =
        match this with
        | CoveredEvent event -> event.CertificateNumber
        | ExclusionEvent event -> event.CertificateNumber

    member this.ScbPolicyNumber =
        match this with
        | CoveredEvent event -> event.ScbPolicyNumber
        | ExclusionEvent event -> event.ScbPolicyNumber

    member this.PolicyNumber =
        match this with
        | CoveredEvent event -> event.PolicyNumber
        | ExclusionEvent event -> event.PolicyNumber

    member this.Carrier =
        match this with
        | CoveredEvent event -> event.Carrier
        | ExclusionEvent event -> event.Carrier

    member this.PlanSelections =
        match this with
        | CoveredEvent event -> event.PlanSelections
        | ExclusionEvent event -> event.PlanSelections

    member this.ClientName =
        match this with
        | CoveredEvent event -> event.ClientName
        | ExclusionEvent event -> event.ClientName

type CertificateStatus =
    | Active
    | Terminated

    member this.toString =
        match this with
        | Active -> "active"
        | Terminated -> "terminated"

type CertificateNumber = CertificateNumber of string

module CertificateNumber =
    let create (cert : string) = CertificateNumber cert
    let apply f (CertificateNumber cert) = f cert
    let value cert = apply id cert

type CarrierName = CarrierName of string

module CarrierName =
    let create (cert : string) = CarrierName cert
    let apply f (CarrierName carrier) = f carrier
    let value carrier = apply id carrier

type ClientName = ClientName of string

module ClientName =
    let create (cert : string) = ClientName cert
    let apply f (ClientName carrier) = f carrier
    let value carrier = apply id carrier

type PolicyNumber = PolicyNumber of string

module PolicyNumber =
    let create (cert : string) = PolicyNumber cert
    let apply f (PolicyNumber carrier) = f carrier
    let value carrier = apply id carrier

type Division = Division of string

module Division =
    let create (division : string) = Division division
    let apply f (Division carrier) = f carrier
    let value carrier = apply id carrier

type Certificate =
    { CertificateNumber : CertificateNumber
      CarrierName : CarrierName
      ClientName : ClientName
      ScbPolicyNumber : PolicyNumber
      PolicyNumber : PolicyNumber
      StartDate : DateTime
      EndDate : DateTime option
      Division : Division
      PlanSelections : PlanSelection list
      CertificateStatus : CertificateStatus }

type ProductLine = ProductLine of string

module ProductLine =
    let create (cert : string) = ProductLine cert
    let apply f (ProductLine carrier) = f carrier
    let value carrier = apply id carrier

type ProductLineGroup = ProductLineGroup of string

module ProductLineGroup =
    let create (cert : string) = ProductLineGroup cert
    let apply f (ProductLineGroup carrier) = f carrier
    let value carrier = apply id carrier

type Coverage = Coverage of string

module Coverage =
    let create (cert : string) = Coverage cert
    let apply f (Coverage carrier) = f carrier
    let value carrier = apply id carrier

type BenefitOption = BenefitOption of string

module BenefitOption =
    let create (cert : string) = BenefitOption cert
    let apply f (BenefitOption carrier) = f carrier
    let value carrier = apply id carrier

type TaxProvince = TaxProvince of string

module TaxProvince =
    let create (cert : string) = TaxProvince cert
    let apply f (TaxProvince carrier) = f carrier
    let value carrier = apply id carrier

type VolumeAmount = VolumeAmount of decimal

module VolumeAmount =
    let create (cert : decimal) = VolumeAmount cert
    let apply f (VolumeAmount carrier) = f carrier
    let value carrier = apply id carrier

type VolumeUnit = VolumeUnit of string

module VolumeUnit =
    let create (cert : string) = VolumeUnit cert
    let apply f (VolumeUnit carrier) = f carrier
    let value carrier = apply id carrier

type RatePer = RatePer of decimal

module RatePer =
    let create (cert : decimal) = RatePer cert
    let apply f (RatePer carrier) = f carrier
    let value carrier = apply id carrier

type Rate = Rate of decimal

module Rate =
    let create (cert : decimal) = Rate cert
    let apply f (Rate carrier) = f carrier
    let value carrier = apply id carrier

type CertificateUsageType =
    | Charge
    | Reversal
    | Correction

type Usage =
    { Id : Guid
      UsageType : CertificateUsageType
      CertificateNumber : CertificateNumber
      CarrierName : CarrierName
      ClientName : ClientName
      ScbPolicyNumber : PolicyNumber
      PolicyNumber : PolicyNumber
      BenefitStartDate : DateTime
      BenefitEndDate : DateTime option
      Division : Division
      ProductLine : ProductLine
      ProductLineGroup : ProductLineGroup
      Coverage : Coverage option
      Option : BenefitOption
      RatePer : RatePer
      VolumeAmount : VolumeAmount
      VolumeUnit : VolumeUnit
      CarrierRate : Rate
      TaxRate : Rate
      TaxProvince : TaxProvince
      BillingEndDate : DateTime
      DateIncurred : DateTime }

type RetroactiveCertificateUpdateType =
    | Enrollment
    | Termination
    | Update

type RetroactiveCertificateUpdate =
    { Type : RetroactiveCertificateUpdateType
      CertificateNumber : CertificateNumber
      CarrierName : CarrierName
      ClientName : ClientName
      PolicyNumber : PolicyNumber
      ProductLine : ProductLine
      Option : BenefitOption
      Coverage : Coverage option
      UpdateDate : DateTime
      Backdate : DateTime }

type Correction =
    { Correction : Usage option
      Reversal : Usage option }

type BillingPeriod =
    { Year : int
      Month : int }

    member this.Intersects(date : DateTime) =
        match this.Year, this.Month with
        | y, m when y = date.Year && m = date.Month -> true
        | _ -> false

    member this.Before(date : DateTime) =
        date < DateTime(this.Year, this.Month, 01)

let tryFindDivisionCode carrierCodes = carrierCodes |> Map.tryFind "division"

let certificateUsageForPeriod (billingPeriod : BillingPeriod) (certificateUsages : CertificateUsage list) =
    certificateUsages
    |> List.filter (fun usage ->
        match usage with
        | CoveredEvent event -> billingPeriod.Before event.Effective
        | ExclusionEvent event -> billingPeriod.Intersects event.Effective)
    |> List.sortByDescending (fun usage -> usage.Effective)
    |> List.groupBy (fun usage -> usage.CertificateNumber)
    |> List.map (fun (_, usages) -> usages |> List.head)

module CarrierRate =
    type ChangedBy = { Id : int; Name : string }

    type CarrierRateModification =
        { Carrier : string
          PolicyNumber : string
          Option : string
          Coverage : string option
          ProductLine : string
          Effective : DateTime
          CarrierRate : decimal
          ChangedBy : ChangedBy }

    let rateIsForPlanSelectionOfUsage
        (rate : CarrierRateModification)
        (usage : CertificateUsage)
        (selection : PlanSelection)
        =
        rate.Carrier = usage.Carrier
        && rate.Coverage = selection.Coverage
        && rate.Option = selection.Option
        && rate.ProductLine = selection.ProductLine
        && rate.PolicyNumber = usage.ScbPolicyNumber

    let rateIsBeforeUsageEffectiveDate (rate : CarrierRateModification) (usage : CertificateUsage) =
        rate.Effective <= usage.Effective

    let syncPlanSelectionCarrierRates
        (rates : CarrierRateModification list)
        (usage : CertificateUsage)
        (ps : PlanSelection)
        =
        rates
        |> List.tryFind (fun rate ->
            rateIsForPlanSelectionOfUsage rate usage ps
            && rateIsBeforeUsageEffectiveDate rate usage)
        |> Option.map (fun rate ->
            { ps with
                CarrierRate = rate.CarrierRate })
        |> Option.defaultValue ps


    let syncCertificateUsageChangeDaoCarrierRate (rates : CarrierRateModification list) (usage : CertificateUsage) =
        let planSelections =
            usage.PlanSelections |> List.map (syncPlanSelectionCarrierRates rates usage)

        match usage with
        | CoveredEvent covered ->
            { covered with
                PlanSelections = planSelections }
            |> CoveredEvent
        | ExclusionEvent exclusion ->
            { exclusion with
                PlanSelections = planSelections }
            |> ExclusionEvent

    let syncCertificateUsageCarrierRates (rates : CarrierRateModification list) (usages : CertificateUsage list) =
        usages |> List.map (syncCertificateUsageChangeDaoCarrierRate rates)
