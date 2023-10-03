module CertificateUsage.Dao

open System

open CertificateUsage.Errors
open CertificateUsage.Domain.CarrierRate

type CoverageType =
    | Covered = 0
    | Excluded = 1

type EventMetaDao =
    { EventId : Guid
      EventNo : uint64
      EventType : string
      EventDate : DateTime
      EventVersion : string
      EventStream : string }

type VolumeDao = { Amount : decimal; Unit : string }

type PlanSelectionDao =
    { ProductLine : string
      ProductLineGroup : string
      Coverage : string option
      Option : string
      RatePer : decimal
      Volume : VolumeDao
      CarrierRate : decimal
      TaxRate : decimal
      TaxProvince : string }

type CertificateEventDao =
    { CertificateNumber : string
      Carrier : string
      ClientName : string
      PolicyNumber : string
      Division : string
      Effective : DateTime
      PlanSelections : PlanSelectionDao list }

type CertificateUsageChangeDao =
    { CertificateNumber : string
      Carrier : string
      ScbPolicyNumber : string
      PolicyNumber : string
      Effective : DateTime
      Type : CoverageType
      CoverageData : CertificateEventDao
      EventMetadata : EventMetaDao }

type RateUpdateData =
    { Carrier : string
      PolicyNumber : string
      Option : string
      Coverage : string option
      ProductLine : string
      Effective : DateTime
      CarrierRate : Decimal
      ChangedBy : ChangedBy }

type RateUpdateDataDao =
    { Carrier : string
      PolicyNumber : string
      Option : string
      Coverage : string option
      ProductLine : string
      Effective : DateTime
      RateUpdateData : RateUpdateData
      EventMetadata : EventMetaDao }

type CertificateStatus =
    | Active = 0
    | Terminated = 1

type CertificateDao =
    { CertificateNumber : string
      CarrierName : string
      ClientName : string
      ScbPolicyNumber : string
      PolicyNumber : string
      StartDate : DateTime
      EndDate : DateTime option
      Division : string
      PlanSelections : PlanSelectionDao list
      CertificateStatus : string }

type CertificateUsageType =
    | Charge = 0
    | Reversal = 1
    | Correction = 2

type CertificateUsageDao =
    { Id : Guid
      CausationId : Guid
      CorrelatedUsageId : Guid option
      UsageType : CertificateUsageType
      CertificateNumber : string
      CarrierName : string
      ClientName : string
      PolicyNumber : string
      ScbPolicyNumber : string
      BenefitStartDate : DateTime
      BenefitEndDate : DateTime option
      Division : string
      ProductLine : string
      ProductLineGroup : string
      Coverage : string option
      Option : string
      RatePer : decimal
      VolumeAmount : decimal
      VolumeUnit : string
      CarrierRate : decimal
      TaxRate : decimal
      TaxProvince : string
      BillingEndDate : DateTime
      DateIncurred : DateTime }

type CertificateRecordDao =
    { CertificateNumber : string
      Carrier : string
      PolicyNumber : string
      ClientName : string
      Status : CertificateStatus
      Certificate : CertificateDao }

type RetroactiveCertificateUpdateType =
    | Enrollment = 0
    | Termination = 1
    | Update = 2

// for writing a retro update for a future closing of the book
type RetroactiveCertificateUpdateDao =
    { Type : RetroactiveCertificateUpdateType
      CertificateNumber : string
      CarrierName : string
      ClientName : string
      PolicyNumber : string
      ProductLine : string
      Option : string
      Coverage : string option
      UpdateDate : DateTime
      Backdate : DateTime }

// used to read a retro update when closing a book
type RetroactiveCertificateUsageUpdateDao =
    { Usage : CertificateUsageDao
      Certificate : CertificateDao
      ProductLine : string
      Coverage : string option
      Option : string }

// used to read a retro enrollment/termination when closing a book
type RetroactiveCertificateUsageTransitionDao =
    { RetroCertificateUpdateId : Guid
      Certificate : CertificateDao
      ProductLine : string
      Coverage : string option
      Option : string
      Usage : CertificateUsageDao option
      BillingStart : DateTime
      BillingEnd : DateTime
      Backdate : DateTime }

type RetroactiveCertificateUsageDao =
    | RetroactiveCertificateUsageUpdateDao of RetroactiveCertificateUsageUpdateDao
    | RetroactiveCertificateUsageTransitionDao of RetroactiveCertificateUsageTransitionDao

module CoverageType =
    let fromString (coverageType : string) =
        match coverageType with
        | "covered" -> CoverageType.Covered
        | "excluded" -> CoverageType.Excluded
        | _ -> raise (InvalidCoverageTypeEnumException($"{coverageType.ToString()}"))

module CertificateStatus =
    let fromString (status : string) =
        match status with
        | "active" -> CertificateStatus.Active
        | "terminated" -> CertificateStatus.Terminated
        | _ -> raise (InvalidCoverageTypeEnumException($"{status.ToString()}"))

module CertificateUsageType =
    let value usageType =
        match usageType with
        | CertificateUsageType.Charge -> "charge"
        | CertificateUsageType.Reversal -> "reversal"
        | CertificateUsageType.Correction -> "correction"
        | t -> raise (InvalidCertificateUsageTypeEnumException($"{t}"))

module RetroactiveCertificateUpdateType =
    let fromDomain domain =
        match domain with
        | Domain.RetroactiveCertificateUpdateType.Enrollment -> RetroactiveCertificateUpdateType.Enrollment
        | Domain.RetroactiveCertificateUpdateType.Termination -> RetroactiveCertificateUpdateType.Termination
        | Domain.RetroactiveCertificateUpdateType.Update -> RetroactiveCertificateUpdateType.Update
