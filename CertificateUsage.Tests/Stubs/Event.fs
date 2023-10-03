module CertificateUsage.Tests.Stubs.Event

open System
open System.Collections.Generic
open System.Text
open EventStore.Client

let createEvent eventType (data: string) (metadata: string) =
    ResolvedEvent(
        EventRecord(
            "Stream",
            Uuid.Empty,
            StreamPosition(0UL),
            Position(),
            dict
                [ "created", DateTime(2022, 2, 28).Ticks |> string
                  "type", eventType
                  "content-type", "application/json"
                  "version", "1.0.0" ]
            |> Dictionary,
            ReadOnlyMemory(Encoding.UTF8.GetBytes(data)),
            ReadOnlyMemory(Encoding.UTF8.GetBytes(metadata))
        ),
        null,
        Nullable 0UL
    )
