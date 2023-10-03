module CertificateUsage.Dto.Events.DependentProfileUpdated

open System
open CertificateUsage.Dto.Events
open BenefitPeriod
open DependentAdded
open Update
open Employment

type DependentProfile =
    { FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime
      Gender: string }

type DependentProfileChangesDto =
    { Dependent: DependentProfile
      Changes: UpdateDto list }

type DependentProfileUpdatedDto =
    { MemberId: int
      EmployeeProfile: EmployeeProfileDto
      Employment: Employment option
      TaxProvince: string
      Dependents: DependentReferenceDto list
      BenefitPeriods: BenefitPeriodDto list
      IsEnrolled: bool
      Update: DependentProfileChangesDto
      Dependent: DependentDto }
