open System
open Elastic.Apm
open Elastic.Apm.Api
open EvolveDb
open Npgsql
open Serilog
open Elastic.Apm.SerilogEnricher
open Microsoft.Extensions.Configuration

let envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

let config =
    ConfigurationBuilder()
        .AddJsonFile("./appsettings.json")
        .AddJsonFile($"./appsettings.{envName}.json", optional = true)
        .AddEnvironmentVariables()
        .Build()

Log.Logger <-
    LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .Enrich.WithElasticApmCorrelationInfo()
        .Enrich.FromLogContext()
        .CreateLogger()

let conn = config.GetValue<string> "Db:ConnectionString"

let migrate =
    fun () ->
        Evolve(
            dbConnection = new NpgsqlConnection(conn),
            Locations = [ "Db/Migrations" ],
            IsEraseDisabled = true,
            logDelegate = (fun l -> Log.Information(l))
        )
            .Migrate()

Agent.Tracer.CaptureTransaction("db-migration", ApiConstants.TypeDb, migrate)
