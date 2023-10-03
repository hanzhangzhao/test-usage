module CertificateUsage.Tests.Dto.Events.Dto

open System

open Xunit
open Expecto

open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.MemberEnrolled

open CertificateUsage.Tests.Stubs

module Stubs =
    let cert = "123456"

    let memberEnrollmentDto =
        { BenefitsStartDate = DateTime(2023, 6, 15)
          TaxProvince = "BC"
          PlanSelections = []
          CertificateNumber = cert
          PolicyNumber = ""
          ExternalPolicyNumber = Some "ExternalPolicyNumber"
          CarrierClientCode = None
          Carrier = ""
          CarrierMapping = None
          Client = { Name = Some "" }
          Coverages = None }

module MemberEventDto =
    module Cert =
        let testData : obj[] list =
            [ [| { Stubs.memberEnrollmentDto with
                     CertificateNumber = "12345" }
                 |> MemberEnrollmentDto
                 "12345" |]
              [| { Stubs.memberEnrollmentDto with
                     CertificateNumber = "12345" }
                 |> MemberReinstatementConfirmed
                 "12345" |]
              [| MemberTerminatedDto MemberTerminatedDto.dto; "1000012" |]
              [| MemberCancelledDto MemberCancelledDto.dto; "1000012" |]
              [| MemberEnrolledSnapshotDto MemberEnrolledSnapshotDto.dto; "1000012" |]
              [| MemberTaxProvinceUpdatedDto MemberTaxProvinceUpdatedDto.dto
                 "CertificateNumber" |]
              [| MemberSpouseAddedDto MemberSpouseAddedDto.dto; "CertificateNumber" |]
              [| MemberDependentAddedDto MemberDependentAddedDto.dto; "CertificateNumber" |]
              [| SpouseTerminatedDto SpouseTerminatedDto.dto; "CertificateNumber" |]
              [| SpouseCohabUpdatedDto SpouseCohabUpdatedDto.dto; "CertificateNumber" |]
              [| MemberCobUpdatedDto MemberCobUpdatedDto.dto; "CertificateNumber" |]
              [| DependentCobUpdatedDto DependentCobUpdatedDto.dto; "CertificateNumber" |]
              [| SpouseCobUpdatedDto SpouseCobUpdatedDto.dto; "CertificateNumber" |]
              [| DependentTerminatedDto DependentTerminatedDto.dto; "CertificateNumber" |]
              [| DependentPostSecondaryEducationUpdatedDto DependentPostSecondaryEducationUpdatedDto.dto
                 "CertificateNumber" |]
              [| DependentDisabilityUpdatedDto DependentDisabilityUpdatedDto.dto
                 "CertificateNumber" |]
              [| MemberEmploymentUpdatedDto MemberEmploymentUpdatedDto.dto
                 "CertificateNumber" |]
              [| MemberIncomeUpdatedDto MemberIncomeUpdatedDto.dto; "CertificateNumber" |]
              [| MemberRateChangedDto MemberRateChanged.Dto.stub
                 MemberRateChanged.certificateNumber |] ]

        [<Theory>]
        [<MemberData(nameof testData)>]
        let ``gets a certificate number from a Dto`` (dto : MemberEventDto, expected : string) =
            let actual = dto.Cert
            Expect.equal (Some expected) actual ""

module RateEventDto =
    module Carrier =
        [<Fact>]
        let ``get a carrier from a CarrierRateModifiedDto`` () =
            let expected = "Carrier"
            let actual = (CarrierRateModifiedDto.dto |> CarrierRateModifiedDto).Carrier
            Expect.equal expected actual ""

        [<Fact>]
        let ``get a policy number from a CarrierRateModifiedDto`` () =
            let expected = "PolicyNumber"
            let actual = (CarrierRateModifiedDto.dto |> CarrierRateModifiedDto).PolicyNumber
            Expect.equal expected actual ""

        [<Fact>]
        let ``get an option from a CarrierRateModifiedDto`` () =
            let expected = "Option"
            let actual = (CarrierRateModifiedDto.dto |> CarrierRateModifiedDto).Option
            Expect.equal expected actual ""

        [<Fact>]
        let ``get a coverage from a CarrierRateModifiedDto`` () =
            let expected = "Coverage"
            let actual = (CarrierRateModifiedDto.dto |> CarrierRateModifiedDto).Coverage
            Expect.equal expected actual ""

        [<Fact>]
        let ``get a product line from a CarrierRateModifiedDto`` () =
            let expected = "ProductLine"
            let actual = (CarrierRateModifiedDto.dto |> CarrierRateModifiedDto).ProductLine
            Expect.equal expected actual ""
