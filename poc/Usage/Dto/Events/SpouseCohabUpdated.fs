module Usage.Dto.Events.SpouseCohabUpdated

open Usage.Dto.Events
open BenefitPeriod
open CoverageCoordinationDto
open EligiblityPeriod
open DependentAdded
open Employment
open System
open MemberAddress

type EmployeeProfileDto = DependentAdded.EmployeeProfileDto

type Spouse =
    { Id: int
      ProfileId: int
      FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime
      Gender: string
      CommonLaw: bool option
      CohabitationDate: DateTime option
      CoverageCoordination: CoverageCoordinationDto
      EmployeeEligibilityPeriods: EligibilityPeriodDto list }

type SpouseCohabUpdatedDto =
    { MemberId: int
      EmployeeProfile: EmployeeProfileDto
      TaxProvince: string
      MemberAddress: MemberAddress
      Employment: Employment option
      Dependents: DependentReferenceDto list
      BenefitPeriods: BenefitPeriodDto list
      IsEnrolled: bool
      Spouse: Spouse }
    