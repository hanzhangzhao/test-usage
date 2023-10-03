module CertificateUsage.Tests.Acceptance

open System
open System.Threading.Tasks

open Xunit
open Expecto

open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dto.Events.MemberEnrolled
open CertificateUsage.Dto.Events.Metadata
open CertificateUsage.Dao
open CertificateUsage.Tests.Spy
open CertificateUsage.CertificateUsageWorkflow

module Stubs =
    let benefitStartDate = DateTime(2023, 6, 14)
    let taxProvince = "TaxProvince"
    let certificateNumber = "CertificateNumber"
    let scbPolicyNumber = "ScbPolicyNumber"
    let externalPolicyNumber = "ExternalPolicyNumber"
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
    let carrierRate = "0.8"
    let taxRate = "1.3"

    let memberEnrollmentDto =
        { MemberEnrollmentDto.BenefitsStartDate = benefitStartDate
          TaxProvince = taxProvince
          CertificateNumber = certificateNumber
          PolicyNumber = scbPolicyNumber
          ExternalPolicyNumber = Some externalPolicyNumber
          CarrierClientCode = None
          Carrier = carrier
          CarrierMapping = Some carrierMapping
          PlanSelections =
            [ { ProductLine = productLine
                LineGroup = lineGroup
                Selection = option
                Coverage = Some coverage
                PricePer = displayPricePer
                Volume =
                  { Amount = volumeAmount
                    Units = volumeUnits }
                CarrierRate = carrierRate
                TaxRate = taxRate
                TaxProvince = taxProvince } ]
          Client = { Name = Some clientName }
          Coverages = Map [ productLine, coverage ] |> Some }

    let memberEnrollmentEventDto = memberEnrollmentDto |> MemberEnrollmentDto

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

    let coveredWriteModel =
        { CertificateNumber = certificateNumber
          Carrier = carrier
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = externalPolicyNumber
          Effective = benefitStartDate
          Type = CoverageType.Covered
          CoverageData =
            { CertificateNumber = certificateNumber
              Carrier = carrier
              ClientName = clientName
              PolicyNumber = externalPolicyNumber
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
                    CarrierRate = 0.8m
                    TaxRate = 1.3m
                    TaxProvince = taxProvince } ] }
          EventMetadata =
            { EventId = eventId
              EventNo = eventNumber
              EventType = eventType
              EventDate = createDate
              EventVersion = version
              EventStream = streamId } }

module ExecuteWriteModelWorkflow =
    [<Fact>]
    let ``successfully executes a workflow`` () =
        let insertCertificateUsageSpy =
            Spy<Task<int>, CertificateUsageChangeDao>(fun _ -> task { return 0 })

        let _ =
            executeCertificateUsageWorkflow
                insertCertificateUsageSpy.Function
                Stubs.metadataDto
                Stubs.memberEnrollmentEventDto

        let actual = insertCertificateUsageSpy.CalledOnceWith Stubs.coveredWriteModel

        Expect.isTrue actual ""
