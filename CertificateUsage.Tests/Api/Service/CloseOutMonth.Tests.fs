module CertificateUsage.Tests.Api.Service.CloseOutMonth

open System
open System.Threading.Tasks

open Xunit
open Expecto

open CertificateUsage.Period
open CertificateUsage.Errors
open CertificateUsage.Api.Service.CloseOutMonth

open CertificateUsage.Tests.Spy
open CertificateUsage.Api.Dependencies
open CertificateUsage.Tests.Stubs

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

module CloseOutMonth =
    [<Fact>]
    let ``successfully executes the closing of the month`` () =
        let closeOutMonthSpy =
            Spy<Task<Result<int list, Errors>>, string>(fun _ -> Task.lift (Ok []))

        let closeOutMonthMock carrier _ = closeOutMonthSpy.Function carrier

        let root =
            { dependencies with
                CloseOutMonth = closeOutMonthMock }

        let billingPeriod =
            { Period.Start = DateTime(2023, 7, 01)
              End = DateTime(2023, 7, 31) }

        let _ = closeOutMonth root "carrier" billingPeriod
        Expect.isTrue (closeOutMonthSpy.CalledOnceWith "carrier") "should have been called"

    [<Fact>]
    let ``fails with CarrierMonthClosed error if already closed`` () =
        let root =
            { dependencies with
                GetClosedBookDates = fun _ -> Task.lift [ DateTime(2023, 8, 31) ] }

        let billingPeriod =
            { Period.Start = DateTime(2023, 8, 01)
              End = DateTime(2023, 8, 31) }

        let actual =
            closeOutMonth root "carrier" billingPeriod
            |> Async.AwaitTask
            |> Async.RunSynchronously

        let expected = Error(Errors.CarrierMonthClosed("carrier", 2023, 8))

        Expect.equal actual expected "should equal"
