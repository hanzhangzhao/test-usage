module Usage.Dto.Mapping.JsonDeserialization

open EventStore.Client
open JorgeSerrano.Json
open Serilog
open System.Text.Json

let jsonSnakeCase = JsonSerializerOptions(PropertyNamingPolicy = JsonSnakeCaseNamingPolicy())

let deserialize<'dto> (e: ResolvedEvent) =
  try
    Some (JsonSerializer.Deserialize<'dto>(utf8Json = e.Event.Data.Span, options = jsonSnakeCase))
   with
  | ex ->
    Log.Error(ex, "Failed to deserialize {@eventType} event {@eventId} {@ex}",
              e.Event.EventType,
              e.Event.EventNumber.ToString())
    None

  
  