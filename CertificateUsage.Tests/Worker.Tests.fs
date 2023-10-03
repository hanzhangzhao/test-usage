module CertificateUsage.Tests.Worker

open System
open CertificateUsage.Dependencies
open CertificateUsage.Dependencies.Root
open CertificateUsage.Tests.Stubs
open Xunit
open FsUnit
open Spy

open CertificateUsage.Listener

[<Fact>]
let ``should try to create a subscription`` () =
    let createSubSpy = Spy(fun _ -> Task.lift ())

    let root =
        { InsertCertificateUsage = fun _ -> Task.lift 1
          GetCertificate = fun _ -> Task.lift None
          PutCertificate = fun _ -> Task.lift 1
          EventStoreIO =
            { CreateMembersSubscription = createSubSpy.Function
              SubscribeToMembersStream = fun _ -> Task.lift ()
              CreateRateSubscription = fun _ -> Task.lift ()
              SubscribeToRateStream = fun _ -> Task.lift () }
          InsertRate = fun _ -> Task.lift 1
          InsertRetroactiveCertificateUpdate = fun _ -> Task.lift 1
          MapCarrierName = fun _ -> Task.lift None }


    let b = new Worker(root)

    task {
        let! _ = b.StartAsync(Threading.CancellationToken.None)

        createSubSpy.CalledOnceWith() |> should equal true
    }

[<Fact>]
let ``should catch any exception which is thrown and log it`` () =
    let createSubSpy = Spy(fun _ -> raise (exn "testing"))

    let root : Root.Root =
        { InsertCertificateUsage = fun _ -> Task.lift 1
          GetCertificate = fun _ -> Task.lift None
          PutCertificate = fun _ -> Task.lift 1
          EventStoreIO =
            { CreateMembersSubscription = createSubSpy.Function
              SubscribeToMembersStream = fun _ -> Task.lift ()
              CreateRateSubscription = fun _ -> Task.lift ()
              SubscribeToRateStream = fun _ -> Task.lift () }
          InsertRate = fun _ -> Task.lift 1
          InsertRetroactiveCertificateUpdate = fun _ -> Task.lift 1
          MapCarrierName = fun _ -> Task.lift None }

    let b = new Worker(root)

    task {
        let! _ = b.StartAsync(Threading.CancellationToken.None)

        createSubSpy.CalledOnceWith() |> should equal true
    }
