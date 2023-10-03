module CertificateUsage.ToDomain

open System

open FsToolkit.ErrorHandling

open CertificateUsage.Dao
open CertificateUsage.Errors
open CertificateUsage.Domain
open CertificateUsage.Dto.Events.MemberRateChanged
open CertificateUsage.Dto.Events.BenefitPeriod
open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.MemberEnrolled
open CertificateUsage.Dto.Events.MemberCancelled
open CertificateUsage.Dto.Events.MemberTerminated
open CertificateUsage.Dto.Events.PlanSelection
open CertificateUsage.Dto.Events.Client
open CertificateUsage.Dto.Events.MemberTaxProvinceUpdate
open CertificateUsage.Dto.Events.SpouseTerminated
open CertificateUsage.Dto.Events.SpouseCohabUpdated
open CertificateUsage.Dto.Events.MemberCobUpdated
open CertificateUsage.Dto.Events.DependentTerminated
open CertificateUsage.Dto.Events.DependentPostSecondaryEducationUpdated
open CertificateUsage.Dto.Events.DependentDisabilityUpdated
open CertificateUsage.Dto.Events.EligiblityPeriod
open CertificateUsage.Dto.Events.MemberEmploymentUpdate
open CertificateUsage.Dto.Events.MemberIncomeUpdate
open CertificateUsage.Dto.Events.Update
open CertificateUsage.Dto.Events.DependentAdded
open CertificateUsage.Dto.Events.MemberEnrolledSnapshot
open CertificateUsage.Dto.Events.CarrierRateModified

let getActiveEligibilityPeriod (eligibilityPeriods : EligibilityPeriodDto list) memberBenefitPeriodId dependentId =
    eligibilityPeriods
    |> List.tryFind (fun ep -> ep.EmployeeBenefitPeriodId = memberBenefitPeriodId)
    |> function
        | None -> Error(MissingEligibilityPeriods("eligibility periods", dependentId))
        | Some ep -> Ok ep

let getEffectiveDate (dependentId : int) (periods : EligibilityPeriodDto list) =
    match periods with
    | [] -> Error(MissingEligibilityPeriods("effective date", dependentId))
    | periods ->
        periods
        |> List.sortByDescending (fun p -> p.EligibilityStartDate)
        |> (fun ps -> Ok ps.Head.EligibilityStartDate)

let division (carrierMapping : CarrierMapping option) (carrierClientCode : string option) =
    carrierMapping
    |> Option.bind tryFindDivisionCode
    |> Option.map Ok
    |> Option.defaultValue (
        carrierClientCode
        |> Option.map Ok
        |> Option.defaultValue (Error(RequiredField "division"))
    )

let clientName (name : string option) =
    name
    |> Option.map Ok
    |> Option.defaultValue (Error(RequiredField "Client.Name"))

let getRequiredValue name value =
    value |> Option.map Ok |> Option.defaultValue (Error(RequiredField name))

let getCertificateNumber = getRequiredValue "CertificateNumber"
let getPolicyNumber = getRequiredValue "PolicyNumber"

let parseDecimal (s : string) =
    match s with
    | s when String.IsNullOrWhiteSpace(s) -> 0.0m
    | s -> Decimal.Parse(s)

let toPlanSelection (dto : PlanSelectionDto) =
    { ProductLine = dto.ProductLine
      ProductLineGroup = dto.LineGroup
      Coverage = dto.Coverage
      Option = dto.Selection
      RatePer = dto.PricePer
      Volume =
        { Amount = dto.Volume.Amount
          Unit = dto.Volume.Units }
      CarrierRate = dto.CarrierRate |> parseDecimal
      TaxRate = dto.TaxRate |> parseDecimal
      TaxProvince = dto.TaxProvince }

let memberEnrollmentDtoToDomain (dto : MemberEnrollmentDto) =
    validation {
        let! division' = division dto.CarrierMapping dto.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! externalPolicyNumber = dto.ExternalPolicyNumber |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection

        return
            { CoveredCertificate.CertificateNumber = dto.CertificateNumber
              Carrier = dto.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = dto.PolicyNumber
              Effective = dto.BenefitsStartDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let memberReinstatementConfirmed = memberEnrollmentDtoToDomain

let memberTerminatedToDomain (dto : MemberTerminatedDto) =
    validation {
        let! benefitsEndedDate =
            match dto.BenefitsEndedDate with
            | Some d -> Ok d
            | None -> Error(RequiredField "BenefitsEndedDate")

        and! division' = division dto.CarrierMapping dto.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! externalPolicyNumber = dto.ExternalPolicyNumber |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection

        return
            { CertificateNumber = dto.CertificateNumber
              Division = division'
              Carrier = dto.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = dto.PolicyNumber
              Effective = benefitsEndedDate
              PlanSelections = planSelections }
            |> ExclusionEvent
    }

let memberCancelledToDomain (dto : MemberCancelledDto) =
    validation {
        let! benefitsEndedDate =
            match dto.BenefitsEndedDate with
            | Some d -> Ok d
            | None -> Error(RequiredField "BenefitsEndedDate")

        and! division' = division dto.CarrierMapping dto.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! externalPolicyNumber = dto.ExternalPolicyNumber |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection

        return
            { CertificateNumber = dto.CertificateNumber
              Division = division'
              Carrier = dto.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = dto.PolicyNumber
              Effective = benefitsEndedDate
              PlanSelections = planSelections }
            |> ExclusionEvent
    }

let memberEnrolledSnapshotToDomain (dto : MemberEnrolledSnapshotDto) =
    validation {
        let! division' = division dto.CarrierMapping dto.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! externalPolicyNumber = dto.ExternalPolicyNumber |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection

        return
            { CoveredCertificate.CertificateNumber = dto.CertificateNumber
              Carrier = dto.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = dto.PolicyNumber
              Effective = dto.BenefitsStartDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let memberTaxProvinceUpdateToDomain (dto : MemberTaxProvinceUpdatedDto) =
    validation {
        let! division' =
            division
                dto.ActiveBenefitPeriod.CarrierMapping
                dto.ActiveBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (MemberTaxProvinceUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            dto.ActiveBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let effectiveDate = dto.Update.Member.EffectiveDate

        let planSelections = dto.PlanSelections |> List.map toPlanSelection

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = dto.ActiveBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = dto.ActiveBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let memberDependentAddedToDomain (dto : MemberDependentAddedDto) =
    validation {
        let activeBenefitPeriod = getActiveBenefitPeriod dto.BenefitPeriods

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (MemberDependentAddedDto dto).Cert |> getCertificateNumber

        and! effectiveDate = getEffectiveDate dto.Dependent.Id dto.Dependent.EligibilityPeriods

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let memberSpouseTerminatedToDomain (dto : SpouseTerminatedDto) =
    validation {
        let activeBenefitPeriod = getActiveBenefitPeriod dto.BenefitPeriods

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (SpouseTerminatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection
        let effectiveDate = dto.EffectiveEndDate

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let memberSpouseAddedToDomain (dto : MemberSpouseAddedDto) =
    validation {
        let activeBenefitPeriod = getActiveBenefitPeriod dto.BenefitPeriods

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (MemberSpouseAddedDto dto).Cert |> getCertificateNumber

        and! effectiveDate = getEffectiveDate dto.Spouse.Id dto.Spouse.EligibilityPeriods

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let memberDependentTerminatedToDomain (dto : DependentTerminatedDto) =
    validation {
        let activeBenefitPeriod = getActiveBenefitPeriod dto.BenefitPeriods

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (DependentTerminatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection
        let effectiveDate = dto.EffectiveEndDate

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let memberCobUpdatedToDomain (dto : MemberCobUpdatedDto) =
    validation {
        let activeBenefitPeriod = getActiveBenefitPeriod dto.BenefitPeriods

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (MemberCobUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection
        let effectiveDate = dto.CoverageCoordination.EffectiveDate

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let dependentCobUpdatedToDomain (dto : DependentCobUpdatedDto) =
    validation {
        let activeBenefitPeriod = getActiveBenefitPeriod dto.BenefitPeriods

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (DependentCobUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection
        let effectiveDate = dto.Dependent.CoverageCoordination.EffectiveDate

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let spouseCobUpdatedToDomain (dto : SpouseCobUpdatedDto) =
    validation {
        let activeBenefitPeriod = getActiveBenefitPeriod dto.BenefitPeriods

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name
        and! cert = (SpouseCobUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection
        let effectiveDate = dto.Spouse.CoverageCoordination.EffectiveDate

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let spouseCohabUpdatedToDomain (dto : SpouseCohabUpdatedDto) =
    validation {
        let activeBenefitPeriod = getActiveBenefitPeriod dto.BenefitPeriods

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (SpouseCohabUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection
        let effectiveDate = dto.Spouse.CohabitationDate |> Option.defaultValue DateTime.Now

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let dependentPostSecondaryEducationUpdatedToDomain (dto : DependentPostSecondaryEducationUpdatedDto) =
    validation {
        let activeBenefitPeriod = dto.ActiveBenefitPeriod

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! activeEligibilityPeriod =
            getActiveEligibilityPeriod dto.Dependent.EligibilityPeriods activeBenefitPeriod.Id dto.Dependent.Id

        and! clientName' = clientName dto.Client.Name

        and! cert = (DependentPostSecondaryEducationUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.Member.PlanSelections |> List.map toPlanSelection
        let effectiveDate = activeEligibilityPeriod.EligibilityStartDate

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let dependentDisabilityUpdatedToDomain (dto : DependentDisabilityUpdatedDto) =
    validation {
        let activeBenefitPeriod = dto.ActiveBenefitPeriod

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (DependentDisabilityUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.Member.PlanSelections |> List.map toPlanSelection
        let dateOfDisability = dto.Dependent.DateOfDisability

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = dateOfDisability
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let employmentChangeToDomain (dto : MemberEmploymentUpdatedDto) (change : UpdateDto) =
    validation {
        let activeBenefitPeriod = dto.ActiveBenefitPeriod

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (MemberEmploymentUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection
        let effectiveDate = change.UpdatedDate.ToLocalTime()

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let memberEmploymentUpdateToDomain (dto : MemberEmploymentUpdatedDto) =
    validation {
        return!
            dto.Update.Changes
            |> List.map (fun update -> employmentChangeToDomain dto update)
            |> List.traverseValidationA id
    }

let memberIncomeUpdateToDomain (dto : MemberIncomeUpdatedDto) =
    validation {
        let activeBenefitPeriod = dto.ActiveBenefitPeriod

        let! division' =
            division activeBenefitPeriod.CarrierMapping activeBenefitPeriod.ProductConfiguration.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! cert = (MemberIncomeUpdatedDto dto).Cert |> getCertificateNumber

        and! externalPolicyNumber =
            activeBenefitPeriod.Certificate.PolicyNumber.ExternalPolicyNumber
            |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection
        let effectiveDate = dto.Update.Income.IncomeEffectiveDate

        return
            { CoveredCertificate.CertificateNumber = cert
              Carrier = activeBenefitPeriod.ProductConfiguration.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = activeBenefitPeriod.Certificate.PolicyNumber.Number
              Effective = effectiveDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let carrierRateModifiedDtoToDomain
    (dto : CarrierRateModifiedDto)
    : Validation<CarrierRate.CarrierRateModification, Errors> =
    validation {
        let carrierRate = dto.CarrierRate |> Decimal.Parse

        return
            { CarrierRate.CarrierRateModification.Carrier = dto.Carrier
              PolicyNumber = dto.PolicyNumber
              Option = dto.Option
              Coverage = dto.Coverage
              ProductLine = dto.ProductLine
              Effective = dto.EffectiveDate
              CarrierRate = carrierRate
              ChangedBy =
                { Id = dto.ChangedBy.Id
                  Name = dto.ChangedBy.Name } }
    }

let memberRateChangedDtoToDomain (dto : MemberRateChangedDto) =
    validation {
        let! division' = division dto.CarrierMapping dto.CarrierClientCode

        and! clientName' = clientName dto.Client.Name

        and! externalPolicyNumber = dto.ExternalPolicyNumber |> getPolicyNumber

        let planSelections = dto.PlanSelections |> List.map toPlanSelection

        return
            { CoveredCertificate.CertificateNumber = dto.CertificateNumber
              Carrier = dto.Carrier
              ClientName = clientName'
              PolicyNumber = externalPolicyNumber
              ScbPolicyNumber = dto.PolicyNumber
              Effective = dto.BenefitsStartDate
              Division = division'
              PlanSelections = planSelections }
            |> CoveredEvent
    }

let toList domain =
    domain |> Validation.map (fun domain -> [ domain ])

let toDomain (dto : MemberEventDto) =
    match dto with
    | MemberEnrollmentDto dto -> memberEnrollmentDtoToDomain dto |> toList
    | MemberReinstatementConfirmed dto -> memberReinstatementConfirmed dto |> toList
    | MemberTerminatedDto dto -> memberTerminatedToDomain dto |> toList
    | MemberCancelledDto dto -> memberCancelledToDomain dto |> toList
    | MemberEnrolledSnapshotDto dto -> memberEnrolledSnapshotToDomain dto |> toList
    | MemberTaxProvinceUpdatedDto dto -> dto |> memberTaxProvinceUpdateToDomain |> toList
    | MemberDependentAddedDto dto -> dto |> memberDependentAddedToDomain |> toList
    | MemberSpouseAddedDto dto -> dto |> memberSpouseAddedToDomain |> toList
    | SpouseTerminatedDto dto -> dto |> memberSpouseTerminatedToDomain |> toList
    | DependentTerminatedDto dto -> dto |> memberDependentTerminatedToDomain |> toList
    | MemberCobUpdatedDto dto -> dto |> memberCobUpdatedToDomain |> toList
    | DependentCobUpdatedDto dto -> dto |> dependentCobUpdatedToDomain |> toList
    | SpouseCobUpdatedDto dto -> dto |> spouseCobUpdatedToDomain |> toList
    | SpouseCohabUpdatedDto dto -> dto |> spouseCohabUpdatedToDomain |> toList
    | DependentPostSecondaryEducationUpdatedDto dto -> dto |> dependentPostSecondaryEducationUpdatedToDomain |> toList
    | DependentDisabilityUpdatedDto dto -> dto |> dependentDisabilityUpdatedToDomain |> toList
    | MemberEmploymentUpdatedDto dto -> memberEmploymentUpdateToDomain dto
    | MemberIncomeUpdatedDto dto -> dto |> memberIncomeUpdateToDomain |> toList
    | MemberRateChangedDto dto -> dto |> memberRateChangedDtoToDomain |> toList

let fromDaoToPlanSelection (dao : PlanSelectionDao) : PlanSelection =
    { Domain.PlanSelection.ProductLine = dao.ProductLine
      ProductLineGroup = dao.ProductLineGroup
      Coverage = dao.Coverage
      Option = dao.Option
      RatePer = dao.RatePer
      Volume =
        { Amount = dao.Volume.Amount
          Unit = dao.Volume.Unit }
      CarrierRate = dao.CarrierRate
      TaxRate = dao.TaxRate
      TaxProvince = dao.TaxProvince }

let fromDaoToCoveredEvent (dao : CertificateUsageChangeDao) : CertificateUsage =
    let planSelections =
        dao.CoverageData.PlanSelections |> List.map fromDaoToPlanSelection

    { CoveredCertificate.CertificateNumber = dao.CoverageData.CertificateNumber
      Carrier = dao.CoverageData.Carrier
      ClientName = dao.CoverageData.ClientName
      ScbPolicyNumber = dao.ScbPolicyNumber
      PolicyNumber = dao.CoverageData.PolicyNumber
      Effective = dao.CoverageData.Effective
      Division = dao.CoverageData.Division
      PlanSelections = planSelections }
    |> CoveredEvent

let fromDaoToExcludedEvent (dao : CertificateUsageChangeDao) : CertificateUsage =
    let planSelections =
        dao.CoverageData.PlanSelections |> List.map fromDaoToPlanSelection

    { ExcludedCertificate.CertificateNumber = dao.CoverageData.CertificateNumber
      Carrier = dao.CoverageData.Carrier
      ClientName = dao.CoverageData.ClientName
      ScbPolicyNumber = dao.ScbPolicyNumber
      PolicyNumber = dao.CoverageData.PolicyNumber
      Effective = dao.CoverageData.Effective
      Division = dao.CoverageData.Division
      PlanSelections = planSelections }
    |> ExclusionEvent

let fromDao (dao : CertificateUsageChangeDao) : CertificateUsage =
    let usageType = dao.Type

    match usageType with
    | CoverageType.Covered -> fromDaoToCoveredEvent dao
    | CoverageType.Excluded -> fromDaoToExcludedEvent dao
    | _ -> raise (InvalidCoverageTypeEnumException($"{usageType.ToString()}"))

module Rate =
    let toDomain (dto : RateEventDto) =
        match dto with
        | CarrierRateModifiedDto dto -> dto |> carrierRateModifiedDtoToDomain |> toList

    let fromDao (dao : RateUpdateDataDao) : CarrierRate.CarrierRateModification =
        { CarrierRate.CarrierRateModification.Carrier = dao.Carrier
          PolicyNumber = dao.PolicyNumber
          Option = dao.Option
          Coverage = dao.Coverage
          ProductLine = dao.ProductLine
          Effective = dao.Effective
          CarrierRate = dao.RateUpdateData.CarrierRate
          ChangedBy =
            { Id = dao.RateUpdateData.ChangedBy.Id
              Name = dao.RateUpdateData.ChangedBy.Name } }
