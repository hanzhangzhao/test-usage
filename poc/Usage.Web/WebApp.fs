module Usage.Web.WebApp

open System
open Giraffe
open Saturn

let playbackUsage : HttpHandler =
    fun ctx next ->
        task {
            let! res = Service.usageService()
            return! json res ctx next
        }
        
let usage: HttpHandler =
    fun next ctx ->
        let year = ctx.TryGetQueryStringValue("year")
        let month = ctx.TryGetQueryStringValue("month")
        match year, month with
        | Some y, Some m ->
            json (Service.getUsage { Year = int y; Month = int m }) next ctx
        | _ ->
            RequestErrors.BAD_REQUEST ("provide year and month") next ctx
        
        
let certs: HttpHandler =
    fun ctx next ->
        json (Db.getAll()) ctx next

let appRouter = router {
    get "/" (text "usage svc")
    get "/health" (text "usage svc")
    post "/run-fold" playbackUsage
    get "/certificates" certs
    get "/usage" usage
}