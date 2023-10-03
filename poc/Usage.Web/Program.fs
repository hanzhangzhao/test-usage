module Usage.Web.Program

open Saturn
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Serilog

let useSerilog (webHostConfig: IWebHostBuilder) = webHostConfig.UseSerilog()
let addSerilog (app: IApplicationBuilder) =
    let config =
        app.ApplicationServices.GetService<IConfiguration>()

    Log.Logger <-
        LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .CreateLogger()

    app.UseSerilogRequestLogging() |> ignore
    app

let app =
    application {
        webhost_config useSerilog
        app_config addSerilog
        use_developer_exceptions
        use_router WebApp.appRouter
    }

[<EntryPoint>]
let main _ =
    run app
    0