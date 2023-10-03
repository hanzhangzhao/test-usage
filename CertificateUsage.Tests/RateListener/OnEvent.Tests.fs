module CertificateUsage.RateListener.Tests.OnEvent

open System

open CertificateUsage
open CertificateUsage.Listener.RateListener
open CertificateUsage.Dependencies
open CertificateUsage.Tests.Stubs
open CertificateUsage.Tests.Spy

open Xunit
open FsUnit

[<Fact>]
let ``Should handle an event`` () =
    let spy = Spy(fun _ -> Task.lift 1)

    let root : Root.Root =
        { InsertCertificateUsage = fun _ -> Task.lift 1
          GetCertificate = fun _ -> Task.lift None
          PutCertificate = fun _ -> Task.lift 1
          EventStoreIO =
            { CreateMembersSubscription = fun _ -> Task.lift ()
              SubscribeToMembersStream = fun _ -> Task.lift ()
              CreateRateSubscription = fun _ -> Task.lift ()
              SubscribeToRateStream = fun _ -> Task.lift () }
          InsertRate = spy.Function
          InsertRetroactiveCertificateUpdate = fun _ -> Task.lift 1
          MapCarrierName = fun _ -> Task.lift None }

    let rateModified = CarrierRateModifiedDto.dto |> Serialization.serializeSnakeCase

    let metaData =
        {| Version = "1.0.0"
           CreateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds() |}
        |> Serialization.serializeSnakeCase

    let event = Event.createEvent "CarrierRateModified" rateModified metaData

    task {
        let! _ = OnEvent.eventAppeared root event
        spy.CalledOnce |> should equal true
    }
