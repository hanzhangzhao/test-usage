module CertificateUsage.Api.Config

open System
open Microsoft.Extensions.Configuration

let envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

let config =
    ConfigurationBuilder()
        .AddJsonFile("./appsettings.json")
        .AddJsonFile($"./appsettings.{envName}.json", optional = true)
        .AddEnvironmentVariables()
        .Build()
