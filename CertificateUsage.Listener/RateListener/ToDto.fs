module CertificateUsage.Listener.RateListener.ToDto

open Serilog

open EventStore.Client

open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Dto.Events.CarrierRateModified
open CertificateUsage.Dto.Mapping.JsonDeserialization

open CertificateUsage.Listener.MetadataDto

let toMetadata (e: ResolvedEvent) : MetadataDto option =
    match e.Event.EventType with
    | "CarrierRateModified" -> e |> eventToMetadata |> Some
    | eventType ->
        Log.Warning("Unrecognized event type {@eventType}", eventType)
        None

let eventToDto (e: ResolvedEvent) : RateEventDto option =
    match e.Event.EventType with
    | "CarrierRateModified" -> e |> deserialize<CarrierRateModifiedDto> |> Option.map CarrierRateModifiedDto
    | eventType ->
        Log.Warning("Unrecognized event type {@eventType}", eventType)
        None

// TODO: properly handle different versions of the events. https://sterlingcb.atlassian.net/browse/PLT-780
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
let toDto e = (eventToDto e, toMetadata e)
