module Usage.Dto.ToDto

open EventStore.Client
open Feeds
open Feeds.Dto.Models
open Serilog

open Usage.Dto.Events
open Dto
open BenefitPeriod
open DependentAdded
open DependentProfileUpdated
open MemberCancelled
open MemberEmploymentUpdate
open MemberEnrolled
open MemberTaxProvinceUpdate
open MemberTerminated
open DependentTerminated
open SpouseProfileUpdated
open SpouseTerminated
open MemberCobUpdated
open MemberIncomeUpdate
open MemberProfileUpdate
open SpouseCohabUpdated
open DependentDisabilityUpdated
open DependentPostSecondaryEducationUpdated
open MemberReinstatementConfirmed
open MemberBenefitClassTransferred

open Usage.Dto.Mapping.JsonDeserialization

let toDto (e: ResolvedEvent) : MemberEventDto option =
    match e.Event.EventType with
    | "MemberEnrollmentConfirmed" ->
        e
        |> deserialize<MemberEnrollmentDto>
        |> Option.map MemberEnrollmentDto
    | "MemberDependentAdded" ->
        e
        |> deserialize<MemberDependentAddedDto>
        |> Option.bind (fun x -> if x.IsEnrolled then (Some x) else None)
        |> Option.map MemberDependentAddedDto
    | "MemberSpouseAdded" ->
        e
        |> deserialize<MemberSpouseAddedDto>
        |> Option.filter (fun x -> x.IsEnrolled)
        |> Option.map MemberSpouseAddedDto
    | "EnrolledMemberIncomeUpdated" ->
        e
        |> deserialize<MemberIncomeUpdatedDto>
        |> Option.map MemberIncomeUpdatedDto
    | "EnrolledMemberEmploymentUpdated" ->
        e
        |> deserialize<MemberEmploymentUpdatedDto>
        |> Option.map MemberEmploymentUpdatedDto
    | "MemberBenefitEnded" ->
        e
        |> deserialize<MemberTerminatedDto>
        |> Option.map MemberTerminatedDto
    | "MemberBenefitCancelled" ->
        e
        |> deserialize<MemberCancelledDto>
        |> Option.map MemberCancelledDto
    | "EnrolledDependentRemoved" ->
        e
        |> deserialize<DependentTerminatedDto>
        |> Option.map DependentTerminatedDto
    | "EnrolledSpouseRemoved" ->
        e
        |> deserialize<SpouseTerminatedDto>
        |> Option.map SpouseTerminatedDto
    | "MemberCobUpdated" ->
        e
        |> deserialize<MemberCobUpdatedDto>
        |> Option.map MemberCobUpdatedDto
    | "DependentCobUpdated" ->
        e
        |> deserialize<DependentCobUpdatedDto>
        |> Option.map DependentCobUpdatedDto
    | "SpouseCobUpdated" ->
        e
        |> deserialize<SpouseCobUpdatedDto>
        |> Option.map SpouseCobUpdatedDto
    | "EnrolledMemberUpdated" ->
        e
        |> deserialize<MemberProfileUpdatedDto>
        |> Option.map MemberProfileUpdatedDto
    | "EnrolledMemberTaxProvinceUpdated" ->
        e
        |> deserialize<MemberTaxProvinceUpdatedDto>
        |> Option.map MemberTaxProvinceUpdatedDto
    | "SpouseProfileUpdated" ->
        e
        |> deserialize<SpouseProfileUpdatedDto>
        |> Option.map SpouseProfileUpdatedDto
    | "DependentProfileUpdated" ->
        e
        |> deserialize<DependentProfileUpdatedDto>
        |> Option.map DependentProfileUpdatedDto
    | "MemberSpouseCohabUpdated" ->
        e
        |> deserialize<SpouseCohabUpdatedDto>
        |> Option.map SpouseCohabUpdatedDto
    | "EnrolledDependentDisabilityUpdated" ->
        e
        |> deserialize<DependentDisabilityUpdatedDto>
        |> Option.map DependentDisabilityUpdatedDto
    | "EnrolledDependentPostSecondaryEducationUpdated" ->
        e
        |> deserialize<DependentPostSecondaryEducationUpdatedDto>
        |> Option.map DependentPostSecondaryEducationUpdatedDto
    | "MemberBenefitClassTransferred" ->
        e
        |> deserialize<MemberBenefitClassTransferredDto>
        |> Option.map MemberBenefitClassTransferredDto
    | "MemberReinstatementConfirmed" ->
        e
        |> deserialize<MemberReinstatementConfirmedDto>
        |> Option.map MemberReinstatementConfirmedDto
    | eventType ->
        Log.Warning("Unrecognized event type {@eventType}", eventType)
        None

let isAllowedCarrier (carriers: string list) =
    let matchingBenefitPeriod (bp: BenefitPeriodDto list) (carriers: string list) =
        bp
        |> List.tryFind (fun bp -> bp.IsEnrolled)
        |> Option.map (fun bp -> List.contains bp.ProductConfiguration.Carrier carriers)
        |> Option.defaultValue false

    function
    | MemberEnrollmentDto dto -> carriers |> List.contains dto.Carrier
    | MemberSpouseAddedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | MemberDependentAddedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | MemberEmploymentUpdatedDto dto ->
        carriers
        |> List.contains dto.ActiveBenefitPeriod.ProductConfiguration.Carrier
    | MemberIncomeUpdatedDto dto ->
        carriers
        |> List.contains dto.ActiveBenefitPeriod.ProductConfiguration.Carrier
    | MemberTerminatedDto dto -> carriers |> List.contains dto.Carrier
    | MemberCancelledDto dto -> carriers |> List.contains dto.Carrier
    | DependentTerminatedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | SpouseTerminatedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | MemberCobUpdatedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | DependentCobUpdatedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | SpouseCobUpdatedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | MemberProfileUpdatedDto dto ->
        carriers
        |> List.contains dto.ActiveBenefitPeriod.ProductConfiguration.Carrier
    | MemberTaxProvinceUpdatedDto dto ->
        carriers
        |> List.contains dto.ActiveBenefitPeriod.ProductConfiguration.Carrier
    | DependentProfileUpdatedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | SpouseProfileUpdatedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | SpouseCohabUpdatedDto dto ->
        carriers
        |> matchingBenefitPeriod dto.BenefitPeriods
    | DependentDisabilityUpdatedDto dto ->
        carriers
        |> List.contains dto.ActiveBenefitPeriod.ProductConfiguration.Carrier
    | DependentPostSecondaryEducationUpdatedDto dto ->
        carriers
        |> List.contains dto.ActiveBenefitPeriod.ProductConfiguration.Carrier
    | MemberBenefitClassTransferredDto dto ->
        carriers
        |> List.contains dto.To.ProductConfiguration.Carrier
    | MemberReinstatementConfirmedDto dto -> carriers |> List.contains dto.Carrier