module CertificateUsage.Dto.Events.MemberTerminated

open System
open CertificateUsage.Dto.Events
open CertificateUsage.Dto.Events.Client
open CertificateUsage.Dto.Events.PlanSelection
open BenefitPeriod

type MemberTerminatedDto =
    { BenefitsStartDate: DateTime
      BenefitsEndedDate: DateTime option
      MemberId: int 
      PublicId: string option
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
    