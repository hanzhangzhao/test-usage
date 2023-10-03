module Usage.Dto.Events.BenefitPeriod

open System
open Usage.Dto.Events
open ProductConfiguration
open Certificate

type Coverages = Map<string, string>
type CarrierMapping = Map<string, string>

type BenefitPeriodDto =
    { Id: int
      IsEnrolled: bool
      ProductConfiguration: ProductConfigurationDto
      Certificate: Certificate
      CarrierMapping: CarrierMapping option
      BenefitsStartDate: DateTime option
      BenefitsEndedDate: DateTime option
      Coverages: Coverages option
      EnrollmentDate: DateTime option }


let getOptionalActiveBenefitPeriod (periods: BenefitPeriodDto list) =
    periods |> List.tryFind (fun b -> b.IsEnrolled)

let getInactiveBenefitPeriods (periods: BenefitPeriodDto list) =
    periods |> List.filter (fun b -> not b.IsEnrolled)
