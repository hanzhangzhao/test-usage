module CertificateUsage.Dto.Events.PostSecondaryEducationPeriod

open System

type PostSecondaryEducationPeriodDto =
    { StartDate: DateTime
      EndDate: DateTime
      InstitutionName: string option
      ProgramTitle: string option }
