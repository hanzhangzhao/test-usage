module CertificateUsage.Tests.Behaviour.Event

open System
open System.Text

open EventStore.Client

open CertificateUsage.Serialization
open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Tests.Behaviour.Config

type EventDataDto =
    | MemberEventDto of MemberEventDto
    | RateEventDto of RateEventDto

let connectionString = config["EventStore:ConnectionString"]

let eventStoreClient (connection : string option) =
    let conn = connection |> Option.defaultValue connectionString

    let settings = EventStoreClientSettings.Create(conn)
    new EventStoreClient(settings)

let toEventData = serializeSnakeCase >> Encoding.UTF8.GetBytes

let memberEventName (dto : MemberEventDto) : string =
    match dto with
    | MemberEnrollmentDto _ -> "MemberEnrollmentConfirmed"
    | MemberDependentAddedDto _ -> "MemberDependentAdded"
    | MemberSpouseAddedDto _ -> "MemberSpouseAdded"
    | MemberEmploymentUpdatedDto _ -> "EnrolledMemberEmploymentUpdated"
    | MemberIncomeUpdatedDto _ -> "EnrolledMemberIncomeUpdated"
    | MemberTerminatedDto _ -> "MemberBenefitEnded"
    | MemberCancelledDto _ -> "MemberBenefitCancelled"
    | SpouseTerminatedDto _ -> "EnrolledSpouseRemoved"
    | DependentTerminatedDto _ -> "EnrolledDependentRemoved"
    | MemberCobUpdatedDto _ -> "MemberCobUpdated"
    | DependentCobUpdatedDto _ -> "DependentCobUpdated"
    | SpouseCobUpdatedDto _ -> "SpouseCobUpdated"
    | MemberTaxProvinceUpdatedDto _ -> "EnrolledMemberTaxProvinceUpdated"
    | SpouseCohabUpdatedDto _ -> "MemberSpouseCohabUpdated"
    | DependentDisabilityUpdatedDto _ -> "EnrolledDependentDisabilityUpdated"
    | DependentPostSecondaryEducationUpdatedDto _ -> "EnrolledDependentPostSecondaryEducationUpdated"
    | MemberEnrolledSnapshotDto _ -> "MemberEnrolledSnapshot"
    | MemberReinstatementConfirmed _ -> "MemberReinstatementConfirmed"
    | MemberRateChangedDto _ -> "MemberRateChanged"

let memberDtoToEventData =
    function
    | MemberEnrollmentDto dto -> toEventData (box dto)
    | MemberDependentAddedDto dto -> toEventData (box dto)
    | MemberSpouseAddedDto dto -> toEventData (box dto)
    | MemberEmploymentUpdatedDto dto -> toEventData (box dto)
    | MemberIncomeUpdatedDto dto -> toEventData (box dto)
    | MemberTerminatedDto dto -> toEventData (box dto)
    | MemberCancelledDto dto -> toEventData (box dto)
    | SpouseTerminatedDto dto -> toEventData (box dto)
    | DependentTerminatedDto dto -> toEventData (box dto)
    | MemberCobUpdatedDto dto -> toEventData (box dto)
    | DependentCobUpdatedDto dto -> toEventData (box dto)
    | SpouseCobUpdatedDto dto -> toEventData (box dto)
    | MemberTaxProvinceUpdatedDto dto -> toEventData (box dto)
    | SpouseCohabUpdatedDto dto -> toEventData (box dto)
    | DependentDisabilityUpdatedDto dto -> toEventData (box dto)
    | DependentPostSecondaryEducationUpdatedDto dto -> toEventData (box dto)
    | MemberEnrolledSnapshotDto dto -> toEventData (box dto)
    | MemberReinstatementConfirmed dto -> toEventData (box dto)
    | MemberRateChangedDto dto -> toEventData (box dto)

let rateDtoToEventData =
    function
    | CarrierRateModifiedDto dto -> toEventData (box dto)

let rateEventName (dto : RateEventDto) : string =
    match dto with
    | CarrierRateModifiedDto _ -> "CarrierRateModified"

let eventName =
    function
    | MemberEventDto dto -> memberEventName dto
    | RateEventDto dto -> rateEventName dto

let dtoToEventData =
    function
    | MemberEventDto dto -> memberDtoToEventData dto
    | RateEventDto dto -> rateDtoToEventData dto

let createEvent ((data, metadata) : EventDataDto * EventMetadataDto) : EventData =
    EventData(
        eventId = Uuid.NewUuid(),
        ``type`` = eventName data,
        data = dtoToEventData data,
        metadata = Nullable(ReadOnlyMemory(toEventData (box metadata)))
    )

let appendEvent (streamName : string) (event : EventData) =
    (eventStoreClient None)
        .AppendToStreamAsync(streamName = streamName, expectedState = StreamState.Any, eventData = [ event ])

let emitEvent streamName = createEvent >> appendEvent streamName

let emitMemberEvent (data, metadata) =
    (MemberEventDto data, metadata) |> emitEvent "members"

let emitRateEvent (data, metadata) =
    (RateEventDto data, metadata) |> emitEvent "carrier_rates"
