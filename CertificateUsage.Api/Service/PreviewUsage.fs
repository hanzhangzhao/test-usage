module CertificateUsage.Api.Service.PreviewUsage

open CertificateUsage.Api.Dependencies
open CertificateUsage.Api.Dao

let previewUsage (root : Root.Root) (carrier : string) =
    task {
        let! usagePreviewDaos = root.PreviewUsage carrier
        return usagePreviewDaos |> List.choose id |> List.map usagePreviewDaoToUsagePreviewDto
    }
