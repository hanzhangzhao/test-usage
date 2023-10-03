module CertificateUsage.CertificateUsageWorkflow

open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Validation

open CertificateUsage.Dao
open CertificateUsage.ToDomain
open CertificateUsage.ToDao

let writeToWriteModel
    (insertCertificateUsage: CertificateUsageChangeDao -> Task<int>)
    (writeModel: CertificateUsageChangeDao)
    =
    insertCertificateUsage writeModel

let executeCertificateUsageWorkflow insertCertificateUsage metadata =
    toDomain
    >> Validation.map (List.map (toDao metadata >> (writeToWriteModel insertCertificateUsage)))
