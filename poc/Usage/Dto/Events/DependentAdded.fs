module Usage.Dto.Events.DependentAdded

open System
open Usage.Dto.Events
open BenefitPeriod
open CoverageCoordinationDto
open EligiblityPeriod
open PostSecondaryEducationPeriod
open Employment

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
      Disabled: bool option
      ReferenceNumber: int
      FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime
      Gender: string
      Id: int
      PostSecondaryEducationPeriods: PostSecondaryEducationPeriodDto list
      EligibilityPeriods: EligibilityPeriodDto list
      CoverageCoordination: CoverageCoordinationDto }
    
type SpouseDto =
    { FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime
      Gender: string
      Id: int
      CohabitationDate: DateTime option
      CommonLaw: bool option
      EligibilityPeriods: EligibilityPeriodDto list
      CoverageCoordination: CoverageCoordinationDto }

type MemberDependentAddedDto =
    {  MemberId: int
       IsEnrolled: bool
       TaxProvince: string
       EmployeeProfile: EmployeeProfileDto
       Employment: Employment option
       Dependents: DependentReferenceDto list
       BenefitPeriods: BenefitPeriodDto list
       Dependent: DependentDto
       MemberAddress: MemberAddress.MemberAddress
       CoverageCoordination: CoverageCoordinationDto option }
    
type MemberSpouseAddedDto =
    { MemberId: int
      IsEnrolled: bool
      TaxProvince: string
      EmployeeProfile: EmployeeProfileDto
      Employment: Employment option
      Dependents: DependentReferenceDto list
      BenefitPeriods: BenefitPeriodDto list 
      Spouse: SpouseDto
      MemberAddress: MemberAddress.MemberAddress
      CoverageCoordination: CoverageCoordinationDto option}
