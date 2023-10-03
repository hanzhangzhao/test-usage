module CertificateUsage.Api.Service.CloseOutMonth

open CertificateUsage.Period
open CertificateUsage.Errors
open CertificateUsage.Api.Dependencies

let closeOutMonth (root : Root.Root) (carrier : string) (billingPeriod : Period) =
    task {
        let! closeBookDates = root.GetClosedBookDates carrier

        let! result =
            closeBookDates
            |> List.tryFind (fun date -> date = billingPeriod.End)
            |> Option.map (fun date -> task { return Error(Errors.CarrierMonthClosed(carrier, date.Year, date.Month)) })
            |> Option.defaultWith (fun () -> root.CloseOutMonth carrier billingPeriod)

        return result
    }
