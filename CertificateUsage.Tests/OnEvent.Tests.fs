module CertificateUsage.Tests.OnEvent

open System

open Xunit
open FsUnit

open CertificateUsage
open CertificateUsage.Listener
open CertificateUsage.Dependencies
open CertificateUsage.Tests.Stubs
open CertificateUsage.Tests.Spy

[<Fact>]
let ``Should handle an event`` () =
    let spy = Spy(fun _ -> Task.lift 1)

    let root : Root.Root =
        { InsertCertificateUsage = spy.Function
          GetCertificate = fun _ -> Task.lift None
          PutCertificate = fun _ -> Task.lift 1
          EventStoreIO =
            { CreateMembersSubscription = fun _ -> Task.lift ()
              SubscribeToMembersStream = fun _ -> Task.lift ()
              CreateRateSubscription = fun _ -> Task.lift ()
              SubscribeToRateStream = fun _ -> Task.lift () }
          InsertRate = fun _ -> Task.lift 1
          InsertRetroactiveCertificateUpdate = fun _ -> Task.lift 1
          MapCarrierName = fun _ -> Task.lift None }

    let terminated = MemberTerminatedDto.dto |> Serialization.serializeSnakeCase

    let metaData =
        {| Version = "1.0.0"
           CreateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds() |}
        |> Serialization.serializeSnakeCase

    let event = Event.createEvent "MemberBenefitEnded" terminated metaData

    task {
        let! _ = OnEvent.eventAppeared root event
        spy.CalledOnce |> should equal true
    }

[<Fact>]
let ``Should handle a member event`` () =
    let spy = Spy(fun _ -> Task.lift 1)

    let root : Root.Root =
        { InsertCertificateUsage = spy.Function
          GetCertificate = fun _ -> Task.lift None
          PutCertificate = fun _ -> Task.lift 1
          EventStoreIO =
            { CreateMembersSubscription = fun _ -> Task.lift ()
              SubscribeToMembersStream = fun _ -> Task.lift ()
              CreateRateSubscription = fun _ -> Task.lift ()
              SubscribeToRateStream = fun _ -> Task.lift () }
          InsertRate = fun _ -> Task.lift 1
          InsertRetroactiveCertificateUpdate = fun _ -> Task.lift 1
          MapCarrierName = fun _ -> Task.lift None }

    let terminated = MemberTerminatedDto.dto |> Serialization.serializeSnakeCase

    let metaData =
        {| Version = "1.0.0"
           CreateDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds() |}
        |> Serialization.serializeSnakeCase

    let event = Event.createEvent "MemberBenefitEnded" terminated metaData

    task {
        let! _ = OnEvent.eventAppeared root event
        spy.CalledOnce |> should equal true
    }
