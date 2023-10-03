module CertificateUsage.Tests.Stubs.MemberEnrolledSnapshotDto

open System
open CertificateUsage.Dto.Events.MemberEnrolledSnapshot

let dto =
    { BenefitsStartDate = DateTime(2021, 01, 01)
      TaxProvince = "BC"
      PlanSelections = []
      CertificateNumber = "1000012"
      PolicyNumber = "9999"
      ExternalPolicyNumber = Some "9998"
      CarrierClientCode = "9105" |> Some
      Carrier = "great_west_life"
      Coverages = None
      CarrierMapping = None
      Client = { Name = Some "Test" } }
