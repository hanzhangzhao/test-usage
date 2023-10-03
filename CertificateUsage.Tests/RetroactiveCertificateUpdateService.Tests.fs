module CertificateUsage.Tests.RetroactiveCertificateUpdateService_Tests

open Xunit
open Expecto

open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Tests.Dto.Events.Dto
open CertificateUsage.Tests.Stubs
open CertificateUsage.Listener.MembersListener.RetroactiveCertificateUpdateService

module FilterRetroEvent =
    [<Fact>]
    let ``it filter out a MemberEnrolledSnapshotDto`` () =
        let testcase = Stubs.MemberEnrolledSnapshotDto.dto |> MemberEnrolledSnapshotDto
        let actual = filterRetroEvent testcase
        let expected = None

        Expect.equal actual expected

    let nonSnapshotMemberDtos : obj[] list =
        [ [| MemberEnrollmentDto Stubs.memberEnrollmentDto |]
          [| MemberReinstatementConfirmed Stubs.memberEnrollmentDto |]
          [| MemberTerminatedDto MemberTerminatedDto.dto |]
          [| MemberCancelledDto MemberCancelledDto.dto |]
          [| MemberTaxProvinceUpdatedDto MemberTaxProvinceUpdatedDto.dto |]
          [| MemberSpouseAddedDto MemberSpouseAddedDto.dto |]
          [| MemberDependentAddedDto MemberDependentAddedDto.dto |]
          [| SpouseTerminatedDto SpouseTerminatedDto.dto |]
          [| SpouseCohabUpdatedDto SpouseCohabUpdatedDto.dto |]
          [| MemberCobUpdatedDto MemberCobUpdatedDto.dto |]
          [| DependentCobUpdatedDto DependentCobUpdatedDto.dto |]
          [| SpouseCobUpdatedDto SpouseCobUpdatedDto.dto |]
          [| DependentTerminatedDto DependentTerminatedDto.dto |]
          [| DependentPostSecondaryEducationUpdatedDto DependentPostSecondaryEducationUpdatedDto.dto |]
          [| DependentDisabilityUpdatedDto DependentDisabilityUpdatedDto.dto |]
          [| DependentDisabilityUpdatedDto DependentDisabilityUpdatedDto.dto |]
          [| MemberIncomeUpdatedDto MemberIncomeUpdatedDto.dto |] ]

    [<Theory>]
    [<MemberData(nameof nonSnapshotMemberDtos)>]
    let ``it not filter out `` dto =
        let testcase = dto
        let actual = filterRetroEvent testcase
        let expected = Some(dto)

        Expect.equal actual expected "should equal"
