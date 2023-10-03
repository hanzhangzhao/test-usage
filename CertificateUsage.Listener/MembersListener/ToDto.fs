module CertificateUsage.Listener.ToDto

open EventStore.Client

open Serilog

open CertificateUsage.Dto.Events.DependentAdded
open CertificateUsage.Dto.Events.MemberReinstatementConfirmed
open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Dto.Mapping.JsonDeserialization
open CertificateUsage.Dto.Events.MemberEnrolled
open CertificateUsage.Dto.Events.MemberCancelled
open CertificateUsage.Dto.Events.MemberTerminated
open CertificateUsage.Dto.Events.MemberEnrolledSnapshot
open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.MemberTaxProvinceUpdate
open CertificateUsage.Dto.Events.SpouseTerminated
open CertificateUsage.Dto.Events.SpouseCohabUpdated
open CertificateUsage.Dto.Events.MemberCobUpdated
open CertificateUsage.Dto.Events.DependentTerminated
open CertificateUsage.Dto.Events.DependentPostSecondaryEducationUpdated
open CertificateUsage.Dto.Events.DependentDisabilityUpdated
open CertificateUsage.Dto.Events.MemberEmploymentUpdate
open CertificateUsage.Dto.Events.MemberIncomeUpdate
open CertificateUsage.Dto.Events.MemberRateChanged
open CertificateUsage.Listener.MetadataDto

let toMetadata (e : ResolvedEvent) : MetadataDto option =
    match e.Event.EventType with
    | "MemberEnrollmentConfirmed" -> e |> eventToMetadata |> Some
    | "MemberReinstatementConfirmed" -> e |> eventToMetadata |> Some
    | "MemberBenefitEnded" -> e |> eventToMetadata |> Some
    | "MemberBenefitCancelled" -> e |> eventToMetadata |> Some
    | "MemberEnrolledSnapshot" -> e |> eventToMetadata |> Some
    | "EnrolledMemberTaxProvinceUpdated" -> e |> eventToMetadata |> Some
    | "MemberDependentAdded" -> e |> eventToMetadata |> Some
    | "MemberSpouseAdded" -> e |> eventToMetadata |> Some
    | "EnrolledDependentRemoved" -> e |> eventToMetadata |> Some
    | "EnrolledSpouseRemoved" -> e |> eventToMetadata |> Some
    | "MemberCobUpdated" -> e |> eventToMetadata |> Some
    | "DependentCobUpdated" -> e |> eventToMetadata |> Some
    | "SpouseCobUpdated" -> e |> eventToMetadata |> Some
    | "MemberSpouseCohabUpdated" -> e |> eventToMetadata |> Some
    | "EnrolledDependentDisabilityUpdated" -> e |> eventToMetadata |> Some
    | "EnrolledDependentPostSecondaryEducationUpdated" -> e |> eventToMetadata |> Some
    | "EnrolledMemberIncomeUpdated" -> e |> eventToMetadata |> Some
    | "EnrolledMemberEmploymentUpdated" -> e |> eventToMetadata |> Some
    | "MemberRateChanged" -> e |> eventToMetadata |> Some
    | eventType ->
        Log.Warning("Unrecognized event type {@eventType}", eventType)
        None

let eventToDto (e : ResolvedEvent) : MemberEventDto option =
    match e.Event.EventType with
    | "MemberEnrollmentConfirmed" -> e |> deserialize<MemberEnrollmentDto> |> Option.map MemberEnrollmentDto
    | "MemberReinstatementConfirmed" ->
        e
        |> deserialize<MemberReinstatementConfirmedDto>
        |> Option.map MemberReinstatementConfirmed
    | "MemberBenefitEnded" -> e |> deserialize<MemberTerminatedDto> |> Option.map MemberTerminatedDto
    | "MemberBenefitCancelled" -> e |> deserialize<MemberCancelledDto> |> Option.map MemberCancelledDto
    | "MemberEnrolledSnapshot" ->
        e
        |> deserialize<MemberEnrolledSnapshotDto>
        |> Option.map MemberEnrolledSnapshotDto
    | "EnrolledMemberTaxProvinceUpdated" ->
        e
        |> deserialize<MemberTaxProvinceUpdatedDto>
        |> Option.map MemberTaxProvinceUpdatedDto
    | "MemberSpouseAdded" ->
        e
        |> deserialize<MemberSpouseAddedDto>
        |> Option.filter (fun dto -> dto.IsEnrolled)
        |> Option.map MemberSpouseAddedDto
    | "MemberDependentAdded" ->
        e
        |> deserialize<MemberDependentAddedDto>
        |> Option.filter (fun dto -> dto.IsEnrolled)
        |> Option.map MemberDependentAddedDto
    | "EnrolledDependentRemoved" ->
        e
        |> deserialize<DependentTerminatedDto>
        |> Option.filter (fun dto -> dto.IsEnrolled)
        |> Option.map DependentTerminatedDto
    | "EnrolledSpouseRemoved" ->
        e
        |> deserialize<SpouseTerminatedDto>
        |> Option.filter (fun dto -> dto.IsEnrolled)
        |> Option.map SpouseTerminatedDto
    | "MemberCobUpdated" ->
        e
        |> deserialize<MemberCobUpdatedDto>
        |> Option.filter (fun dto -> dto.IsEnrolled)
        |> Option.map MemberCobUpdatedDto
    | "DependentCobUpdated" ->
        e
        |> deserialize<DependentCobUpdatedDto>
        |> Option.filter (fun dto -> dto.IsEnrolled)
        |> Option.map DependentCobUpdatedDto
    | "SpouseCobUpdated" ->
        e
        |> deserialize<SpouseCobUpdatedDto>
        |> Option.filter (fun dto -> dto.IsEnrolled)
        |> Option.map SpouseCobUpdatedDto
    | "MemberSpouseCohabUpdated" ->
        e
        |> deserialize<SpouseCohabUpdatedDto>
        |> Option.filter (fun dto -> dto.IsEnrolled)
        |> Option.map SpouseCohabUpdatedDto
    | "EnrolledDependentDisabilityUpdated" ->
        e
        |> deserialize<DependentDisabilityUpdatedDto>
        |> Option.map DependentDisabilityUpdatedDto
    | "EnrolledDependentPostSecondaryEducationUpdated" ->
        e
        |> deserialize<DependentPostSecondaryEducationUpdatedDto>
        |> Option.map DependentPostSecondaryEducationUpdatedDto
    | "EnrolledMemberIncomeUpdated" -> e |> deserialize<MemberIncomeUpdatedDto> |> Option.map MemberIncomeUpdatedDto
    | "EnrolledMemberEmploymentUpdated" ->
        e
        |> deserialize<MemberEmploymentUpdatedDto>
        |> Option.map MemberEmploymentUpdatedDto
    | "MemberRateChanged" -> e |> deserialize<MemberRateChangedDto> |> Option.map MemberRateChangedDto
    | eventType ->
        Log.Warning("Unrecognized event type {@eventType}", eventType)
        None

// TODO: properly handle different versions of the events. https://sterlingcb.atlassian.net/browse/PLT-780
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
let toDto e = (eventToDto e, toMetadata e)
