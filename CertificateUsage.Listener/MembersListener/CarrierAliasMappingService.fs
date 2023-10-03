module CertificateUsage.Listener.MembersListener.CarrierAliasMappingService

open System.Threading.Tasks

open CertificateUsage.Dependencies
open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.BenefitPeriod

let updateCarrierInProductConfig (mapper : string -> Task<string option>) x =
    { x.ProductConfiguration with
        Carrier =
            (mapper x.ProductConfiguration.Carrier).Result
            |> Option.defaultValue x.ProductConfiguration.Carrier }

let updateCarrierNameInBenefitPeriod (mapper : string -> Task<string option>) bp =
    { bp with
        ProductConfiguration = updateCarrierInProductConfig mapper bp }

let updateCarrierNameInBenefitPeriods (mapper : string -> Task<string option>) =
    List.map (updateCarrierNameInBenefitPeriod mapper)

let updateCarrierName (root : Root.Root) (dto : MemberEventDto) =
    match dto with
    | MemberEnrollmentDto dto' ->
        { dto' with
            Carrier = (root.MapCarrierName dto'.Carrier).Result |> Option.defaultValue dto'.Carrier }
        |> MemberEnrollmentDto
    | MemberReinstatementConfirmed dto' ->
        { dto' with
            Carrier = (root.MapCarrierName dto'.Carrier).Result |> Option.defaultValue dto'.Carrier }
        |> MemberReinstatementConfirmed
    | MemberTerminatedDto dto' ->
        { dto' with
            Carrier = (root.MapCarrierName dto'.Carrier).Result |> Option.defaultValue dto'.Carrier }
        |> MemberTerminatedDto
    | MemberCancelledDto dto' ->
        { dto' with
            Carrier = (root.MapCarrierName dto'.Carrier).Result |> Option.defaultValue dto'.Carrier }
        |> MemberCancelledDto
    | DependentTerminatedDto dto' ->
        { dto' with
            BenefitPeriods = updateCarrierNameInBenefitPeriods root.MapCarrierName dto'.BenefitPeriods }
        |> DependentTerminatedDto
    | MemberTaxProvinceUpdatedDto dto' ->
        { dto' with
            ActiveBenefitPeriod = updateCarrierNameInBenefitPeriod root.MapCarrierName dto'.ActiveBenefitPeriod }
        |> MemberTaxProvinceUpdatedDto
    | MemberSpouseAddedDto dto' ->
        { dto' with
            BenefitPeriods = updateCarrierNameInBenefitPeriods root.MapCarrierName dto'.BenefitPeriods }
        |> MemberSpouseAddedDto
    | MemberDependentAddedDto dto' ->
        { dto' with
            BenefitPeriods = updateCarrierNameInBenefitPeriods root.MapCarrierName dto'.BenefitPeriods }
        |> MemberDependentAddedDto
    | SpouseTerminatedDto dto' ->
        { dto' with
            BenefitPeriods = updateCarrierNameInBenefitPeriods root.MapCarrierName dto'.BenefitPeriods }
        |> SpouseTerminatedDto
    | SpouseCohabUpdatedDto dto' ->
        { dto' with
            BenefitPeriods = updateCarrierNameInBenefitPeriods root.MapCarrierName dto'.BenefitPeriods }
        |> SpouseCohabUpdatedDto
    | MemberCobUpdatedDto dto' ->
        { dto' with
            BenefitPeriods = updateCarrierNameInBenefitPeriods root.MapCarrierName dto'.BenefitPeriods }
        |> MemberCobUpdatedDto
    | DependentCobUpdatedDto dto' ->
        { dto' with
            BenefitPeriods = updateCarrierNameInBenefitPeriods root.MapCarrierName dto'.BenefitPeriods }
        |> DependentCobUpdatedDto
    | SpouseCobUpdatedDto dto' ->
        { dto' with
            BenefitPeriods = updateCarrierNameInBenefitPeriods root.MapCarrierName dto'.BenefitPeriods }
        |> SpouseCobUpdatedDto
    | DependentPostSecondaryEducationUpdatedDto dto' ->
        { dto' with
            ActiveBenefitPeriod = updateCarrierNameInBenefitPeriod root.MapCarrierName dto'.ActiveBenefitPeriod }
        |> DependentPostSecondaryEducationUpdatedDto
    | DependentDisabilityUpdatedDto dto' ->
        { dto' with
            ActiveBenefitPeriod = updateCarrierNameInBenefitPeriod root.MapCarrierName dto'.ActiveBenefitPeriod }
        |> DependentDisabilityUpdatedDto
    | MemberEmploymentUpdatedDto dto' ->
        { dto' with
            ActiveBenefitPeriod = updateCarrierNameInBenefitPeriod root.MapCarrierName dto'.ActiveBenefitPeriod }
        |> MemberEmploymentUpdatedDto
    | MemberIncomeUpdatedDto dto' ->
        { dto' with
            ActiveBenefitPeriod = updateCarrierNameInBenefitPeriod root.MapCarrierName dto'.ActiveBenefitPeriod }
        |> MemberIncomeUpdatedDto
    | MemberEnrolledSnapshotDto dto' ->
        { dto' with
            Carrier = (root.MapCarrierName dto'.Carrier).Result |> Option.defaultValue dto'.Carrier }
        |> MemberEnrolledSnapshotDto
