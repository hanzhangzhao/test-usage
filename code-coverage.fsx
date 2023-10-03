open System
open System.IO
open System.Text.RegularExpressions

let lineCoveragePattern = """line-rate="(\d+.\d+)"""
let lineCoverageRegex str = Regex.Match(str, lineCoveragePattern).Groups[1].Captures[0].Value 

let report =
    Directory.GetFiles("CertificateUsage.Tests", "coverage.cobertura.xml", SearchOption.AllDirectories)
    |> Array.head
    |> File.ReadAllText
    
let coverageRate =
    lineCoverageRegex report
    |> Decimal.Parse
    |> (*) 100m
    |> sprintf "%.2f"

printfn $"TOTAL_COVERAGE={coverageRate}"