module CertificateUsage.Api.Program

open System.Text.Json
open System.Text.Json.Serialization

open Giraffe
open Saturn
open Elastic.Apm.NetCoreAll
open Elastic.Apm.SerilogEnricher
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting.Server.Features
open Serilog

open CertificateUsage.Api.WebApp
open CertificateUsage.Api
open CertificateUsage.Api.Dependencies

let addApm (app: IApplicationBuilder) =
    if Config.envName <> "Local" then
        app.UseAllElasticApm(Config.config)
    else
        app

let useSerilog (hostConfig: IHostBuilder) = hostConfig.UseSerilog()

let addSerilog (app: IApplicationBuilder) =
    Log.Logger <-
        LoggerConfiguration()
            .ReadFrom.Configuration(Config.config)
            .Enrich.WithElasticApmCorrelationInfo()
            .Enrich.WithEnvironmentName()
            .Enrich.FromLogContext()
            .CreateLogger()

    let envName = Config.envName

    let addressFeature = app.ServerFeatures.Get<IServerAddressesFeature>()

    Log.Logger.Information("Starting App - Environment Name: {@envName}", envName)

    if addressFeature <> null then
        Log.Logger.Information("Listening on {@addresses}", addressFeature.Addresses)

    app

let configureSerializer (svc: IServiceCollection) =
    let jsonOptions =
        JsonFSharpOptions
            .FSharpLuLike()
            .ToJsonSerializerOptions()
    jsonOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    svc.AddSingleton<Json.ISerializer>(SystemTextJson.Serializer(jsonOptions))

let addSwagger (app: IApplicationBuilder) =
    let staticFileOptions = StaticFileOptions(ServeUnknownFileTypes = true)

    app
        .UseStaticFiles(staticFileOptions)
        .UseSwaggerUi3(fun a -> a.DocumentPath <- "/swagger.yml")

let app =
    application {
        app_config addApm
        host_config useSerilog
        app_config addSerilog
        use_cors "allow_any" (fun c -> c.AllowAnyOrigin().AllowAnyHeader() |> ignore)
        app_config addSwagger
        use_router (Config.config |> Trunk.compose |> Root.compose |> appRouter)
        use_developer_exceptions
        use_static "wwwroot"
        service_config configureSerializer
        url (Config.config.GetSection("Server").GetValue("Url"))
    }

[<EntryPoint>]
let main _ =
    run app
    0
