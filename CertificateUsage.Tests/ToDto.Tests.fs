module CertificateUsage.Tests.ToDto

open System

open CertificateUsage
open Xunit
open Expecto

open CertificateUsage.Tests.Stubs.Event
open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Dto.Events.MemberEnrolled
open CertificateUsage.Listener.ToDto

module Stubs =

    let memberEnrolledEvent =
        """
        {
          "benefits_start_date": "2022-03-01",
          "tax_province": "TaxProvince",
          "plan_selections": [ ],
          "certificate_number": "CertificateNumber",
          "policy_number": "PolicyNumber",
          "carrier": "Carrier",
          "client": {
            "name": "Client.Name"
          }
        }
        """

    let memberReinstatementConfirmedEvent = memberEnrolledEvent

    let metadata =
        """
        {
            "version": "Version",
            "create_date": 1646006400
        }
        """

    let memberEnrollmentDto =
        { MemberEnrollmentDto.BenefitsStartDate = DateTime(2022, 3, 1)
          TaxProvince = "TaxProvince"
          PlanSelections = []
          CertificateNumber = "CertificateNumber"
          PolicyNumber = "PolicyNumber"
          ExternalPolicyNumber = None
          CarrierClientCode = None
          Carrier = "Carrier"
          CarrierMapping = None
          Client = { Name = Some "Client.Name" }
          Coverages = None }

    let metadataDto eventType =
        { EventId = Guid.Empty
          EventType = eventType
          Version = "Version"
          CreateDate = DateTime(2022, 2, 28)
          EventNumber = 0UL
          StreamId = "Stream" }

    let memberReinstatementConfirmedDto = memberEnrollmentDto

module ToMetadata =
    [<Fact>]
    let ``transforms event metadata to MetadataDto`` () =
        let event =
            createEvent "MemberEnrollmentConfirmed" Stubs.memberEnrolledEvent Stubs.metadata

        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberEnrollmentConfirmed" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms MemberReinstatementConfirmed event to MetadataDto`` () =
        let terminated =
            Stubs.memberReinstatementConfirmedDto |> Serialization.serializeSnakeCase

        let event = createEvent "MemberReinstatementConfirmed" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberReinstatementConfirmed" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms MemberTerminated event to MetadataDto`` () =
        let terminated = Stubs.MemberTerminatedDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "MemberBenefitEnded" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberBenefitEnded" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms MemberBenefitCancelled event to MetadataDto`` () =
        let terminated = Stubs.MemberCancelledDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "MemberBenefitCancelled" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberBenefitCancelled" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms MemberEnrolledSnapshot event to MetadataDto`` () =
        let terminated = Stubs.MemberCancelledDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "MemberEnrolledSnapshot" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberEnrolledSnapshot" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``returns None given an invalid event type`` () =
        let event = createEvent "ABC" "{}" "{}"
        let actual = toMetadata event
        let expected = None

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms EnrolledMemberTaxProvinceUpdated event to MetadataDto`` () =
        let terminated =
            Stubs.MemberTaxProvinceUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledMemberTaxProvinceUpdated" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "EnrolledMemberTaxProvinceUpdated" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms MemberDependentAdded event to MetadataDto`` () =
        let terminated =
            Stubs.MemberDependentAddedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "MemberDependentAdded" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberDependentAdded" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms MemberSpouseAdded event to MetadataDto`` () =
        let terminated = Stubs.MemberSpouseAddedDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "MemberSpouseAdded" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberSpouseAdded" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms EnrolledDependentRemoved event to MetadataDto`` () =
        let terminated =
            Stubs.DependentTerminatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledDependentRemoved" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "EnrolledDependentRemoved" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms EnrolledSpouseRemoved event to MetadataDto`` () =
        let terminated = Stubs.SpouseTerminatedDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "EnrolledSpouseRemoved" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "EnrolledSpouseRemoved" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms MemberCobUpdated event to MetadataDto`` () =
        let terminated = Stubs.MemberCobUpdatedDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "MemberCobUpdated" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberCobUpdated" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms DependentCobUpdated event to MetadataDto`` () =
        let terminated =
            Stubs.DependentCobUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "DependentCobUpdated" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "DependentCobUpdated" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms SpouseCobUpdated event to MetadataDto`` () =
        let terminated = Stubs.SpouseCobUpdatedDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "SpouseCobUpdated" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "SpouseCobUpdated" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms MemberSpouseCohabUpdated event to MetadataDto`` () =
        let terminated = Stubs.SpouseCohabUpdatedDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "MemberSpouseCohabUpdated" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "MemberSpouseCohabUpdated" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms EnrolledDependentDisabilityUpdated event to MetadataDto`` () =
        let terminated =
            Stubs.DependentDisabilityUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event =
            createEvent "EnrolledDependentDisabilityUpdated" terminated Stubs.metadata

        let actual = toMetadata event
        let expected = Stubs.metadataDto "EnrolledDependentDisabilityUpdated" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms EnrolledDependentPostSecondaryEducationUpdated event to MetadataDto`` () =
        let terminated =
            Stubs.DependentPostSecondaryEducationUpdatedDto.dto
            |> Serialization.serializeSnakeCase

        let event =
            createEvent "EnrolledDependentPostSecondaryEducationUpdated" terminated Stubs.metadata

        let actual = toMetadata event

        let expected =
            Stubs.metadataDto "EnrolledDependentPostSecondaryEducationUpdated" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms EnrolledMemberIncomeUpdated event to MetadataDto`` () =
        let terminated =
            Stubs.MemberIncomeUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledMemberIncomeUpdated" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "EnrolledMemberIncomeUpdated" |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms EnrolledMemberEmploymentUpdated event to MetadataDto`` () =
        let terminated =
            Stubs.MemberEmploymentUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledMemberEmploymentUpdated" terminated Stubs.metadata
        let actual = toMetadata event
        let expected = Stubs.metadataDto "EnrolledMemberEmploymentUpdated" |> Some

        Expect.equal actual expected ""

module ToDto =
    [<Fact>]
    let ``transforms a MemberEnrollmentConfirmed to MemberEventDto`` () =
        let event =
            createEvent "MemberEnrollmentConfirmed" Stubs.memberEnrolledEvent Stubs.metadata

        let actual = eventToDto event
        let expected = Stubs.memberEnrollmentDto |> MemberEnrollmentDto |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms a MemberTermination event`` () =
        let terminated = Stubs.MemberTerminatedDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "MemberBenefitEnded" terminated Stubs.metadata
        let actual = eventToDto event
        let expected = Stubs.MemberTerminatedDto.dto |> MemberTerminatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a MemberCancelled event`` () =
        let terminated = Stubs.MemberCancelledDto.dto |> Serialization.serializeSnakeCase
        let event = createEvent "MemberBenefitCancelled" terminated Stubs.metadata
        let actual = eventToDto event
        let expected = Stubs.MemberCancelledDto.dto |> MemberCancelledDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``return None given an invalid member enrollment event`` () =
        let event = createEvent "MemberEnrollmentConfirmed" "}" "{}"
        let actual = eventToDto event
        let expected = None

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms a MemberReinstatementConfirmed to MemberEventDto`` () =
        let event =
            createEvent "MemberReinstatementConfirmed" Stubs.memberReinstatementConfirmedEvent Stubs.metadata

        let actual = eventToDto event
        let expected = Stubs.memberEnrollmentDto |> MemberReinstatementConfirmed |> Some

        Expect.equal actual expected ""

    [<Fact>]
    let ``return None given an invalid MemberReinstatementConfirmed  event`` () =
        let event = createEvent "MemberReinstatementConfirmed" "}" "{}"
        let actual = eventToDto event
        let expected = None

        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms a MemberEnrolledSnapshot to MemberEventDto`` () =
        let snapshot =
            Stubs.MemberEnrolledSnapshotDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "MemberEnrolledSnapshot" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected =
            Stubs.MemberEnrolledSnapshotDto.dto |> MemberEnrolledSnapshotDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a EnrolledMemberTaxProvinceUpdated to MemberEventDto`` () =
        let snapshot =
            Stubs.MemberTaxProvinceUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledMemberTaxProvinceUpdated" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected =
            Stubs.MemberTaxProvinceUpdatedDto.dto |> MemberTaxProvinceUpdatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a MemberDependentAdded to MemberEventDto`` () =
        let snapshot = Stubs.MemberDependentAddedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "MemberDependentAdded" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.MemberDependentAddedDto.dto |> MemberDependentAddedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a MemberSpouseAdded to MemberEventDto`` () =
        let snapshot = Stubs.MemberSpouseAddedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "MemberSpouseAdded" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.MemberSpouseAddedDto.dto |> MemberSpouseAddedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a EnrolledDependentRemoved to MemberEventDto`` () =
        let snapshot = Stubs.DependentTerminatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledDependentRemoved" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.DependentTerminatedDto.dto |> DependentTerminatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a EnrolledSpouseRemoved to MemberEventDto`` () =
        let snapshot = Stubs.SpouseTerminatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledSpouseRemoved" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.SpouseTerminatedDto.dto |> SpouseTerminatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a MemberCobUpdated to MemberEventDto`` () =
        let snapshot = Stubs.MemberCobUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "MemberCobUpdated" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.MemberCobUpdatedDto.dto |> MemberCobUpdatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a DependentCobUpdated to MemberEventDto`` () =
        let snapshot = Stubs.DependentCobUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "DependentCobUpdated" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.DependentCobUpdatedDto.dto |> DependentCobUpdatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a SpouseCobUpdated to MemberEventDto`` () =
        let snapshot = Stubs.SpouseCobUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "SpouseCobUpdated" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.SpouseCobUpdatedDto.dto |> SpouseCobUpdatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a MemberSpouseCohabUpdated to MemberEventDto`` () =
        let snapshot = Stubs.SpouseCohabUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "MemberSpouseCohabUpdated" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.SpouseCohabUpdatedDto.dto |> SpouseCohabUpdatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a EnrolledDependentDisabilityUpdated to MemberEventDto`` () =
        let snapshot =
            Stubs.DependentDisabilityUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledDependentDisabilityUpdated" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected =
            Stubs.DependentDisabilityUpdatedDto.dto |> DependentDisabilityUpdatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a EnrolledDependentPostSecondaryEducationUpdated to MemberEventDto`` () =
        let snapshot =
            Stubs.DependentPostSecondaryEducationUpdatedDto.dto
            |> Serialization.serializeSnakeCase

        let event =
            createEvent "EnrolledDependentPostSecondaryEducationUpdated" snapshot Stubs.metadata

        let actual = eventToDto event

        let expected =
            Stubs.DependentPostSecondaryEducationUpdatedDto.dto
            |> DependentPostSecondaryEducationUpdatedDto
            |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a EnrolledMemberIncomeUpdated to MemberEventDto`` () =
        let snapshot = Stubs.MemberIncomeUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledMemberIncomeUpdated" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected = Stubs.MemberIncomeUpdatedDto.dto |> MemberIncomeUpdatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transforms a EnrolledMemberEmploymentUpdated to MemberEventDto`` () =
        let snapshot =
            Stubs.MemberEmploymentUpdatedDto.dto |> Serialization.serializeSnakeCase

        let event = createEvent "EnrolledMemberEmploymentUpdated" snapshot Stubs.metadata
        let actual = eventToDto event

        let expected =
            Stubs.MemberEmploymentUpdatedDto.dto |> MemberEmploymentUpdatedDto |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``return None given an invalid event type`` () =
        let event = createEvent "ABC" "{}" "{}"
        let actual = eventToDto event
        let expected = None

        Expect.equal actual expected ""
