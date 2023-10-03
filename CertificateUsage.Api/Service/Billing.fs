namespace CertificateUsage.Api.Service

open System

open CertificateUsage.Domain
open CertificateUsage.ToDomain
open CertificateUsage.Api.Dependencies
open CertificateUsage.Api.Repository.Usage
open CertificateUsage.Api.Dto.BillingReadModelDto

module Billing =

    let billingByCarrier (root : Root.Root) (carrier : string) (cutoffDate : DateTime) =
        task {
            let query =
                { Carrier = carrier
                  Cutoff = cutoffDate }

            let billingPeriod =
                { Year = cutoffDate.Year
                  Month = cutoffDate.Month }

            let! usage = root.GetUsageForCarrierByDate query
            let! rates = root.GetRateChangesForCarrierOnOrAfter query
            let rateDomainModels = rates |> List.choose id |> List.map Rate.fromDao

            return
                usage
                |> List.choose id
                |> List.map fromDao
                |> certificateUsageForPeriod billingPeriod
                |> CarrierRate.syncCertificateUsageCarrierRates rateDomainModels
                |> List.map toDto
                |> List.concat
        }

    let billingByCarrierClosedBook (root : Root.Root) (carrier : string) (billingDate : DateTime) =
        task {
            let! usage = root.GetUsageForCarrierByDateFromClosedBook carrier billingDate
            return usage |> Result.map (List.choose id)
        }
