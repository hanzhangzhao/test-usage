module Usage.EventStore

open System
open FSharp.Control
open EventStore.Client

type Metadata = { Version: string }

let readStream (conn: string) (streamId: string) =
    let settings = EventStoreClientSettings.Create(conn)
    let client = new EventStoreClient(settings)
    
    client.ReadStreamAsync(direction = Direction.Forwards,
                           streamName = streamId,
                           revision = StreamPosition.Start,
                           resolveLinkTos = true,
                           deadline = TimeSpan.FromSeconds(60))
    |> AsyncSeq.ofAsyncEnum
    |> AsyncSeq.toListAsync

let readStreamBackwards (conn: string) (streamId: string) =
    let settings = EventStoreClientSettings.Create(conn)
    let client = new EventStoreClient(settings)
    client.ReadStreamAsync(
        direction = Direction.Backwards,
        streamName = streamId,
        revision = StreamPosition.End,
        deadline = TimeSpan.FromSeconds(30))
    
let sortByStreamPosition (events: ResolvedEvent list) =
    events
    |> List.sortBy (fun e  -> e.Event.EventNumber.ToInt64())
