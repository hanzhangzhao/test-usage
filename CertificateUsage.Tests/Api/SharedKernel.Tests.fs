namespace CertificateUsage.UnitTests.Api

open System
open Xunit
open Expecto

open CertificateUsage.Api.SharedKernel
        
module DateTime =
    [<Fact>]
    let ``Should parse valid date format`` () =
        match (DateTime.tryParse "2022-03-30") with
        | Ok d ->
            Expect.equal d.Kind DateTimeKind.Unspecified ""
            Expect.equal d (DateTime(2022, 03, 30)) "" 
        | Error e -> failwith $"should have parsed date {e}"

    [<Fact>]        
    let ``Should not parse invalid date format`` () =
        match (DateTime.tryParse "2022-30-01") with
        | Ok _ -> failwith "Should not have parsed"
        | Error e -> Expect.equal e "Invalid date format: 2022-30-01" ""
   