module Usage.Dto.Events.DependentDisabilityUpdated

open System

open BenefitPeriod
open CoverageCoordinationDto
open EligiblityPeriod
open DependentAdded
open Employment

type MemberDto =
    { EmployeeProfile: EmployeeProfileDto
      MemberId: int
      TaxProvince: string }

type DependentDto =
    { Id: int
      DateOfDisability: DateTime
      Disabled: bool
      FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime
      Gender: string
      EligibilityPeriods: EligibilityPeriodDto list
      CoverageCoordination: CoverageCoordinationDto }

type DependentDisabilityUpdatedDto =
    { ActiveBenefitPeriod: BenefitPeriodDto
      Dependent: DependentDto
      Member: MemberDto
      Employment: Employment option }
