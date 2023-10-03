module Usage.Dto.Events.SpouseProfileUpdated

open System
open Usage.Dto.Events
open BenefitPeriod
open DependentAdded
open Update
open Employment

type SpouseProfileDto =
    { FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime
      Gender: string }

type SpouseProfileChangesDto =
    { Spouse: SpouseProfileDto
      Changes: UpdateDto list }

type SpouseProfileUpdatedDto =
    { MemberId: int
      EmployeeProfile: EmployeeProfileDto
      Employment: Employment option
      TaxProvince: string
      Dependents: DependentReferenceDto list
      BenefitPeriods: BenefitPeriodDto list
      IsEnrolled: bool
      Update: SpouseProfileChangesDto
      Spouse: SpouseDto }
