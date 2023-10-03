module CertificateUsage.Dto.Mapping.JsonDeserialization

open EventStore.Client
open JorgeSerrano.Json
open Serilog
open System.Text.Json
open CertificateUsage.Dto.Events.Metadata

let jsonSnakeCase =
    JsonSerializerOptions(PropertyNamingPolicy = JsonSnakeCaseNamingPolicy())

let deserializeMetadata (e: ResolvedEvent) =
    try
        Some(JsonSerializer.Deserialize<EventMetadataDto>(utf8Json = e.Event.Metadata.Span, options = jsonSnakeCase))
    with ex ->
        Log.Error(
            ex,
            "Failed to deserialize metadata for {@eventType} event {@eventId} {@ex}",
            e.Event.EventType,
            e.Event.EventNumber.ToString()
        )

        None

let deserialize<'dto> (e: ResolvedEvent) =
    try
        Some(JsonSerializer.Deserialize<'dto>(utf8Json = e.Event.Data.Span, options = jsonSnakeCase))
    with ex ->
        Log.Error(
            ex,
            "Failed to deserialize {@eventType} event {@eventId} {@ex}",
            e.Event.EventType,
            e.Event.EventNumber.ToString()
        )

        None
