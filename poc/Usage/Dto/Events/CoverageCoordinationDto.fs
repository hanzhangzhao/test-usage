module Usage.Dto.Events.CoverageCoordinationDto

open System

type CoverageCoordinationDto =
    { BridgeCoverage: bool
      Carrier: string option
      EffectiveDate: DateTime
      HasCoverage: bool
      Policy: string option
      ProvincialHealthCare: bool
      WaiveDental: bool
      WaiveHealth: bool }