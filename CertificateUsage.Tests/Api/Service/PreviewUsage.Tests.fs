module CertificateUsage.Tests.Api.Service.PreviewUsage_Tests

open System.Threading.Tasks

open Xunit
open Expecto

open CertificateUsage.Api.Dao
open CertificateUsage.Tests.Spy
open CertificateUsage.Tests.Stubs
open CertificateUsage.Api.Dependencies
open CertificateUsage.Api.Service.PreviewUsage

let dependencies : Root.Root =
    { GetUsageForCarrierByDate = fun _ -> Task.lift [ None ]
      GetRateChangesForCarrierOnOrAfter = fun _ -> Task.lift [ None ]
      CloseOutMonth = fun _ _ -> Task.lift (Ok [])
      PreviewUsage = fun _ -> Task.lift []
      GetUsageForCarrierByDateFromClosedBook = fun _ _ -> Task.lift (Ok [])
      GetRetroUpdates = fun _ _ _ -> Task.lift []
      GetRetroTerminations = fun _ _ -> Task.lift []
      GetRetroEnrollments = fun _ _ -> Task.lift []
      InsertCorrections = fun _ -> Task.lift []
      GetClosedBookDates = fun _ -> Task.lift [] }

module PreviewUsage =
    [<Fact>]
    let ``successfully executes a preview usage`` () =
        let previewUsageSpy =
            Spy<Task<UsagePreviewDao option list>, string>(fun _ -> Task.lift [])

        let root =
            { dependencies with
                PreviewUsage = previewUsageSpy.Function }

        let _ = previewUsage root "carrier"

        Expect.isTrue (previewUsageSpy.CalledOnceWith "carrier") "should have been called"
