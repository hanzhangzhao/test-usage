module CertificateUsage.Dto.Events.Metadata

open System

type EventMetadataDto =
    { Version: string
      CreateDate: int64 }
    
type MetadataDto =
    { EventId: Guid
      EventType: string
      Version: string
      CreateDate: DateTime
      EventNumber: uint64
      StreamId: string }