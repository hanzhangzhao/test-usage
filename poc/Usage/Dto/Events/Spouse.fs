module Usage.Dto.Events.Spouse

open System

open Usage.Dto.Events.CoverageCoordinationDto
open Usage.Dto.Events.Dependent

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
      EligibilityPeriods: DependentEligibilityPeriod list }
