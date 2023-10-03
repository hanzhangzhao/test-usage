module Usage.Dto.Events.Dto

open Feeds.Dto.Models
open DependentAdded
open DependentProfileUpdated
open MemberCancelled
open MemberCobUpdated
open MemberEnrolled
open MemberProfileUpdate
open MemberTaxProvinceUpdate
open MemberTerminated
open DependentTerminated
open SpouseProfileUpdated
open SpouseTerminated
open MemberIncomeUpdate
open MemberEmploymentUpdate
open DependentDisabilityUpdated
open DependentPostSecondaryEducationUpdated
open MemberBenefitClassTransferred
open Usage.Dto.Events.BenefitPeriod
open Usage.Dto.Events.SpouseCohabUpdated
open Usage.Dto.Events.MemberReinstatementConfirmed

let private mergeCerts (bps: BenefitPeriodDto list) =
    bps
    |> List.map (fun bp -> bp.Certificate.CertificateNumber)
    |> List.distinct
    |> (String.concat ";")

type MemberEventDto =
    | MemberEnrollmentDto of MemberEnrollmentDto
    | MemberDependentAddedDto of MemberDependentAddedDto
    | MemberSpouseAddedDto of MemberSpouseAddedDto
    | MemberEmploymentUpdatedDto of MemberEmploymentUpdatedDto
    | MemberIncomeUpdatedDto of MemberIncomeUpdatedDto
    | MemberTerminatedDto of MemberTerminatedDto
    | MemberCancelledDto of MemberCancelledDto
    | DependentTerminatedDto of DependentTerminatedDto
    | SpouseTerminatedDto of SpouseTerminatedDto
    | MemberCobUpdatedDto of MemberCobUpdatedDto
    | DependentCobUpdatedDto of DependentCobUpdatedDto
    | SpouseCobUpdatedDto of SpouseCobUpdatedDto
    | MemberProfileUpdatedDto of MemberProfileUpdatedDto
    | MemberTaxProvinceUpdatedDto of MemberTaxProvinceUpdatedDto
    | SpouseProfileUpdatedDto of SpouseProfileUpdatedDto
    | DependentProfileUpdatedDto of DependentProfileUpdatedDto
    | SpouseCohabUpdatedDto of SpouseCohabUpdatedDto
    | DependentDisabilityUpdatedDto of DependentDisabilityUpdatedDto
    | DependentPostSecondaryEducationUpdatedDto of DependentPostSecondaryEducationUpdatedDto
    | MemberBenefitClassTransferredDto of MemberBenefitClassTransferredDto
    | MemberReinstatementConfirmedDto of MemberReinstatementConfirmedDto

    member this.MemberId =
        match this with
        | MemberEnrollmentDto dto -> dto.MemberId
        | MemberDependentAddedDto dto -> dto.MemberId
        | MemberSpouseAddedDto dto -> dto.MemberId
        | MemberEmploymentUpdatedDto dto -> dto.MemberId
        | MemberIncomeUpdatedDto dto -> dto.MemberId
        | MemberTerminatedDto dto -> dto.MemberId
        | MemberCancelledDto dto -> dto.MemberId
        | DependentTerminatedDto dto -> dto.MemberId
        | SpouseTerminatedDto dto -> dto.MemberId
        | MemberCobUpdatedDto dto -> dto.MemberId
        | DependentCobUpdatedDto dto -> dto.MemberId
        | SpouseCobUpdatedDto dto -> dto.MemberId
        | MemberProfileUpdatedDto dto -> dto.MemberId
        | MemberTaxProvinceUpdatedDto dto -> dto.MemberId
        | SpouseProfileUpdatedDto dto -> dto.MemberId
        | DependentProfileUpdatedDto dto -> dto.MemberId
        | SpouseCohabUpdatedDto dto -> dto.MemberId
        | DependentDisabilityUpdatedDto dto -> dto.Member.MemberId
        | DependentPostSecondaryEducationUpdatedDto dto -> dto.Member.MemberId
        | MemberBenefitClassTransferredDto dto -> dto.MemberId
        | MemberReinstatementConfirmedDto dto -> dto.MemberId

    member this.Cert =
        match this with
        | MemberEnrollmentDto dto -> dto.CertificateNumber
        | MemberDependentAddedDto dto -> mergeCerts dto.BenefitPeriods
        | MemberSpouseAddedDto dto -> mergeCerts dto.BenefitPeriods
        | MemberEmploymentUpdatedDto dto -> dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | MemberIncomeUpdatedDto dto -> dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | MemberTerminatedDto dto -> dto.CertificateNumber
        | MemberCancelledDto dto -> dto.CertificateNumber
        | DependentTerminatedDto dto -> mergeCerts dto.BenefitPeriods
        | SpouseTerminatedDto dto -> mergeCerts dto.BenefitPeriods
        | MemberCobUpdatedDto dto -> mergeCerts dto.BenefitPeriods
        | DependentCobUpdatedDto dto -> mergeCerts dto.BenefitPeriods
        | SpouseCobUpdatedDto dto -> mergeCerts dto.BenefitPeriods
        | MemberProfileUpdatedDto dto -> dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | MemberTaxProvinceUpdatedDto dto -> dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | SpouseProfileUpdatedDto dto -> mergeCerts dto.BenefitPeriods
        | DependentProfileUpdatedDto dto -> mergeCerts dto.BenefitPeriods
        | SpouseCohabUpdatedDto dto -> mergeCerts dto.BenefitPeriods
        | DependentDisabilityUpdatedDto dto -> dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | DependentPostSecondaryEducationUpdatedDto dto -> dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | MemberBenefitClassTransferredDto dto ->
            [ dto.From.Certificate.CertificateNumber; dto.To.Certificate.CertificateNumber ]
            |> List.distinct
            |> String.concat ";"
        | MemberReinstatementConfirmedDto dto -> dto.CertificateNumber

    member this.EventName =
        match this with
        | MemberEnrollmentDto _ -> "MemberEnrollmentConfirmed"
        | MemberDependentAddedDto _ -> "MemberDependentAdded"
        | MemberSpouseAddedDto _ -> "MemberSpouseAdded"
        | MemberEmploymentUpdatedDto _ -> "EnrolledMemberEmploymentUpdated"
        | MemberIncomeUpdatedDto _ -> "EnrolledMemberIncomeUpdated"
        | MemberTerminatedDto _ -> "MemberBenefitEnded"
        | MemberCancelledDto _ -> "MemberBenefitCancelled"
        | SpouseTerminatedDto _ -> "EnrolledSpouseRemoved"
        | DependentTerminatedDto _ -> "EnrolledDependentRemoved"
        | MemberCobUpdatedDto _ -> "MemberCobUpdated"
        | DependentCobUpdatedDto _ -> "DependentCobUpdated"
        | SpouseCobUpdatedDto _ -> "SpouseCobUpdated"
        | MemberProfileUpdatedDto _ -> "EnrolledMemberUpdated"
        | MemberTaxProvinceUpdatedDto _ -> "EnrolledMemberTaxProvinceUpdated"
        | SpouseProfileUpdatedDto _ -> "SpouseProfileUpdated"
        | DependentProfileUpdatedDto _ -> "DependentProfileUpdated"
        | SpouseCohabUpdatedDto _ -> "MemberSpouseCohabUpdated"
        | DependentDisabilityUpdatedDto _ -> "EnrolledDependentDisabilityUpdated"
        | DependentPostSecondaryEducationUpdatedDto _ -> "EnrolledDependentPostSecondaryEducationUpdated"
        | MemberBenefitClassTransferredDto _ -> "MemberBenefitClassTransferred"
        | MemberReinstatementConfirmedDto _ -> "MemberReinstatementConfirmed"
