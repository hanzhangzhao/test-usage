module CertificateUsage.Listener.MetadataDto
open System

open EventStore.Client

open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Dto.Mapping.JsonDeserialization


let eventToMetadata (e: ResolvedEvent) : MetadataDto =
    let eventMetadata: EventMetadataDto =
        e
        |> deserializeMetadata
        |> Option.defaultValue
            { Version = ""
              CreateDate = ((DateTimeOffset) e.Event.Created).ToUnixTimeSeconds() }

    { EventId = e.Event.EventId.ToGuid()
      EventType = e.Event.EventType
      Version = eventMetadata.Version
      CreateDate = DateTimeOffset.FromUnixTimeSeconds(eventMetadata.CreateDate).DateTime
      EventNumber = e.Event.EventNumber.ToUInt64()
      StreamId = e.Event.EventStreamId }
