module CertificateUsage.Dto.Events.MemberCancelled

open System
open CertificateUsage.Dto.Events
open CertificateUsage.Dto.Events.Client
open CertificateUsage.Dto.Events.PlanSelection
open BenefitPeriod

type MemberCancelledDto =
    { BenefitsStartDate: DateTime
      BenefitsEndedDate: DateTime option
      IsEnrolled: bool
      MemberId: int
      DateOfBirth: DateTime option
      TaxProvince: string
      CertificateNumber: string
      PolicyNumber: string
      ExternalPolicyNumber: string option
      CarrierClientCode: string option
      Carrier: string
      BenefitClassCode: string
      Client: Client
      CarrierMapping: CarrierMapping option
      PlanSelections: PlanSelectionDto list }
    