module CertificateUsage.Dto.Events.Dto

open CertificateUsage.Dto.Events
open CertificateUsage.Dto.Events.MemberRateChanged
open MemberReinstatementConfirmed
open MemberEnrolled
open MemberCancelled
open MemberTerminated
open MemberEnrolledSnapshot
open MemberTaxProvinceUpdate
open DependentAdded
open BenefitPeriod
open SpouseTerminated
open SpouseCohabUpdated
open MemberCobUpdated
open DependentTerminated
open DependentPostSecondaryEducationUpdated
open DependentDisabilityUpdated
open MemberEmploymentUpdate
open MemberIncomeUpdate
open CarrierRateModified
open CertificateUsage.Domain

type MemberEventDto =
    | MemberEnrollmentDto of MemberEnrollmentDto
    | MemberReinstatementConfirmed of MemberReinstatementConfirmedDto
    | MemberTerminatedDto of MemberTerminatedDto
    | MemberCancelledDto of MemberCancelledDto
    | MemberTaxProvinceUpdatedDto of MemberTaxProvinceUpdatedDto
    | MemberSpouseAddedDto of MemberSpouseAddedDto
    | MemberDependentAddedDto of MemberDependentAddedDto
    | SpouseTerminatedDto of SpouseTerminatedDto
    | SpouseCohabUpdatedDto of SpouseCohabUpdatedDto
    | MemberCobUpdatedDto of MemberCobUpdatedDto
    | DependentCobUpdatedDto of DependentCobUpdatedDto
    | SpouseCobUpdatedDto of SpouseCobUpdatedDto
    | DependentTerminatedDto of DependentTerminatedDto
    | DependentPostSecondaryEducationUpdatedDto of DependentPostSecondaryEducationUpdatedDto
    | DependentDisabilityUpdatedDto of DependentDisabilityUpdatedDto
    | MemberEmploymentUpdatedDto of MemberEmploymentUpdatedDto
    | MemberIncomeUpdatedDto of MemberIncomeUpdatedDto
    | MemberEnrolledSnapshotDto of MemberEnrolledSnapshotDto
    | MemberRateChangedDto of MemberRateChangedDto

    member this.Cert =
        match this with
        | MemberEnrollmentDto dto -> Some dto.CertificateNumber
        | MemberReinstatementConfirmed dto -> Some dto.CertificateNumber
        | MemberTerminatedDto dto -> Some dto.CertificateNumber
        | MemberCancelledDto dto -> Some dto.CertificateNumber
        | MemberEnrolledSnapshotDto dto -> Some dto.CertificateNumber
        | MemberTaxProvinceUpdatedDto dto -> Some dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | MemberDependentAddedDto dto ->
            (getActiveBenefitPeriod dto.BenefitPeriods).Certificate.CertificateNumber
            |> Some
        | MemberSpouseAddedDto dto ->
            (getActiveBenefitPeriod dto.BenefitPeriods).Certificate.CertificateNumber
            |> Some
        | DependentTerminatedDto dto ->
            (getActiveBenefitPeriod dto.BenefitPeriods).Certificate.CertificateNumber
            |> Some
        | SpouseTerminatedDto dto ->
            (getActiveBenefitPeriod dto.BenefitPeriods).Certificate.CertificateNumber
            |> Some
        | MemberCobUpdatedDto dto ->
            (getActiveBenefitPeriod dto.BenefitPeriods).Certificate.CertificateNumber
            |> Some
        | DependentCobUpdatedDto dto ->
            (getActiveBenefitPeriod dto.BenefitPeriods).Certificate.CertificateNumber
            |> Some
        | SpouseCobUpdatedDto dto ->
            (getActiveBenefitPeriod dto.BenefitPeriods).Certificate.CertificateNumber
            |> Some
        | SpouseCohabUpdatedDto dto ->
            (getActiveBenefitPeriod dto.BenefitPeriods).Certificate.CertificateNumber
            |> Some
        | DependentDisabilityUpdatedDto dto -> Some dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | DependentPostSecondaryEducationUpdatedDto dto -> Some dto.ActiveBenefitPeriod.Certificate.CertificateNumber
        | MemberEmploymentUpdatedDto dto -> dto.ActiveBenefitPeriod.Certificate.CertificateNumber |> Some
        | MemberIncomeUpdatedDto dto -> dto.ActiveBenefitPeriod.Certificate.CertificateNumber |> Some
        | MemberRateChangedDto dto -> Some dto.CertificateNumber

type RateEventDto =
    | CarrierRateModifiedDto of CarrierRateModifiedDto

    member this.Carrier =
        match this with
        | CarrierRateModifiedDto dto -> dto.Carrier

    member this.PolicyNumber =
        match this with
        | CarrierRateModifiedDto dto -> dto.PolicyNumber

    member this.Option =
        match this with
        | CarrierRateModifiedDto dto -> dto.Option

    member this.Coverage =
        match this with
        | CarrierRateModifiedDto dto -> dto.Coverage |> Option.defaultValue ""

    member this.ProductLine =
        match this with
        | CarrierRateModifiedDto dto -> dto.ProductLine

module RetroactiveCertificateUpdateType =
    let fromDto (dto : MemberEventDto) =
        match dto with
        | MemberEnrollmentDto _ -> RetroactiveCertificateUpdateType.Enrollment
        | MemberCancelledDto _
        | MemberTerminatedDto _ -> RetroactiveCertificateUpdateType.Termination
        | _ -> RetroactiveCertificateUpdateType.Update
