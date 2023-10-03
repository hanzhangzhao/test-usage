module CertificateUsage.Dto.Events.BenefitPeriod

open System
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

let getActiveBenefitPeriod (periods: BenefitPeriodDto list) =
    periods |> List.find (fun b -> b.IsEnrolled)
