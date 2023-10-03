module CertificateUsage.Listener.RateListener.OnEvent

open Serilog
open EventStore.Client

open System.Threading.Tasks
open FsToolkit.ErrorHandling
open CertificateUsage.Logging
open CertificateUsage.Errors
open CertificateUsage.Dependencies
open CertificateUsage.RateWorkflow
open CertificateUsage.Listener
open CertificateUsage.Listener.RateListener.ToDto

let eventAppeared (root: Root.Root) (event: ResolvedEvent) =
    task {
        let optionalDto, optionalMetadata = toDto event

        match (optionalDto, optionalMetadata) with
        | Some dto, Some metadata ->
            use _ =
                [ "carrier", box dto.Carrier
                  "policy", dto.PolicyNumber
                  "productLine", dto.ProductLine
                  "option", dto.Option
                  "coverage", dto.Coverage ]
                |> List.toSeq
                |> enrichWithProperties

            Log.Information("Found {@eventType} id {@eventId}", event.Event.EventType, event.Event.EventId.ToString())

            do!
                executeRateWorkflow root.InsertRate metadata dto
                |> fun result ->
                    match result with
                    | Ok a ->
                        task {
                            let! _ = Task.WhenAll(a)
                            do logInformation "Finished Handling Event" []
                            return ()
                        }
                    | Error e ->
                        do e |> List.iter toErrorLog
                        do e |> List.map toMessage |> String.concat "; " |> Apm.error event.Event.EventType
                        task { () }
        | _ -> logInformation "Ignoring Event" []
    }

let rateEventAppeared (root: Root.Root) (subscription: PersistentSubscription) (event: ResolvedEvent) =
    use _ =
        enrichWithProperties [ "eventId", event.Event.EventId; "eventType", event.Event.EventType ]

    Apm.trace $"event/carrier_rates/{event.Event.EventType}" (fun () ->
        task {
            do! eventAppeared root event
            do! subscription.Ack(event)
        })
