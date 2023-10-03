module CertificateUsage.Dependencies.Leaves

open System.Threading.Tasks

open Microsoft.Extensions.Configuration
open Npgsql
open Npgsql.FSharp


open CertificateUsage
open CertificateUsage.Serialization
open CertificateUsage.Repository
open CertificateUsage.Dao
open CertificateUsage.Domain

module WriteModel =
    type IO =
        { InsertCertificateUsage : CertificateUsageChangeDao -> Task<int>
          InsertRate : RateUpdateDataDao -> Task<int>
          GetCertificate : CertificateUsage -> Task<CertificateRecordDao option>
          PutCertificate : CertificateRecordDao -> Task<int>
          InsertRetroactiveCertificateUpdate : RetroactiveCertificateUpdateDao -> Task<int>
          MapCarrierName : string -> Task<string option> }

    [<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
    let compose connectionString : IO =
        let connect connStr =
            let src = NpgsqlDataSourceBuilder(connStr)
            src.MapEnum<CoverageType>("type") |> ignore

            src.MapEnum<Dao.RetroactiveCertificateUpdateType>("retro_cert_update_type")
            |> ignore

            Sql.fromDataSource (src.Build())

        let insert = Postgres.nonQuery (connect connectionString)
        let reader = Postgres.queryFirstOrNone (connect connectionString)
        let carrierNameReader = Postgres.queryFirstOrNone (connect connectionString)

        { InsertCertificateUsage = insertCertificateUsage serialize insert
          InsertRate = insertRate serialize insert
          GetCertificate = queryCertificate reader (rowToCertificateRecordDao deserialize<CertificateDao>)
          PutCertificate = upsertCertificateRecordDao serialize insert
          InsertRetroactiveCertificateUpdate = insertRetroactiveCertificateUpdate insert
          MapCarrierName = mapCarrierName carrierNameReader nameMapper }

module EventStore =
    open CertificateUsage.EventStore

    type IO =
        { CreateMembersSubscription : unit -> Task<unit>
          CreateRateSubscription : unit -> Task<unit>
          SubscribeToMembersStream : SubscriptionHandler -> Task<unit>
          SubscribeToRateStream : SubscriptionHandler -> Task<unit> }

    let compose (config : IConfiguration) =
        let streamName = config.GetValue<string>("EventStore:MembersStream")
        let listenerName = config.GetValue<string>("EventStore:ListenerGroup")
        let rateStreamName = config.GetValue<string>("EventStore:CarrierRateStream")

        let rateListenerName =
            config.GetValue<string>("EventStore:CarrierRateListenerGroup")

        let esConn = config.GetValue<string>("EventStore:ConnectionString")

        { CreateMembersSubscription = fun () -> createSubscription esConn streamName listenerName
          CreateRateSubscription = fun () -> createSubscription esConn rateStreamName rateListenerName
          SubscribeToMembersStream = subscribe esConn streamName listenerName
          SubscribeToRateStream = subscribe esConn rateStreamName rateListenerName }
