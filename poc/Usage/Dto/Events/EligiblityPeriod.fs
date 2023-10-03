module Usage.Dto.Events.EligiblityPeriod

open System

type EligibilityPeriodDto =
    { EligibilityStartDate: DateTime
      EligibilityEndedDate: DateTime option
      Status: string
      EmployeeBenefitPeriodId: int }
    