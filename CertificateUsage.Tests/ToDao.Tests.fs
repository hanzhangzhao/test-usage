module CertificateUsage.Tests.ToDao

open System
open CertificateUsage
open CertificateUsage.Dao
open CertificateUsage.Tests.Domain
open Xunit
open Expecto

open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Domain
open CertificateUsage.ToDao
open CertificateUsage.Tests

module Stubs =
    let benefitStartDate = DateTime(2023, 6, 14)
    let benefitsEndedDate = DateTime(2023, 07, 01)

    let taxProvince = "TaxProvince"
    let certificateNumber = "CertificateNumber"
    let scbPolicyNumber = "ScbPolicyNumber"
    let policyNumber = "PolicyNumber"
    let carrier = "Carrier"
    let division = "Division"
    let carrierMapping = Map [ "division", division ]
    let productLine = "ProductLine"
    let lineGroup = "LineGroup"
    let option = "Option"
    let coverage = "Coverage"
    let displayPricePer = 1.1m
    let volumeAmount = 1.2m
    let volumeUnits = "CAD"
    let clientName = "ClientName"
    let carrierRate = 0.8m
    let taxRate = 1.3m


    let coveredCertificate =
        { CoveredCertificate.CertificateNumber = certificateNumber
          Carrier = carrier
          ClientName = clientName
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = policyNumber
          Effective = benefitStartDate
          Division = division
          PlanSelections =
            [ { ProductLine = productLine
                ProductLineGroup = lineGroup
                Coverage = Some coverage
                Option = option
                RatePer = displayPricePer
                Volume =
                  { Amount = volumeAmount
                    Unit = volumeUnits }
                CarrierRate = carrierRate
                TaxRate = taxRate
                TaxProvince = taxProvince } ] }

    let excludedCertificate : ExcludedCertificate =
        { CertificateNumber = certificateNumber
          Carrier = carrier
          ClientName = clientName
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = policyNumber
          Division = division
          Effective = benefitsEndedDate
          PlanSelections = [] }

    let eventId = Guid.NewGuid()
    let eventType = "EventType"
    let version = "Version"
    let createDate = DateTime(2023, 06, 14)
    let eventNumber = 1UL
    let streamId = "StreamId"

    let metadataDto =
        { MetadataDto.EventId = eventId
          EventType = eventType
          Version = version
          CreateDate = createDate
          EventNumber = eventNumber
          StreamId = streamId }

    let coveredDao =
        { CertificateNumber = certificateNumber
          Carrier = carrier
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = policyNumber
          Effective = benefitStartDate
          Type = Dao.CoverageType.Covered
          CoverageData =
            { CertificateNumber = certificateNumber
              Carrier = carrier
              ClientName = clientName
              PolicyNumber = policyNumber
              Division = division
              Effective = benefitStartDate
              PlanSelections =
                [ { ProductLine = productLine
                    ProductLineGroup = lineGroup
                    Coverage = Some coverage
                    Option = option
                    RatePer = displayPricePer
                    Volume =
                      { Amount = volumeAmount
                        Unit = volumeUnits }
                    CarrierRate = carrierRate
                    TaxRate = taxRate
                    TaxProvince = taxProvince } ] }
          EventMetadata =
            { EventId = eventId
              EventNo = eventNumber
              EventType = eventType
              EventDate = createDate
              EventVersion = version
              EventStream = streamId } }

module ToCoveredCertificateDao =
    [<Fact>]
    let ``transforms a CoveredCertificate to a CertificateUsageChangeDao`` () =
        let domainModel = Stubs.coveredCertificate
        let metadata = Stubs.metadataDto
        let actual = toCoveredCertificateDao metadata domainModel
        let expected = Stubs.coveredDao

        Expect.equal actual expected ""

module ToExcludedCertificateWriteModel =
    [<Fact>]
    let ``should transform an ExcludedCertificate to a CertificateUsageChangeDao`` () =
        let domainModel = Stubs.excludedCertificate |> ExclusionEvent
        let metadata = Stubs.metadataDto
        let actual = toDao metadata domainModel

        let expected : CertificateUsageChangeDao =
            { CertificateNumber = Stubs.certificateNumber
              Carrier = Stubs.carrier
              ScbPolicyNumber = Stubs.scbPolicyNumber
              PolicyNumber = Stubs.policyNumber
              Effective = Stubs.benefitsEndedDate
              Type = CoverageType.Excluded
              CoverageData =
                { CertificateNumber = Stubs.certificateNumber
                  Carrier = Stubs.carrier
                  ClientName = Stubs.clientName
                  PolicyNumber = Stubs.policyNumber
                  Division = Stubs.division
                  Effective = Stubs.benefitsEndedDate
                  PlanSelections = [] }
              EventMetadata =
                { EventId = Stubs.eventId
                  EventNo = Stubs.eventNumber
                  EventType = Stubs.eventType
                  EventDate = Stubs.createDate
                  EventVersion = Stubs.version
                  EventStream = Stubs.streamId } }

        Expect.equal actual expected "should map successfully"

module CoverageType =
    [<Fact>]
    let ``should transform covered to correct CoverageType`` () =
        let expected = CoverageType.Covered
        let actual = CoverageType.fromString "covered"

        Expect.equal actual expected "should convert successfully"

    [<Fact>]
    let ``should transform excluded to correct CoverageType`` () =
        let expected = CoverageType.Excluded
        let actual = CoverageType.fromString "excluded"

        Expect.equal actual expected "should convert successfully"

[<Fact>]
let ``should error when unknown coverage`` () =
    Expect.throws (fun _ -> CoverageType.fromString "unknown" |> ignore) ""

module Rate =
    module ToDao =
        [<Fact>]
        let ``transforms a CarrierRateModification domain model to a dao`` () =
            let domainModel = Stubs.CarrierRateModificationDomainModel.domain

            let expected =
                { RateUpdateDataDao.Carrier = "Carrier"
                  PolicyNumber = "PolicyNumber"
                  Option = "Option"
                  Coverage = Some "Coverage"
                  ProductLine = "ProductLine"
                  Effective = Stubs.CarrierRateModificationDomainModel.effective
                  EventMetadata =
                    { EventId = Guid.Empty
                      EventNo = 0UL
                      EventType = "EventType"
                      EventDate = Stubs.MetadataDto.createDate
                      EventVersion = "Version"
                      EventStream = "StreamId" }
                  RateUpdateData =
                    { Carrier = "Carrier"
                      PolicyNumber = "PolicyNumber"
                      Option = "Option"
                      Coverage = Some "Coverage"
                      ProductLine = "ProductLine"
                      Effective = Stubs.CarrierRateModificationDomainModel.effective
                      CarrierRate = Stubs.CarrierRateModificationDomainModel.carrierRate
                      ChangedBy = { Id = 1; Name = "ChangedBy.Name" } } }

            let metadata = Stubs.MetadataDto.dto
            let actual = Rate.toDao metadata domainModel
            Expect.equal actual expected ""

module Certificate =
    module fromDomain =
        [<Fact>]
        let ``a certificate is transformed to a CertificateRecordDao`` () =
            let certificate = Stubs.Certificate.Domain.domain
            let actual = Certificate.fromDomain certificate

            let expected =
                { CertificateNumber = Stubs.Certificate.certificateNumber
                  Carrier = Stubs.Certificate.carrierName
                  PolicyNumber = Stubs.Certificate.policyNumber
                  ClientName = Stubs.Certificate.clientName
                  Status = Dao.CertificateStatus.Active
                  Certificate =
                    { CertificateNumber = Stubs.Certificate.certificateNumber
                      CarrierName = Stubs.Certificate.carrierName
                      ClientName = Stubs.Certificate.clientName
                      ScbPolicyNumber = Stubs.Certificate.scbPolicyNumber
                      PolicyNumber = Stubs.Certificate.policyNumber
                      StartDate = Stubs.Certificate.startDate
                      EndDate = Stubs.Certificate.endDate
                      Division = Stubs.Certificate.division
                      CertificateStatus = "active"
                      PlanSelections =
                        [ { ProductLine = Stubs.Certificate.productLine
                            ProductLineGroup = Stubs.Certificate.productLineGroup
                            Coverage = Some Stubs.Certificate.coverage
                            Option = Stubs.Certificate.option
                            RatePer = Stubs.Certificate.ratePer
                            Volume =
                              { Amount = Stubs.Certificate.volume
                                Unit = Stubs.Certificate.volumeUnit }
                            CarrierRate = Stubs.Certificate.carrierRate
                            TaxRate = Stubs.Certificate.taxRate
                            TaxProvince = Stubs.Certificate.taxProvince } ] } }

            Expect.equal actual expected "Should Equal"

    module CertificateStatusFromDomain =
        [<Fact>]
        let ``transfer an Active status`` () =
            let actual = Certificate.certificateStatusFromDomain Domain.CertificateStatus.Active
            let expected = Dao.CertificateStatus.Active

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``transfer a Terminated status`` () =
            let actual =
                Certificate.certificateStatusFromDomain Domain.CertificateStatus.Terminated

            let expected = Dao.CertificateStatus.Terminated

            Expect.equal actual expected "should equal"

module RetroactiveCertificateUpdateDao =
    module FromRetroactiveCertificateUpdate =
        [<Fact>]
        let ``transforms a RetroactiveCertificateUpdate domain model to a RetroactiveCertificateUpdateDao`` () =
            let domainModel = Stubs.RetroactiveUpdate.domainModel

            let actual =
                RetroactiveCertificateUpdate.fromRetroactiveCertificateUpdate domainModel

            let expected =
                { RetroactiveCertificateUpdateDao.Type = Dao.RetroactiveCertificateUpdateType.Update
                  CertificateNumber = "CertificateNumber"
                  CarrierName = "CarrierName"
                  ClientName = "ClientName"
                  PolicyNumber = "PolicyNumber"
                  ProductLine = "ProductLine"
                  Option = "Option"
                  Coverage = Some "Coverage"
                  UpdateDate = Stubs.RetroactiveUpdate.createdDate
                  Backdate = Stubs.RetroactiveUpdate.effectiveDate }

            Expect.equal actual expected "should equal"

module Correction =
    module FromCorrection =
        [<Fact>]
        let ``maps a Usage domain model to a CorrectionUsageDao`` () =
            let correctionUsage =
                { Stubs.Usage.Domain.domain with
                    UsageType = CertificateUsageType.Correction }

            let reversalUsage =
                { Stubs.Usage.Domain.domain with
                    UsageType = CertificateUsageType.Reversal }

            let genId = fun () -> Guid.Empty
            let billingEndDate = DateTime(2023, 8, 31)

            let domain =
                { Correction = Some correctionUsage
                  Reversal = Some reversalUsage }

            let actual =
                Correction.fromCorrectionDomain genId billingEndDate (Some Guid.Empty) Guid.Empty domain

            let expected =
                [{ Id = Guid.Empty
                   CausationId = Guid.Empty
                   CorrelatedUsageId = Some Guid.Empty
                   UsageType = Dao.CertificateUsageType.Reversal
                   CertificateNumber = Stubs.Usage.certificateNumber
                   CarrierName = Stubs.Usage.carrierName
                   ClientName = Stubs.Usage.clientName
                   PolicyNumber = Stubs.Usage.policyNumber
                   ScbPolicyNumber = Stubs.Usage.scbPolicyNumber
                   BenefitStartDate = Stubs.Usage.benefitStartDate
                   BenefitEndDate = Stubs.Usage.benefitEndDate
                   Division = Stubs.Usage.division
                   ProductLine = Stubs.Usage.productLine
                   ProductLineGroup = Stubs.Usage.productLineGroup
                   Coverage = Some Stubs.Usage.coverage
                   Option = Stubs.Usage.option
                   RatePer = Stubs.Usage.ratePer
                   VolumeAmount = Stubs.Usage.volumeAmount
                   VolumeUnit = Stubs.Usage.volumeUnit
                   CarrierRate = Stubs.Usage.carrierRate
                   TaxRate = Stubs.Usage.taxRate
                   TaxProvince = Stubs.Usage.taxProvince
                   BillingEndDate = Stubs.Usage.billingEndDate
                   DateIncurred = Stubs.Usage.dateIncurred }
                  |> Some;
                  { Id = Guid.Empty
                    CausationId = Guid.Empty
                    CorrelatedUsageId = Some Guid.Empty
                    UsageType = Dao.CertificateUsageType.Correction
                    CertificateNumber = Stubs.Usage.certificateNumber
                    CarrierName = Stubs.Usage.carrierName
                    ClientName = Stubs.Usage.clientName
                    PolicyNumber = Stubs.Usage.policyNumber
                    ScbPolicyNumber = Stubs.Usage.scbPolicyNumber
                    BenefitStartDate = Stubs.Usage.benefitStartDate
                    BenefitEndDate = Stubs.Usage.benefitEndDate
                    Division = Stubs.Usage.division
                    ProductLine = Stubs.Usage.productLine
                    ProductLineGroup = Stubs.Usage.productLineGroup
                    Coverage = Some Stubs.Usage.coverage
                    Option = Stubs.Usage.option
                    RatePer = Stubs.Usage.ratePer
                    VolumeAmount = Stubs.Usage.volumeAmount
                    VolumeUnit = Stubs.Usage.volumeUnit
                    CarrierRate = Stubs.Usage.carrierRate
                    TaxRate = Stubs.Usage.taxRate
                    TaxProvince = Stubs.Usage.taxProvince
                    BillingEndDate = Stubs.Usage.billingEndDate
                    DateIncurred = Stubs.Usage.dateIncurred }
                    |> Some ]

            Expect.equal actual expected "should equal"
