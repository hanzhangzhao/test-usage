module CertificateUsage.Tests.JsonDeserialization_Tests

open System

open Xunit
open Expecto

open CertificateUsage.Dto.Mapping.JsonDeserialization
open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Tests

module DeserializeMetadata =
    [<Fact>]
    let ``it deserializes metadata`` () =
        let createDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        let metaData = $"{{\"version\": \"1.0.0\",  \"create_date\": {createDate} }}"

        let event = Stubs.Event.createEvent "type" "data" metaData
        let actual = deserializeMetadata event

        let expected =
            { EventMetadataDto.Version = "1.0.0"
              CreateDate = createDate }
            |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``it gracefully fails (return None) when failing to deserialize metadata`` () =
        let metaData = "bad"

        let event = Stubs.Event.createEvent "type" "data" metaData
        let actual = deserializeMetadata event
        let expected = None

        Expect.equal actual expected "should equal"
