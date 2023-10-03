module CertificateUsage.Dependencies.Root

open System.Diagnostics.CodeAnalysis
open System.Threading.Tasks

open CertificateUsage.Domain
open CertificateUsage.Dao

type Root =
    { InsertCertificateUsage : CertificateUsageChangeDao -> Task<int>
      InsertRate : RateUpdateDataDao -> Task<int>
      EventStoreIO : Leaves.EventStore.IO
      GetCertificate : CertificateUsage -> Task<CertificateRecordDao option>
      PutCertificate : CertificateRecordDao -> Task<int>
      InsertRetroactiveCertificateUpdate : RetroactiveCertificateUpdateDao -> Task<int>
      MapCarrierName : string -> Task<string option> }

[<ExcludeFromCodeCoverage(Justification = "passthrough")>]
let compose (trunk : Trunk.Trunk) =
    { InsertCertificateUsage = trunk.CommandModelIO.InsertCertificateUsage
      InsertRate = trunk.CommandModelIO.InsertRate
      EventStoreIO = trunk.EventStoreIO
      GetCertificate = trunk.CommandModelIO.GetCertificate
      PutCertificate = trunk.CommandModelIO.PutCertificate
      InsertRetroactiveCertificateUpdate = trunk.CommandModelIO.InsertRetroactiveCertificateUpdate
      MapCarrierName = trunk.CommandModelIO.MapCarrierName }
