module CertificateUsage.Dto.Events.MemberCobUpdated

open CertificateUsage.Dto.Events
open CoverageCoordinationDto
open BenefitPeriod
open PlanSelection
open Client
open DependentAdded

type MemberCobUpdatedDto =
    { IsEnrolled: bool
      BenefitPeriods: BenefitPeriodDto list
      PlanSelections: PlanSelectionDto list
      Client: Client
      CoverageCoordination: CoverageCoordinationDto }

type DependentCobUpdatedDto =
    { IsEnrolled: bool
      BenefitPeriods: BenefitPeriodDto list
      PlanSelections: PlanSelectionDto list
      Client: Client
      Dependent: DependentDto }

type SpouseCobUpdatedDto =
    { IsEnrolled: bool
      BenefitPeriods: BenefitPeriodDto list
      PlanSelections: PlanSelectionDto list
      Client: Client
      Spouse: SpouseDto }
