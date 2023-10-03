namespace CertificateUsage.Api.Dependencies

open System
open System.Threading.Tasks

open CertificateUsage.Period
open CertificateUsage.Errors
open CertificateUsage.Dao
open CertificateUsage.Api.Dao
open CertificateUsage.Api.Repository.Usage

module Root =
    type Root =
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

    [<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
    let compose (trunk : Trunk.Trunk) =
        { GetUsageForCarrierByDate = trunk.UsageDataReaderDependencies.GetUsageForCarrierByDate
          GetRateChangesForCarrierOnOrAfter = trunk.UsageDataReaderDependencies.GetRateChangesForCarrierOnOrAfter
          CloseOutMonth = trunk.UsageDataReaderDependencies.CloseOutMonth
          PreviewUsage = trunk.UsageDataReaderDependencies.PreviewUsage
          GetUsageForCarrierByDateFromClosedBook =
            trunk.UsageDataReaderDependencies.GetUsageForCarrierByDateFromClosedBook
          GetRetroUpdates = trunk.UsageDataReaderDependencies.GetRetroUpdates
          GetRetroTerminations = trunk.UsageDataReaderDependencies.GetRetroTerminations
          GetRetroEnrollments = trunk.UsageDataReaderDependencies.GetRetroEnrollments
          InsertCorrections = trunk.UsageDataReaderDependencies.InsertCorrections
          GetClosedBookDates = trunk.UsageDataReaderDependencies.GetClosedBookDates }
