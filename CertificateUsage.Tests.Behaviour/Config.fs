module CertificateUsage.Tests.Behaviour.Config

open System
open Microsoft.Extensions.Configuration

let envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

let config =
    ConfigurationBuilder()
        .AddJsonFile($"./appsettings.Behaviour.json", optional = true)
        .AddEnvironmentVariables()
        .Build()
