namespace CertificateUsage.UnitTests.Api

open System

open Xunit
open Expecto

open CertificateUsage.Dao
open CertificateUsage.Api.Dto
open CertificateUsage.Api.Service.Billing
open CertificateUsage.Api.Dependencies

open CertificateUsage.Tests.Stubs

module Service =
    module Stubs =
        let benefitStartDate = DateTime(2023, 5, 14)
        let taxProvince = "TaxProvince"
        let certificateNumber = "CertificateNumber"
        let policyNumber = "PolicyNumber"
        let scbPolicyNumber = "scbPolicyNumber"
        let carrier = "Carrier"
        let division = "Division"
        let carrierMapping = Map [ "division", division ]
        let productLine = "ProductLine"
        let productLineGroup = "ProductLineGroup"
        let lineGroup = "LineGroup"
        let option = "Option"
        let coverage = "Coverage"
        let displayPricePer = 1.1m
        let volumeAmount = 1.2m
        let volumeUnits = "CAD"
        let clientName = "ClientName"

        let eventId = Guid.NewGuid()
        let eventType = "EventType"
        let version = "Version"
        let createDate = DateTime(2023, 06, 14)
        let eventNumber = 1UL
        let streamId = "StreamId"
        let carrierRate = 0.8m
        let taxRate = 1.3m


        let coveredDao =
            { CertificateNumber = certificateNumber
              Carrier = carrier
              ScbPolicyNumber = scbPolicyNumber
              PolicyNumber = policyNumber
              Effective = benefitStartDate
              Type = CoverageType.Covered
              CoverageData =
                { CertificateNumber = certificateNumber
                  Carrier = carrier
                  ClientName = clientName
                  PolicyNumber = policyNumber
                  Division = division
                  Effective = benefitStartDate
                  PlanSelections =
                    [ { ProductLine = productLine
                        ProductLineGroup = productLineGroup
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

    let getUsageForCarrierByDateMock =
        fun _ -> task { return [ Some Stubs.coveredDao ] }

    let root : Root.Root =
        { GetUsageForCarrierByDate = getUsageForCarrierByDateMock
          GetRateChangesForCarrierOnOrAfter = fun _ -> Task.lift [ None ]
          CloseOutMonth = fun _ _ -> Task.lift (Ok [])
          PreviewUsage = fun _ -> Task.lift []
          GetUsageForCarrierByDateFromClosedBook = fun _ _ -> Task.lift (Ok [])
          GetRetroUpdates = fun _ _ _ -> Task.lift []
          GetRetroTerminations = fun _ _ -> Task.lift []
          GetRetroEnrollments = fun _ _ -> Task.lift []
          InsertCorrections = fun _ -> Task.lift []
          GetClosedBookDates = fun _ -> Task.lift [] }

    [<Fact>]
    let ``should return billing`` () =
        let carrier = "mbc"
        let cutoffDate = DateTime(2023, 06, 15)

        let expected : BillingReadModelDto list =
            [ { CarrierCode = "Carrier"
                CertificateNumber = "CertificateNumber"
                ClientName = "ClientName"
                PolicyNumber = "PolicyNumber"
                ProductLine = "ProductLine"
                ProductOption = "Option"
                Coverage = Some "Coverage"
                Volume = Stubs.volumeAmount
                Lives = 0.0m
                TaxRate = Stubs.taxRate
                Year = 2023
                Month = 05
                CarrierRate = Stubs.carrierRate
                ClientRate = 0.0m
                TaxProvince = Stubs.taxProvince
                Division = Stubs.division } ]

        task {
            let! actual = billingByCarrier root carrier cutoffDate

            Expect.equal actual expected ""
        }
