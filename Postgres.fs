module CertificateUsage.Postgres

open System.Diagnostics.CodeAnalysis
open Npgsql
open Npgsql.FSharp

open CertificateUsage.IRowReader


type DuplicateKeyValueExceptionCode =
    | Unknown
    | DuplicateKeyValueExceptionCode

let getPostgresExceptionType (ex : PostgresException) : DuplicateKeyValueExceptionCode =
    match ex.SqlState with
    | "23505" -> DuplicateKeyValueExceptionCode
    | _ -> Unknown

[<ExcludeFromCodeCoverage(Justification = "integration")>]
let query connection q parameters (mapper: IRowReader -> 'a) =
    connection
    |> Sql.query q
    |> Sql.parameters parameters
    |> Sql.executeAsync (fun rowReader -> mapper (NpgsqlFSharpRowReader(rowReader)))

[<ExcludeFromCodeCoverage(Justification = "integration")>]
let queryFirstOrNone connection q parameters mapper =
    task {
        let! response =
            connection
            |> Sql.query q
            |> Sql.parameters parameters
            |> Sql.executeAsync (fun rowReader -> mapper (NpgsqlFSharpRowReader(rowReader)))

        return List.tryHead response
    }

[<ExcludeFromCodeCoverage(Justification = "integration")>]
let nonQuery connection q parameters =
    try
        connection
        |> Sql.query q
        |> Sql.parameters parameters
        |> Sql.executeNonQueryAsync
    with ex ->
        Serilog.Log.Error("{@ex}", ex)
        task { return 0 }

[<ExcludeFromCodeCoverage(Justification = "integration")>]
let transact (connection : Sql.SqlProps) queries =
    try
        connection |> Sql.executeTransactionAsync queries
    with ex ->
        Serilog.Log.Error("{@ex}", ex)
        task { return [ 0 ] }
