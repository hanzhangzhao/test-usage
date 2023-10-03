namespace CertificateUsage.Api.Dependencies

open System
open System.Threading.Tasks

open Npgsql
open Npgsql.FSharp

open CertificateUsage.Period
open CertificateUsage.Errors
open CertificateUsage.Postgres
open CertificateUsage.Dao
open CertificateUsage.Api.Dao
open CertificateUsage.Api.Repository.Usage
open CertificateUsage.Api.UsageCommandRepository
open CertificateUsage.Serialization

module UsageDataDependencies =

    type UsageDataReaderIO =
        { GetUsageForCarrierByDate : UsageForCarrierByDateParameters -> Task<CertificateUsageChangeDao option list>
          GetRateChangesForCarrierOnOrAfter : UsageForCarrierByDateParameters -> Task<RateUpdateDataDao option list>
          CloseOutMonth : string -> Period -> Task<Result<int list, Errors>>
          PreviewUsage : string -> Task<UsagePreviewDao option list>
          GetUsageForCarrierByDateFromClosedBook : string -> DateTime -> Task<Result<UsageLineDao option list, Errors>>
          GetRetroUpdates :
              RetroactiveCertificateUpdateType -> string -> Period -> Task<RetroactiveCertificateUsageUpdateDao list>
          GetRetroTerminations : string -> Period -> Task<RetroactiveCertificateUsageTransitionDao list>
          GetRetroEnrollments : string -> Period -> Task<RetroactiveCertificateUsageTransitionDao list>
          InsertCorrections : CertificateUsageDao option list -> Task<int list>
          GetClosedBookDates : string -> Task<DateTime list> }

    module UsageDataReaderIO =
        [<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
        let compose (connectionString : string) : UsageDataReaderIO =
            let connect connStr =
                let src = NpgsqlDataSourceBuilder(connStr)
                src.MapEnum<CoverageType>("type") |> ignore

                src.MapEnum<RetroactiveCertificateUpdateType>("retro_cert_update_type")
                |> ignore

                Sql.fromDataSource (src.Build())

            let certificateUsageChangeSelector = query (connect connectionString)
            let rateSelector = query (connect connectionString)
            let transact = transact (connect connectionString)
            let previewUsageSelector = query (connect connectionString)
            let usageSelector = query (connect connectionString)
            let retroactiveUpdateSelector = query (connect connectionString)

            let retroactiveTransitionSelector = query (connect connectionString)
            let closedBookDateSelector = query (connect connectionString)

            { GetUsageForCarrierByDate =
                getUsageForCarrierByDate certificateUsageChangeSelector mapRowToCertificateUsageChangeDao
              GetRateChangesForCarrierOnOrAfter =
                getRateChangesForCarrierOnOrAfter rateSelector mapRowToRateChangeDataDto
              CloseOutMonth = closeOutMonth transact
              PreviewUsage = previewUsage previewUsageSelector previewMapper
              GetUsageForCarrierByDateFromClosedBook =
                getUsageForCarrierByDateFromClosedBook usageSelector mapCertificateRowToUsageLineDao
              GetRetroUpdates = getRetroUpdates retroactiveUpdateSelector (mapRetroUpdate deserialize<CertificateDao>)
              GetRetroEnrollments =
                getRetroEnrollments retroactiveTransitionSelector (mapRetroTransition deserialize<CertificateDao>)
              GetRetroTerminations =
                getRetroTerminations retroactiveTransitionSelector (mapRetroTransition deserialize<CertificateDao>)
              InsertCorrections = insertCorrections transact
              GetClosedBookDates = getClosedBookDates closedBookDateSelector mapClosedBookDate }
