module Usage.Dto.Events.MemberCobUpdated

open Usage.Dto.Events
open DependentAdded
open CoverageCoordinationDto
open BenefitPeriod
open Employment

type MemberCobUpdatedDto =
    { MemberId: int
      EmployeeProfile: EmployeeProfileDto
      Employment: Employment option
      TaxProvince: string
      Dependents: DependentReferenceDto list
      BenefitPeriods: BenefitPeriodDto list
      IsEnrolled: bool
      CoverageCoordination: CoverageCoordinationDto }

type DependentCobUpdatedDto =
    { MemberId: int
      EmployeeProfile: EmployeeProfileDto
      Employment: Employment option
      TaxProvince: string
      Dependents: DependentReferenceDto list
      BenefitPeriods: BenefitPeriodDto list
      IsEnrolled: bool
      Dependent: DependentDto }

type SpouseCobUpdatedDto =
    { MemberId: int
      IsEnrolled: bool
      EmployeeProfile: EmployeeProfileDto
      Employment: Employment option
      Spouse: SpouseDto
      BenefitPeriods: BenefitPeriodDto list
      Dependents: DependentReferenceDto list }
