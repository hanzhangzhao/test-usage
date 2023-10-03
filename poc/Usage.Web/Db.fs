module Usage.Web.Db

open System.Collections.Generic
open Usage.Models

let private db = Dictionary<string, Certificate>()

let get (cert: string) =
    match db.TryGetValue(cert) with
    | false, _ -> None
    | true, cert -> cert |> Some
    
let put (cert: Certificate) =
    db[cert.CertificateNumber] <- cert
    ()
    
let getAll () =
    db.Values |> Seq.map id |> Seq.toList