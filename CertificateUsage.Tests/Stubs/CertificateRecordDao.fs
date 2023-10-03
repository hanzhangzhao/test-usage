module CertificateUsage.Tests.Stubs.CertificateRecordDao

open System

open CertificateUsage.Dao

let dao =
    { CertificateRecordDao.CertificateNumber = "CertificateNumber"
      Carrier = "Carrier"
      PolicyNumber = "PolicyNumber"
      ClientName = "ClientName"
      Status = CertificateStatus.Active
      Certificate =
        { CertificateNumber = "CertificateNumber"
          CarrierName = "Carrier"
          ScbPolicyNumber = "ScbPolicyNumber"
          PolicyNumber = "PolicyNumber"
          ClientName = "ClientName"
          StartDate = DateTime(2023, 08, 14)
          EndDate = None
          Division = "Division"
          PlanSelections = []
          CertificateStatus = "active" } }
