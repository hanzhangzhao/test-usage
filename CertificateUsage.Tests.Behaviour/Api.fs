module CertificateUsage.Tests.Behaviour.Api

open System
open System.Threading
open FsHttp.DslCE
open FsHttp

open CertificateUsage.Serialization
open CertificateUsage.Api.Dto
open CertificateUsage.Api.Dependencies

open CertificateUsage.Tests.Behaviour.Config
open CertificateUsage.Api.Service

let root = config |> Trunk.compose |> Root.compose
let getBilling = Billing.billingByCarrierClosedBook root

let host = config["UsageApi:Host"]

let deserializeUsage (data: string) : BillingReadModelDto list =
    deserializeCaseInsensitive<BillingReadModelDto list> (data)

let callGetCertificateUsage (carrier: string) (year: int) (month: int) =
    let payload =
        http { GET $"{host}/v0/usage/carrier/{carrier}/billing?year={year}&month={month}" }
        |> Request.send

    payload.content.ReadAsStringAsync()
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> deserializeUsage


let rec getUsageWithRetry retries carrier year month predicate =
    let result = callGetCertificateUsage carrier year month

    result
    |> List.tryFind predicate
    |> fun optionalUsage ->
        match optionalUsage with
        | Some usage -> usage
        | _ ->
            match retries with
            | 0 -> failwith $"[{DateTime.Now} could not find matching usage line in {result}"
            | _ ->
                Thread.Sleep(12800 / (int (Math.Pow(2, float (retries - 1) )) ))
                getUsageWithRetry (retries - 1) carrier year month predicate

let getUsage carrier year month predicate =
    getUsageWithRetry 8 carrier year month predicate
