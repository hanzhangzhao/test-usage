module CertificateUsage.Dto.Events.DependentAdded

open System
open CertificateUsage.Dto.Events
open BenefitPeriod
open EligiblityPeriod
open PlanSelection
open Client
open CoverageCoordinationDto

type DependentReferenceDto = { Id: int }

type EmployeeProfileDto =
    { ProfileId: int
      PreferredLanguage: string option
      FirstName: string
      LastName: string
      MiddleName: string option
      Gender: string
      DateOfBirth: DateTime option }

type DependentDto =
    { DateOfDisability: DateTime option
      Id: int
      CoverageCoordination: CoverageCoordinationDto
      EligibilityPeriods: EligibilityPeriodDto list }

type SpouseDto =
    { Id: int
      EligibilityPeriods: EligibilityPeriodDto list
      CoverageCoordination: CoverageCoordinationDto }

type MemberDependentAddedDto =
    { IsEnrolled: bool
      BenefitPeriods: BenefitPeriodDto list
      PlanSelections: PlanSelectionDto list
      Dependent: DependentDto
      Client: Client }

type MemberSpouseAddedDto =
    { IsEnrolled: bool
      BenefitPeriods: BenefitPeriodDto list
      PlanSelections: PlanSelectionDto list
      Spouse: SpouseDto
      Client: Client }
