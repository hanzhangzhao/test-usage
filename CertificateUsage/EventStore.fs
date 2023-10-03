module CertificateUsage.EventStore

open System
open EventStore.Client
open Serilog

open System.Threading
open System.Threading.Tasks
open System.Diagnostics.CodeAnalysis

type SubscriptionHandler = PersistentSubscription -> ResolvedEvent -> Nullable<int> -> CancellationToken -> Task

[<ExcludeFromCodeCoverage(Justification = "integration")>]
let rec createSubscription (conn: string) (streamName: string) (groupName: string) =
    let clientSettings = EventStoreClientSettings.Create(conn)
    let client = new EventStorePersistentSubscriptionsClient(clientSettings)
    task {
        try
            do! client.CreateToStreamAsync(
                streamName = streamName,
                groupName = groupName,
                settings = PersistentSubscriptionSettings(startFrom = StreamPosition.Start))
            Log.Logger.Information("Subscription Created {@streamName} {@groupName}", streamName, groupName)
        with
        | :? Grpc.Core.RpcException as error ->
            if error.Message.Contains("AlreadyExists") then
                Log.Logger.Information(
                    "Subscription already exists {@streamName} {@groupName}", streamName, groupName)

        | :? InvalidOperationException as error ->
            if error.Message.Contains("AlreadyExists") then
                Log.Logger.Information(
                    "Subscription already exists {@streamName} {@groupName}", streamName, groupName)
            else
                Log.Logger.Fatal("Failed to create new subscription {@error}", error)
                raise error
        | :? NotLeaderException as error ->
            do! createSubscription $"esdb://{error.LeaderEndpoint.Host}:{error.LeaderEndpoint.Port}?tls=false" streamName groupName
        | error ->
            Log.Fatal("Failed to create new subscription {@error}", error)
            raise error
    }

[<ExcludeFromCodeCoverage(Justification = "integration")>]
let subscribe (conn: string) (streamName: string) (groupName: string) (handler: SubscriptionHandler) =
    let clientSettings = EventStoreClientSettings.Create(conn)
    let client = new EventStorePersistentSubscriptionsClient(clientSettings)
    task {
        try
            let! _ = client.SubscribeToStreamAsync(
                streamName = streamName,
                groupName = groupName,
                eventAppeared = handler)
            Log.Logger.Information("Listening {@streamName} {@groupName}", streamName, groupName)
            return ()
        with
        | :? Grpc.Core.RpcException as error ->
            if error.Message.Contains("AlreadyExists") then
                Log.Logger.Information(
                    "Subscription already exists {@streamName} {@groupName}", streamName, groupName)

        | :? InvalidOperationException as error ->
            if error.Message.Contains("AlreadyExists") then
                Log.Logger.Information(
                    "Subscription already exists {@streamName} {@groupName}", streamName, groupName)
            else
                Log.Logger.Fatal("Failed to create new subscription {@error}", error)
                raise error
        | :? NotLeaderException as error ->
            do! createSubscription $"esdb://{error.LeaderEndpoint.Host}:{error.LeaderEndpoint.Port}?tls=false" streamName groupName
        | error ->
            Log.Fatal("Failed to create new subscription {@error}", error)
            raise error
    }
