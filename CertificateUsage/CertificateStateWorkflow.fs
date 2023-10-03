module CertificateUsage.CertificateStateWorkflow

open FsToolkit.ErrorHandling

open CertificateUsage
open CertificateUsage.Dto.Events
open CertificateUsage.Dao
open CertificateUsage.Domain

let coveredCertificateToCertificate (dao: CertificateRecordDao option) (event: CoveredCertificate) =
    let endDate = dao |> Option.bind (fun dao -> dao.Certificate.EndDate)

    { CertificateNumber = CertificateNumber event.CertificateNumber
      CarrierName = CarrierName.create event.Carrier
      ClientName = ClientName.create event.ClientName
      PolicyNumber = PolicyNumber.create event.PolicyNumber
      ScbPolicyNumber = PolicyNumber.create event.ScbPolicyNumber
      StartDate = event.Effective
      EndDate = endDate
      Division = Division.create event.Division
      PlanSelections = event.PlanSelections
      CertificateStatus = CertificateStatus.Active }
    |> Ok

let exclusionCertificateToCertificate (dao: CertificateRecordDao option) (event: ExcludedCertificate) =
    dao
    |> Option.map (fun dao ->
        { CertificateNumber = CertificateNumber.create event.CertificateNumber
          CarrierName = CarrierName.create event.Carrier
          ClientName = ClientName.create event.ClientName
          PolicyNumber = PolicyNumber.create event.PolicyNumber
          ScbPolicyNumber = PolicyNumber.create event.ScbPolicyNumber
          StartDate = dao.Certificate.StartDate
          EndDate = Some event.Effective
          Division = Division.create event.Division
          PlanSelections = event.PlanSelections
          CertificateStatus = CertificateStatus.Terminated }
        |> Ok)
    |> Option.defaultValue (Error(Errors.MissingCertificate event.CertificateNumber))

let toCertificate (dao: CertificateRecordDao option) (domain: Domain.CertificateUsage) =
    match domain with
    | CoveredEvent coveredEvent -> coveredCertificateToCertificate dao coveredEvent
    | ExclusionEvent excludedCertificate -> exclusionCertificateToCertificate dao excludedCertificate

let executeCertificateWorkflow (dao: CertificateRecordDao option) certificateUsageEvents =
    toCertificate dao certificateUsageEvents
