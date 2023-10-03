module CertificateUsage.UnitTests.Mocks

open System.IO
open System.Text
open System.Collections.Generic
open System.Threading.Tasks

open Giraffe

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Primitives

open NSubstitute

let next: HttpFunc = Some >> Task.FromResult

let buildMockHttpContext () =
    let ctx = Substitute.For<HttpContext>()

    ctx.Request.Scheme.ReturnsForAnyArgs "http" |> ignore

    ctx.Request.Host.ReturnsForAnyArgs(HostString("localhost")) |> ignore

    ctx.Request.PathBase.ReturnsForAnyArgs(PathString "") |> ignore

    ctx.Items.ReturnsForAnyArgs(Dictionary<_, _>()) |> ignore

    ctx.RequestServices
        .GetService(typeof<Json.ISerializer>)
        .Returns(NewtonsoftJson.Serializer(NewtonsoftJson.Serializer.DefaultSettings))
    |> ignore

    ctx.RequestServices
        .GetService(typeof<INegotiationConfig>)
        .Returns(DefaultNegotiationConfig())
    |> ignore

    let config =
        ConfigurationBuilder()
            .AddJsonFile("./appsettings.json")
            .AddEnvironmentVariables()
            .Build()

    ctx.RequestServices.GetService(typeof<IConfiguration>).Returns(config) |> ignore

    ctx.Response.Body <- new MemoryStream()
    ctx

let getContext (method: string) (path: string) =
    let ctx = buildMockHttpContext ()

    ctx.Request.Method.ReturnsForAnyArgs method |> ignore

    ctx.Request.Path.ReturnsForAnyArgs(PathString path) |> ignore

    ctx.Request.HasFormContentType.ReturnsForAnyArgs(true) |> ignore

    ctx

let getContextWithQueryFields (method: string) (path: string) (fields: (string * StringValues) list) =
    let ctx = getContext method path
    
    let queryFields = dict fields |> Dictionary
    
    let queryCollection = QueryCollection(queryFields)
                
    ctx.Request.Query.ReturnsForAnyArgs(queryCollection)
    |> ignore

    ctx
    
let getBody (ctx: HttpContext) =
    ctx.Response.Body.Position <- 0L
    use reader = new StreamReader(ctx.Response.Body, Encoding.UTF8)
    reader.ReadToEnd()
    
