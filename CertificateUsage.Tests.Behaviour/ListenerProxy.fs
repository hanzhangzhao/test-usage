module CertificateUsage.Tests.Behaviour.ListenerProxy

open CertificateUsage.Dependencies
open CertificateUsage.Tests.Behaviour.Config

let root = config |> Trunk.compose |> Root.compose
let putCertificate = root.PutCertificate
