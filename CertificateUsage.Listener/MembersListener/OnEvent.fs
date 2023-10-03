module CertificateUsage.Listener.OnEvent

open Serilog
open EventStore.Client

open System.Threading.Tasks
open FsToolkit.ErrorHandling
open CertificateUsage.Logging
open CertificateUsage.Errors
open CertificateUsage.Dependencies
open CertificateUsage.CertificateUsageWorkflow
open CertificateUsage.Listener.MembersListener.RetroactiveCertificateUpdateService
open CertificateUsage.Listener.MembersListener.UpsertService
open CertificateUsage.Listener.MembersListener.CarrierAliasMappingService

let eventAppeared (root : Root.Root) (event : ResolvedEvent) =
    let upsertCertificate = upsertCertificateWithRoot root
    let addRetroactiveUpdates = addRetroactiveUpdatesWithRoot root

    task {
        let optionalDto, optionalMetadata = ToDto.toDto event

        match (optionalDto, optionalMetadata) with
        | Some dto, Some metadata ->

            use _ = enrichWithProperties [ "certificate", dto.Cert ]
            Log.Information("Found {@eventType} id {@eventId}", event.Event.EventType, event.Event.EventId.ToString())


            do! upsertCertificate dto
            do! addRetroactiveUpdates dto

            do!
                executeCertificateUsageWorkflow root.InsertCertificateUsage metadata dto
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

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "integration")>]
let membersEventAppeared (root : Root.Root) (subscription : PersistentSubscription) (event : ResolvedEvent) =
    use _ =
        enrichWithProperties [ "eventId", event.Event.EventId; "eventType", event.Event.EventType ]

    Apm.trace $"event/members/{event.Event.EventType}" (fun () ->
        task {
            try
                do! eventAppeared root event
                do! subscription.Ack(event)
            with
            | :? System.Text.Json.JsonException as ex ->
                do! subscription.Nack(PersistentSubscriptionNakEventAction.Skip, "", event)
                Log.Error(ex, "Error handling event with {@action}: {@ex}", ex.Message, "skipped")
                Apm.``exception`` ex
            | ex ->
                do! subscription.Nack(PersistentSubscriptionNakEventAction.Park, "", event)
                Log.Error(ex, "Error handling event with {@action}: {@ex}", ex.Message, "parked")
                Apm.``exception`` ex
        })
