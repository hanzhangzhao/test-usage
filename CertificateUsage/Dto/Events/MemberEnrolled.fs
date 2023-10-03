﻿module CertificateUsage.Dto.Events.MemberEnrolled

open System
open CertificateUsage.Dto.Events.BenefitPeriod
open CertificateUsage.Dto.Events.PlanSelection
open CertificateUsage.Dto.Events.Client

type MetaData = { Version: string }

type MemberEnrollmentDto =
    { BenefitsStartDate: DateTime
      TaxProvince: string
      PlanSelections: PlanSelectionDto list
      CertificateNumber: string
      PolicyNumber: string
      ExternalPolicyNumber: string option
      CarrierClientCode: string option
      Carrier: string
      Coverages: Coverages option
      CarrierMapping: CarrierMapping option
      Client: Client }
