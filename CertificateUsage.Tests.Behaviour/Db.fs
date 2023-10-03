module CertificateUsage.Tests.Behaviour.Db

open System

open Npgsql.FSharp

open CertificateUsage.Postgres
open Config

let connectionString = config["Db:ConnectionString"]

let nextBillingPeriod () =
    let period =
          ( query
              (Sql.connect connectionString)
              "select max(billing_date) as last_billing_period from certificate_usage"
              []
              (fun reader -> reader.dateTimeOrNone "last_billing_period")
          ).Result

    period
    |> List.tryHead
    |> Option.flatten
    |> Option.map (fun (period: DateTime) -> period.AddMonths(1))
    |> Option.defaultValue DateTime.Now
