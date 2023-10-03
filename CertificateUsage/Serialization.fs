module CertificateUsage.Serialization

open JorgeSerrano.Json
open System.Text.Json
open System.Text.Json.Serialization
open System.Diagnostics.CodeAnalysis

let options = JsonSerializerOptions()
options.Converters.Add(JsonFSharpConverter())

[<ExcludeFromCodeCoverage(Justification = "passthrough")>]
let serialize (o: obj) = JsonSerializer.Serialize(o, options)

[<ExcludeFromCodeCoverage(Justification = "passthrough")>]
let deserialize<'a> (str: string) =
    JsonSerializer.Deserialize<'a>(str, options)

[<ExcludeFromCodeCoverage(Justification = "passthrough")>]
let deserializeCaseInsensitive<'a> (str: string) =
    let options = JsonSerializerOptions(PropertyNameCaseInsensitive = true)
    options.Converters.Add(JsonFSharpConverter())
    JsonSerializer.Deserialize<'a>(str, options)

let serializeSnakeCase (o: obj) =
    let jsonSnakeCase =
        JsonSerializerOptions(PropertyNamingPolicy = JsonSnakeCaseNamingPolicy())

    JsonSerializer.Serialize(o, jsonSnakeCase)
