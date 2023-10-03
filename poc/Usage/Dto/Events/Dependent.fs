module Usage.Dto.Events.Dependent

open System

open Usage.Dto.Events.CoverageCoordinationDto

type DependentEligibilityPeriod =
    { Id: int
      BenefitPeriodId: int
      Status: string
      StartDate: DateTime
      EndDate: DateTime option }

type DependentPostSecondary =
    { Id: int
      StartDate: DateTime
      EndDate: DateTime
      ProgramTitle: string option
      InstitutionName: string option }

type Dependent =
    { Id: int
      ProfileId: int
      FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime
      Gender: string
      PostSecondaryEducation: DependentPostSecondary list
      EligibilityPeriods: DependentEligibilityPeriod list
      CoverageCoordination: CoverageCoordinationDto }
