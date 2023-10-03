module CertificateUsage.Dto.Events.DependentTerminated

open System

open CertificateUsage.Dto.Events.BenefitPeriod
open CertificateUsage.Dto.Events.PlanSelection
open CertificateUsage.Dto.Events.Client

type DependentTerminatedDto =
    { IsEnrolled: bool
      BenefitPeriods: BenefitPeriodDto list
      PlanSelections: PlanSelectionDto list
      Client: Client
      EffectiveEndDate: DateTime }
