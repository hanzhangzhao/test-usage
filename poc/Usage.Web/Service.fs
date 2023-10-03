module Usage.Web.Service

open Usage
open Models
open Usage.EventStore
open Usage.Dto.ToDto

let usageService () =
    task {
        let! events = readStream "esdb://admin:changeit@localhost:2113?tls=false" "members"

        let mapped =
            events
            |> List.map toDto
            |> List.choose id
            |> List.where (fun e -> e.Cert <> null)

        mapped
        |> List.iter (fun evnt ->
            let certificate = Db.get evnt.Cert
            let updated = CertificateUsage.nextCertificateState certificate evnt
            updated |> Option.iter Db.put)

        return Db.getAll ()
    }

let getUsage (billingPeriod: BillingPeriod) =
    
    let allUsage = Db.getAll ()

    // All active certs, who were effective BEFORE the epoch
    let activeCerts =
        allUsage
        |> List.where (fun cert ->
            cert.State = Covered
            && billingPeriod.InsideOrBefore(cert.Effective))
        |> List.map (fun cert ->
                cert.ProductSelections
                |> List.map (fun ps ->
                    {| Carrier = cert.Carrier
                       PolicyNumber = cert.PolicyNumber
                       CertificateNumber = cert.CertificateNumber
                       Client = cert.ClientName
                       ProductLine = ps.ProductLine
                       Option = ps.Option
                       Coverage = ps.Coverage |> Option.defaultValue null
                       Volume = ps.Volume.Amount
                       RatePer = ps.RatePer |}))
            |> List.concat
    
    // Certificates which were terminated INSIDE the epoch
    let wereActive =
        allUsage
        |> List.where (fun cert ->
            cert.State = Excluded
            && billingPeriod.Intersects(cert.Effective))
        |> List.map (fun cert ->
            let previouslyCovered =
                cert.History
                |> List.where (fun h ->
                    h.State = Covered
                    && billingPeriod.Intersects(h.Effective))
                |> List.tryHead
            match previouslyCovered with
            | Some covered -> Some (cert, covered.ProductSelections)
            | None -> None)
        |> List.choose id
        |> List.map (fun (cert, productSelections) ->
            productSelections |> List.map (fun ps ->
                {| Carrier = cert.Carrier
                   PolicyNumber = cert.PolicyNumber
                   CertificateNumber = cert.CertificateNumber
                   Client = cert.ClientName
                   ProductLine = ps.ProductLine
                   Option = ps.Option
                   Coverage = ps.Coverage |> Option.defaultValue null
                   Volume = ps.Volume.Amount
                   RatePer = ps.RatePer |}
                )
            )
        |> List.concat
    
    activeCerts @ wereActive
