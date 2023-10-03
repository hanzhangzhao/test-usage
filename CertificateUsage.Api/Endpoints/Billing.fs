module CertificateUsage.Api.Endpoints.Usage

open System

open Giraffe
open Saturn
open FsToolkit.ErrorHandling

open CertificateUsage.Period
open CertificateUsage.Errors
open CertificateUsage.Api.Service.Billing
open CertificateUsage.Api.Dependencies
open CertificateUsage.Api.Dependencies.Root
open CertificateUsage.Api.SharedKernel.DateTime
open CertificateUsage.Api.SharedKernel.Giraffe
open CertificateUsage.Api.Service.CloseOutMonth
open CertificateUsage.Api.Service.PreviewUsage
open CertificateUsage.Api.Service.CloseOutCorrections

let billingByCarrierAndMonthHandler (root : Root) (carrier : string) : HttpHandler =
    fun next ctx ->
        task {
            let! billingResult =
                asyncResult {
                    let! yearParam = ctx.GetQueryStringValue "year" |> Result.mapError Errors.BadFormat
                    let! monthParam = ctx.GetQueryStringValue "month" |> Result.mapError Errors.BadFormat
                    let! dayParam = ctx.GetQueryStringValue "day" |> Result.mapError Errors.BadFormat

                    let! asDate =
                        $"{yearParam}-{monthParam}-{dayParam} 23:59:59"
                        |> tryParse
                        |> Result.mapError Errors.BadFormat

                    return! billingByCarrierClosedBook root carrier asDate
                }

            return!
                match billingResult with
                | Ok billing -> Successful.okJson billing next ctx
                | Error e ->
                    match e with
                    | BadFormat _ -> RequestErrors.BAD_REQUEST (toMessage e) next ctx
                    | _ -> ServerErrors.INTERNAL_ERROR (toMessage e) next ctx
        }

let closeUsageForCarrierInYearAndMonthHandler
    (root : Root.Root)
    (carrier : string)
    (year : int)
    (month : int)
    (day : int)
    : HttpHandler =
    fun next ctx ->
        task {
            let! result =
                asyncResult {
                    let! billingPeriodCloseDate =
                        $"{year}-{month}-{day}" |> tryParse |> Result.mapError Errors.BadFormat

                    let billingStart = billingPeriodCloseDate.AddMonths(-1)
                    let billingEnd = billingPeriodCloseDate.AddSeconds(-1)
                    let billingPeriod =
                        { Period.Start = billingStart
                          End = billingEnd }

                    let! result = closeOutMonth root carrier billingPeriod

                    let! _ = closeOutRetroactiveUpdates root carrier billingPeriod
                    let! _ = Terminations.closeOutRetroactiveTerminations root carrier billingPeriod
                    let! _ = Enrollments.closeOutRetroactiveEnrollments root carrier billingPeriod

                    return result
                }

            return!
                match result with
                | Ok _ -> Successful.okJson {| Status = "Ok" |} next ctx
                | Error e ->
                    toErrorLog e

                    match e with
                    | CarrierMonthClosed _ -> RequestErrors.CONFLICT (toMessage e) next ctx
                    | BadFormat _ -> RequestErrors.BAD_REQUEST (toMessage e) next ctx
                    | _ -> ServerErrors.INTERNAL_ERROR e next ctx
        }

let previewUsageByCarrierYearAndMonthHandler (root : Root.Root) (carrier : string) : HttpHandler =
    fun next ctx ->
        task {
            let! result = previewUsage root carrier
            return! Successful.okJson result next ctx
        }

let usageRouter (compositionRoot : Root.Root) : HttpHandler =
    router {
        getf "/carrier/%s/billing" (fun carrier -> billingByCarrierAndMonthHandler compositionRoot carrier)

        postf "/close/carrier/%s/year/%i/month/%i/day/%i" (fun (carrier, year, month, day) ->
            closeUsageForCarrierInYearAndMonthHandler compositionRoot carrier year month day)

        getf "/preview/carrier/%s" (fun carrier -> previewUsageByCarrierYearAndMonthHandler compositionRoot carrier)
    }
