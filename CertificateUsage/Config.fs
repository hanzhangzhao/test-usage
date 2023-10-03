module CertificateUsage.Config

open System
open Microsoft.Extensions.Configuration

let envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "integration")>]
let config =
    ConfigurationBuilder()
        .AddJsonFile("./appsettings.json")
        .AddJsonFile($"./appsettings.{envName}.json", optional = true)
        .AddEnvironmentVariables()
        .Build()
