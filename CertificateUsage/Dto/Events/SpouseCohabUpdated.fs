module CertificateUsage.Dto.Events.SpouseCohabUpdated

open System

open CertificateUsage.Dto.Events
open BenefitPeriod
open PlanSelection
open Client

type Spouse = { CohabitationDate: DateTime option }

type SpouseCohabUpdatedDto =
    { IsEnrolled: bool
      BenefitPeriods: BenefitPeriodDto list
      PlanSelections: PlanSelectionDto list
      Client: Client
      Spouse: Spouse }
