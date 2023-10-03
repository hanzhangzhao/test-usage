module CertificateUsage.Tests.RateListner.ToDto

open System

open Xunit
open Expecto

open CertificateUsage.Tests.Stubs.Event
open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Dto.Events.CarrierRateModified
open CertificateUsage.Listener.RateListener.ToDto
open CertificateUsage.Tests

module ToMetadata =
    [<Fact>]
    let ``transforms event metadata to MetadataDto`` () =
        let event =
            createEvent
                "CarrierRateModified"
                Stubs.CarrierRateModifiedEvent.event
                Stubs.CarrierRateModifiedEvent.metadata

        let actual = toMetadata event

        let expected =
            { EventId = Guid.Empty
              EventType = "CarrierRateModified"
              Version = "Version"
              CreateDate = DateTime(2022, 2, 28)
              EventNumber = 0UL
              StreamId = "Stream" }
            |> Some

        Expect.equal actual expected ""

module ToDto =
    [<Fact>]
    let ``transforms a CarrierRateModified event to MemberEventDto`` () =
        let event =
            createEvent
                "CarrierRateModified"
                Stubs.CarrierRateModifiedEvent.event
                Stubs.CarrierRateModifiedEvent.metadata

        let actual = eventToDto event

        let expected =
            { CarrierRateModifiedDto.Carrier = "Carrier"
              PolicyNumber = "PolicyNumber"
              Option = "Option"
              Coverage = Some "Coverage"
              ProductLine = "ProductLine"
              EffectiveDate = DateTime(2023, 07, 20)
              CarrierRate = "1.23"
              ChangedBy = { Id = 2; Name = "ChangedBy.Name" }
              DcOption = None
              PricePer = 1 }
            |> CarrierRateModifiedDto
            |> Some

        Expect.equal actual expected ""
