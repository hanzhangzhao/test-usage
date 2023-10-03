module CertificateUsage.Listener.MembersListener.RetroactiveCertificateUpdateService

open System
open System.Threading.Tasks

open CertificateUsage.Domain
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Validation

open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Errors
open CertificateUsage.ToDao
open CertificateUsage.Dependencies
open CertificateUsage.RetroactiveCertificateUpdateWorkflow

open CertificateUsage

let noopTask = task { () }
let ignoreTaskResult (_ : Task<'a>) : Task<unit> = noopTask

let logErrors errors =
    List.iter toErrorLog errors
    errors

let getRetroactiveUpdates (domain : CertificateUsage list, updateType : RetroactiveCertificateUpdateType) =
    domain
    |> List.map (getRetroactiveUpdatesForCertificate DateTime.Now updateType)
    |> List.traverseValidationA id
    |> Validation.map (List.choose id >> List.collect id)

let filterRetroEvent = function
    | MemberEnrolledSnapshotDto _ -> None
    | dto -> Some dto

let toDomain =
    Tuple.pipe (ToDomain.toDomain, RetroactiveCertificateUpdateType.fromDto )
    >> Tuple.traverseValidationFst id

let addRetroactiveUpdatesWithRoot (root : Root.Root) =
    filterRetroEvent
    >> Option.map (
        toDomain
        >> Validation.bind getRetroactiveUpdates
        >> Validation.mapErrors logErrors
        >> Validation.map (List.map RetroactiveCertificateUpdate.fromRetroactiveCertificateUpdate)
        >> Validation.map (List.map (root.InsertRetroactiveCertificateUpdate >> ignoreTaskResult))
        >> Result.defaultValue [ noopTask ]
        >> Task.WhenAll
        >> ignoreTaskResult )
    >> Option.defaultValue noopTask
