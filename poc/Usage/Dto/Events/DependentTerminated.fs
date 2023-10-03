module Usage.Dto.Events.DependentTerminated

open System

open Usage.Dto.Events
open BenefitPeriod
open Employment
open MemberAddress
open CoverageCoordinationDto

type DependentDto = DependentAdded.DependentDto
type EmployeeProfileDto = DependentAdded.EmployeeProfileDto
type EligibilityPeriodDto = EligiblityPeriod.EligibilityPeriodDto
type PostSecondaryEducationPeriodDto = PostSecondaryEducationPeriodDto
type DependentReferenceDto = DependentAdded.DependentReferenceDto

type DependentTerminatedDto =
    { MemberId: int
      IsEnrolled: bool
      EffectiveEndDate: DateTime
      TaxProvince: string
      EmployeeProfile: EmployeeProfileDto
      Employment: Employment option
      Dependents: DependentReferenceDto list
      BenefitPeriods: BenefitPeriodDto list
      Dependent: DependentDto
      CoverageCoordination: CoverageCoordinationDto option
      MemberAddress: MemberAddress }
