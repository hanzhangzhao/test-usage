module CertificateUsage.Tests.Stubs.MemberEnrollment

open System
open CertificateUsage.Dto.Events.MemberEnrolled

let benefitsStartDate = DateTime(2023, 09, 27)
let taxProvince = "Manitoba"
let certificateNumber = "CertificateNumber"
let policyNumber = "PolicyNumber"
let externalPolicyNumber = "ExternalPolicyNumber"
let carrierClientCode = "CarrierClientCode"
let carrier = "Carrier"
let coverages = None
let carrierMapping = None
let clientName = "ClientName"

module Dto =
    let stub =
        { MemberEnrollmentDto.BenefitsStartDate = DateTime(2021, 01, 01)
          TaxProvince = taxProvince
          CertificateNumber = certificateNumber
          PolicyNumber = policyNumber
          ExternalPolicyNumber = Some externalPolicyNumber
          CarrierClientCode = Some carrierClientCode
          Carrier = carrier
          Coverages = coverages
          Client = { Name = Some clientName }
          CarrierMapping = carrierMapping
          PlanSelections = [] }
