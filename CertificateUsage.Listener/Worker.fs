namespace CertificateUsage.Listener

open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Serilog
open CertificateUsage.Dependencies

open CertificateUsage.EventStore
open CertificateUsage
open EventStore.Client

type Subscriber = SubscriptionHandler -> Task<unit>
type Handler = Root.Root -> PersistentSubscription -> ResolvedEvent -> Task<unit>

type Subscription =
    { Generator: unit -> Task<unit>
      Subscriber: Subscriber
      Handler: Handler }

type Worker(root: Root.Root) =
    inherit BackgroundService()

    let subscriptions =
        [ { Generator = root.EventStoreIO.CreateMembersSubscription
            Subscriber = root.EventStoreIO.SubscribeToMembersStream
            Handler = OnEvent.membersEventAppeared }
          { Generator = root.EventStoreIO.CreateRateSubscription
            Subscriber = root.EventStoreIO.SubscribeToRateStream
            Handler = RateListener.OnEvent.rateEventAppeared } ]

    let createSubscription (generator: unit -> Task<unit>) =
        task {
            return! generator ()
            Log.Logger.Information("Subscription exists or was created successfully")
        }

    let subscribe (subscriber: Subscriber) (handler: Handler) =
        task { return! subscriber (fun subscription event _ _ -> handler root subscription event) }

    override _.ExecuteAsync _ =
        Log.Logger.Information("Starting event listener")

        task {
            try
                let _ = subscriptions |> List.map (fun s -> createSubscription s.Generator)
                let _ = subscriptions |> List.map (fun s -> subscribe s.Subscriber s.Handler)

                return ()
            with ex ->
                Log.Error(ex, "Error creating subscription {@message}", ex.Message)
                Apm.``exception`` ex
        }
