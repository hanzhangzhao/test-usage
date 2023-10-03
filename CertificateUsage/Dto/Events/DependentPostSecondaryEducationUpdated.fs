module CertificateUsage.Dto.Events.DependentPostSecondaryEducationUpdated

open System

open CertificateUsage.Dto.Events.BenefitPeriod
open CertificateUsage.Dto.Events.PlanSelection
open CertificateUsage.Dto.Events.Client
open CertificateUsage.Dto.Events.EligiblityPeriod

type MemberDto =
    { PlanSelections: PlanSelectionDto list }

type DependentDto =
    { Id: int
      EligibilityPeriods: EligibilityPeriodDto list }

type DependentPostSecondaryEducationUpdatedDto =
    { ActiveBenefitPeriod: BenefitPeriodDto
      Member: MemberDto
      Dependent: DependentDto
      Client: Client }
