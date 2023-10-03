module CertificateUsage.Listener.Program

open CertificateUsage.Config
open CertificateUsage.Dependencies
open Elastic.Apm.AspNetCore
open Elastic.Apm.SerilogEnricher
open CertificateUsage
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Saturn
open Serilog

let addApm (app: IApplicationBuilder) =
    if (Config.envName <> "Local") then
        app.UseElasticApm(Config.config)
    else
        app

let useSerilog (webHostConfig: IHostBuilder) = webHostConfig.UseSerilog()

let addSerilog (app: IApplicationBuilder) =

    Log.Logger <-
        LoggerConfiguration()
            .ReadFrom.Configuration(Config.config)
            .Enrich.WithElasticApmCorrelationInfo()
            .Enrich.FromLogContext()
            .CreateLogger()

    Log.Logger.Information("Starting App - Environment Name: {@envName}", Config.envName)
    app.UseSerilogRequestLogging() |> ignore
    app

let addListenerService (services: IServiceCollection) =
    let root = config |> Trunk.compose |> Root.compose
    services.AddSingleton<Root.Root>(root).AddHostedService<Worker>() |> ignore
    ()

let app =
    application {
        use_developer_exceptions
        no_router
        host_config useSerilog
        app_config addApm
        app_config addSerilog
    }

// Forces the worker service to get registered after APM and Serilog
let appWithWorker = app.ConfigureServices(addListenerService)

[<EntryPoint>]
let main _ =
    run appWithWorker
    0
