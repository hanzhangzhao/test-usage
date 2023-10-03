module Usage.Dto.Events.SpouseTerminated

open System
open Usage.Dto.Events
open BenefitPeriod
open Employment
open MemberAddress
open CoverageCoordinationDto

type SpouseDto = DependentAdded.SpouseDto
type EmployeeProfileDto = DependentAdded.EmployeeProfileDto
type EligibilityPeriodDto = EligiblityPeriod.EligibilityPeriodDto
type PostSecondaryEducationPeriodDto = PostSecondaryEducationPeriodDto
type DependentReferenceDto = DependentAdded.DependentReferenceDto

type SpouseTerminatedDto =
    { MemberId: int
      EffectiveEndDate: DateTime
      IsEnrolled: bool
      TaxProvince: string
      EmployeeProfile: EmployeeProfileDto
      Employment: Employment option
      Dependents: DependentReferenceDto list
      BenefitPeriods: BenefitPeriodDto list
      Spouse: SpouseDto
      CoverageCoordination: CoverageCoordinationDto option
      MemberAddress: MemberAddress }
