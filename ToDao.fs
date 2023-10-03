module CertificateUsage.ToDao

open System

open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Domain
open CertificateUsage.Dao

let volumeToDao (volume : Volume) =
    { Amount = volume.Amount
      Unit = volume.Unit }

let planSelectionToWriteModel (planSelection : PlanSelection) =
    { PlanSelectionDao.ProductLine = planSelection.ProductLine
      PlanSelectionDao.ProductLineGroup = planSelection.ProductLineGroup
      Coverage = planSelection.Coverage
      Option = planSelection.Option
      RatePer = planSelection.RatePer
      Volume = volumeToDao planSelection.Volume
      CarrierRate = planSelection.CarrierRate
      TaxRate = planSelection.TaxRate
      TaxProvince = planSelection.TaxProvince }

let metadataToDao (metadata : MetadataDto) =
    { EventId = metadata.EventId
      EventNo = metadata.EventNumber
      EventType = metadata.EventType
      EventDate = metadata.CreateDate
      EventVersion = metadata.Version
      EventStream = metadata.StreamId }

let toCoveredCertificateDao (metadata : MetadataDto) (domain : CoveredCertificate) =
    let planSelections = domain.PlanSelections |> List.map planSelectionToWriteModel

    { CertificateNumber = domain.CertificateNumber
      Carrier = domain.Carrier
      ScbPolicyNumber = domain.ScbPolicyNumber
      PolicyNumber = domain.PolicyNumber
      Effective = domain.Effective
      Type = CoverageType.Covered
      EventMetadata = metadataToDao metadata
      CoverageData =
        { CertificateEventDao.CertificateNumber = domain.CertificateNumber
          Carrier = domain.Carrier
          ClientName = domain.ClientName
          PolicyNumber = domain.PolicyNumber
          Effective = domain.Effective
          Division = domain.Division
          PlanSelections = planSelections } }

let toExcludedCertificateDao (metadata : MetadataDto) (domain : ExcludedCertificate) =
    let planSelections = domain.PlanSelections |> List.map planSelectionToWriteModel

    { CertificateNumber = domain.CertificateNumber
      Carrier = domain.Carrier
      ScbPolicyNumber = domain.ScbPolicyNumber
      PolicyNumber = domain.PolicyNumber
      Effective = domain.Effective
      Type = CoverageType.Excluded
      EventMetadata = metadataToDao metadata
      CoverageData =
        { CertificateEventDao.CertificateNumber = domain.CertificateNumber
          Carrier = domain.Carrier
          ClientName = domain.ClientName
          PolicyNumber = domain.PolicyNumber
          Effective = domain.Effective
          Division = domain.Division
          PlanSelections = planSelections } }

let toDao (metadata : MetadataDto) (domain : CertificateUsage) =
    match domain with
    | CoveredEvent coveredEvent -> toCoveredCertificateDao metadata coveredEvent
    | ExclusionEvent excludedCertificate -> toExcludedCertificateDao metadata excludedCertificate

module Certificate =
    let certificateStatusFromDomain (status : Domain.CertificateStatus) =
        match status with
        | Active -> CertificateStatus.Active
        | Terminated -> CertificateStatus.Terminated

    let fromDomain (domain : Certificate) : CertificateRecordDao =
        { CertificateRecordDao.CertificateNumber = CertificateNumber.value domain.CertificateNumber
          Carrier = CarrierName.value domain.CarrierName
          PolicyNumber = PolicyNumber.value domain.PolicyNumber
          ClientName = ClientName.value domain.ClientName
          Status = certificateStatusFromDomain domain.CertificateStatus
          Certificate =
            { CertificateNumber = CertificateNumber.value domain.CertificateNumber
              CarrierName = CarrierName.value domain.CarrierName
              PolicyNumber = PolicyNumber.value domain.PolicyNumber
              ScbPolicyNumber = PolicyNumber.value domain.ScbPolicyNumber
              ClientName = ClientName.value domain.ClientName
              CertificateStatus = domain.CertificateStatus.toString
              StartDate = domain.StartDate
              EndDate = domain.EndDate
              Division = Division.value domain.Division
              PlanSelections = List.map planSelectionToWriteModel domain.PlanSelections } }

module Rate =
    let toDao (metadata : MetadataDto) (domain : CarrierRate.CarrierRateModification) : RateUpdateDataDao =
        { RateUpdateDataDao.Carrier = domain.Carrier
          PolicyNumber = domain.PolicyNumber
          Option = domain.Option
          Coverage = domain.Coverage
          ProductLine = domain.ProductLine
          Effective = domain.Effective
          EventMetadata = metadataToDao metadata
          RateUpdateData =
            { RateUpdateData.Carrier = domain.Carrier
              PolicyNumber = domain.PolicyNumber
              Option = domain.Option
              Coverage = domain.Coverage
              ProductLine = domain.ProductLine
              Effective = domain.Effective
              CarrierRate = domain.CarrierRate
              ChangedBy =
                { Id = domain.ChangedBy.Id
                  Name = domain.ChangedBy.Name } } }

module RetroactiveCertificateUpdate =
    let fromRetroactiveCertificateUpdate
        (retroactiveUpdate : RetroactiveCertificateUpdate)
        : RetroactiveCertificateUpdateDao =
        { RetroactiveCertificateUpdateDao.Type = RetroactiveCertificateUpdateType.fromDomain retroactiveUpdate.Type
          CertificateNumber = CertificateNumber.value retroactiveUpdate.CertificateNumber
          CarrierName = CarrierName.value retroactiveUpdate.CarrierName
          ClientName = ClientName.value retroactiveUpdate.ClientName
          PolicyNumber = PolicyNumber.value retroactiveUpdate.PolicyNumber
          ProductLine = ProductLine.value retroactiveUpdate.ProductLine
          Option = BenefitOption.value retroactiveUpdate.Option
          Coverage = Option.map Coverage.value retroactiveUpdate.Coverage
          UpdateDate = retroactiveUpdate.UpdateDate
          Backdate = retroactiveUpdate.Backdate }

module Correction =
    let private fromCorrectionUsageDomain
        (id : Guid)
        (billingEndDate : DateTime)
        usageId
        retroactiveUpdateId
        (correction : Usage)
        : CertificateUsageDao =

        { Id = id
          CertificateUsageDao.CorrelatedUsageId = usageId
          CausationId = retroactiveUpdateId
          UsageType = CertificateUsageType.Correction
          CertificateNumber = CertificateNumber.value correction.CertificateNumber
          CarrierName = CarrierName.value correction.CarrierName
          ClientName = ClientName.value correction.ClientName
          PolicyNumber = PolicyNumber.value correction.PolicyNumber
          ScbPolicyNumber = PolicyNumber.value correction.ScbPolicyNumber
          BenefitStartDate = correction.BenefitStartDate
          BenefitEndDate = correction.BenefitEndDate
          Division = Division.value correction.Division
          ProductLine = ProductLine.value correction.ProductLine
          ProductLineGroup = ProductLineGroup.value correction.ProductLineGroup
          Coverage = Option.map Coverage.value correction.Coverage
          Option = BenefitOption.value correction.Option
          RatePer = RatePer.value correction.RatePer
          VolumeAmount = VolumeAmount.value correction.VolumeAmount
          VolumeUnit = VolumeUnit.value correction.VolumeUnit
          CarrierRate = Rate.value correction.CarrierRate
          TaxRate = Rate.value correction.TaxRate
          TaxProvince = TaxProvince.value correction.TaxProvince
          BillingEndDate = billingEndDate
          DateIncurred = correction.DateIncurred }

    let private fromReversalUsageDomain
        (id : Guid)
        (billingEndDate : DateTime)
        usageId
        retroactiveUpdateId
        (reversal : Usage)
        =
        { fromCorrectionUsageDomain id billingEndDate usageId retroactiveUpdateId reversal with
            UsageType = CertificateUsageType.Reversal }

    let fromCorrectionDomain
        (generateId : unit -> Guid)
        (billingEndDate : DateTime)
        (usageId : Guid option)
        (retroId : Guid)
        (correction : Correction)
        : CertificateUsageDao option list =

        let reversal =
            correction.Reversal
            |> Option.map (fromReversalUsageDomain (generateId ()) billingEndDate usageId retroId)

        let correction =
            correction.Correction
            |> Option.map (fromCorrectionUsageDomain (generateId ()) billingEndDate usageId retroId)

        [ reversal; correction ]


    let fromCorrectionDomainModel = fromCorrectionDomain Guid.NewGuid
