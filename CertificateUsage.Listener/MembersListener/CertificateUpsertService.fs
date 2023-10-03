module CertificateUsage.Listener.MembersListener.UpsertService

open System.Threading.Tasks

open CertificateUsage.Dto.Events
open FsToolkit.ErrorHandling

open CertificateUsage.Errors
open CertificateUsage.Dependencies
open CertificateUsage.ToDao
open CertificateUsage.CertificateStateWorkflow

let noopTask = task { () }
let ignoreTaskResult (_ : Task<'a>) : Task<unit> = noopTask

let logErrors errors =
    List.iter toErrorLog errors
    errors

let putCertificates (root : Root.Root) =
    List.map (root.PutCertificate >> ignoreTaskResult)

let getCertificate (root : Root.Root) =
    (root.GetCertificate >> Async.AwaitTask >> Async.RunSynchronously)

let upsertCertificateWithRoot (root : Root.Root) =
    CertificateUsage.ToDomain.toDomain
    >> Validation.bind (fun usages ->
        List.map (fun usage -> (executeCertificateWorkflow (getCertificate root usage) usage)) usages
        |> List.traverseResultA id)
    >> Validation.mapErrors logErrors
    >> Validation.map (List.map Certificate.fromDomain)
    >> Validation.map (putCertificates root)
    >> Result.defaultValue [ noopTask ]
    >> Task.WhenAll
    >> ignoreTaskResult
