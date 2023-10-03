module CertificateUsage.Listener.Apm

open System.Threading.Tasks
open Elastic.Apm
open Elastic.Apm.Api

let trace (name: string) (f: unit -> Task) =
    task { do! Agent.Tracer.CaptureTransaction(name, ApiConstants.TypeMessaging, f) }

let ``exception`` (ex: exn) = Agent.Tracer.CaptureException(ex)

let ``error`` (culprit: string) (msg: string) = Agent.Tracer.CaptureError(msg, culprit)
