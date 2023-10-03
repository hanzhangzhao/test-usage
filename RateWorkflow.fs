module CertificateUsage.RateWorkflow

open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Validation

open CertificateUsage.Dao
open CertificateUsage.ToDomain.Rate
open CertificateUsage.ToDao.Rate

let writeToWriteModel (insertCertificateUsage: RateUpdateDataDao -> Task<int>) (writeModel: RateUpdateDataDao) =
    insertCertificateUsage writeModel

let executeRateWorkflow insertRateUsage metadata =
    toDomain
    >> Validation.map (List.map (toDao metadata >> (writeToWriteModel insertRateUsage)))
