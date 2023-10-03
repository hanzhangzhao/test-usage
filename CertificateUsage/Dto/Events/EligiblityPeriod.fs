module CertificateUsage.Dto.Events.EligiblityPeriod

open System

type EligibilityPeriodDto =
    { EligibilityStartDate: DateTime
      EmployeeBenefitPeriodId: int }
