module CertificateUsage.Logging

open Serilog
open Serilog.Context
open Serilog.Core
open Serilog.Core.Enrichers

let propertyEnricher (name, value) =
    PropertyEnricher(name, value) :> ILogEventEnricher

let enrichWithProperties (properties: (string * obj) seq) =
    properties
    |> Seq.map propertyEnricher
    |> Seq.toArray
    |> LogContext.Push

let logInformation (message:string) (args: string list) =
    let parameters = List.map box args
    Log.Information(message, List.toArray parameters)

let logError (messageTemplate: string) (args: string list) =
    let parameters = List.map box args
    Log.Error(messageTemplate, parameters)
    
let logWarning (message:string) (args: string list) =
    let parameters = List.map box args
    Log.Warning(message, parameters)
