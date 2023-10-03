namespace CertificateUsage.Api.SharedKernel

open System
open Giraffe

module Giraffe =
    module Successful =
        let okJson<'t> (data: 't) = setStatusCode 200 >=> json data

module DateTime =
    let tryParse (date: string) =
        match DateTime.TryParse(date) with
        | true, d -> Ok d
        | false, _ -> Error $"Invalid date format: {date}"
