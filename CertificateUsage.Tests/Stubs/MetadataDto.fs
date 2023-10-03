module CertificateUsage.Tests.Stubs.MetadataDto

open System

open CertificateUsage.Dto.Events.Metadata

let createDate = DateTime()

let dto =
    { EventId = Guid.Empty
      EventType = "EventType"
      Version = "Version"
      CreateDate = createDate
      EventNumber = 0UL
      StreamId = "StreamId" }
