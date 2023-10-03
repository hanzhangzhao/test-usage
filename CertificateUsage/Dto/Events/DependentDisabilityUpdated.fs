module CertificateUsage.Dto.Events.DependentDisabilityUpdated

open System

open CertificateUsage.Dto.Events.BenefitPeriod
open CertificateUsage.Dto.Events.PlanSelection
open CertificateUsage.Dto.Events.Client

type MemberDto =
    { PlanSelections: PlanSelectionDto list }

type DependentDto = { DateOfDisability: DateTime }

type DependentDisabilityUpdatedDto =
    { ActiveBenefitPeriod: BenefitPeriodDto
      Member: MemberDto
      Dependent: DependentDto
      Client: Client }
