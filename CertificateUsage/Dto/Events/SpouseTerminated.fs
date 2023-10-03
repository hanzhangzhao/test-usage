module CertificateUsage.Dto.Events.SpouseTerminated

open System

open CertificateUsage.Dto.Events.BenefitPeriod
open CertificateUsage.Dto.Events.PlanSelection
open CertificateUsage.Dto.Events.Client

type SpouseTerminatedDto =
    { IsEnrolled: bool
      BenefitPeriods: BenefitPeriodDto list
      PlanSelections: PlanSelectionDto list
      Client: Client
      EffectiveEndDate: DateTime }
