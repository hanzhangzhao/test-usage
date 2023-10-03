module Usage.Dto.Events.DependentPostSecondaryEducationUpdated

open System

open BenefitPeriod
open CoverageCoordinationDto
open EligiblityPeriod
open DependentAdded
open Employment

type MemberDto =
    { EmployeeProfile: EmployeeProfileDto
      MemberId: int
      TaxProvince: string
      Employment: Employment option
      BenefitPeriods: BenefitPeriodDto list
      Dependents: DependentReferenceDto list }

type DependentDto =
    { Id: int
      FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime
      Gender: string
      EligibilityPeriods: EligibilityPeriodDto list
      CoverageCoordination: CoverageCoordinationDto }

type DependentPostSecondaryEducationUpdatedDto =
    { ActiveBenefitPeriod: BenefitPeriodDto
      Dependent: DependentDto
      Member: MemberDto
      PreviousEligibilityPeriods: EligibilityPeriodDto list option }
