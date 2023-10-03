module CertificateUsage.Api.WebApp

open Saturn
open Giraffe

open CertificateUsage.Api.Endpoints.Usage
open CertificateUsage.Api.Dependencies
open CertificateUsage.Api.SharedKernel.Giraffe

let healthHandler: HttpHandler =
    fun next ctx -> Successful.okJson {| Status = "OK" |} next ctx

let appRouter (compositionRoot: Root.Root) =
    router {
        get "/health" healthHandler
        forward "/v0/usage" (usageRouter compositionRoot)
        not_found_handler (RequestErrors.notFound (text "Nothing here."))
    }
