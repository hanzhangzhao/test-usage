namespace CertificateUsage.UnitTests.Api

open System
open Xunit
open Expecto

open Microsoft.Extensions.Primitives

open CertificateUsage.Errors
open CertificateUsage.Dao
open CertificateUsage.Api.Endpoints.Usage
open CertificateUsage.Api.Dependencies
open CertificateUsage.UnitTests
open CertificateUsage.Tests.Stubs

module Endpoint =
    module Stubs =
        let benefitStartDate = DateTime(2023, 6, 14)
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
    let ``Should return billing for MBC`` () =
        task {
            let carrier = "mbc"

            let query =
                [ "year", StringValues "2023"
                  "month", StringValues "01"
                  "day", StringValues "01" ]

            let mockContext =
                Mocks.getContextWithQueryFields "GET" "/v0/usage/carrier/mbc/billing" query

            let! actual = billingByCarrierAndMonthHandler root carrier Mocks.next mockContext

            match actual with
            | Some ctx -> Expect.equal ctx.Response.StatusCode 200 ""
            | None -> failwith "error"
        }

    [<Fact>]
    let ``Should handle empty params`` () =
        task {

            let carrier = "mbc"
            let query = [ "year", StringValues "2023" ]

            let mockContext =
                Mocks.getContextWithQueryFields "GET" "/v0/usage/carrier/mbc/billing" query

            let! actual = billingByCarrierAndMonthHandler root carrier Mocks.next mockContext

            match actual with
            | Some ctx -> Expect.equal ctx.Response.StatusCode 400 ""
            | None -> failwith "error"
        }

    [<Fact>]
    let ``Should load the router`` () =
        task {
            let query =
                [ "year", StringValues "2023"
                  "month", StringValues "01"
                  "day", StringValues "01" ]

            let mockContext = Mocks.getContextWithQueryFields "GET" "/carrier/mbc/billing" query

            let! actual = usageRouter root Mocks.next mockContext

            match actual with
            | Some ctx -> Expect.equal ctx.Response.StatusCode 200 ""
            | None -> failwith "error"
        }

    module CloseUsageForCarrierInYearAndMonthHandler =
        [<Fact>]
        let ``returns 200 when successful`` () =
            task {
                let mockContext =
                    Mocks.getContextWithQueryFields "POST" "/close/carrier/carrier_name/year/2023/month/8/day/1" []

                let! actual = usageRouter root Mocks.next mockContext

                match actual with
                | Some ctx -> Expect.equal ctx.Response.StatusCode 200 ""
                | None -> failwith "error"
            }

        [<Fact>]
        let ``returns 200 on a lower date boundary`` () =
            task {
                let mockContext =
                    Mocks.getContextWithQueryFields "POST" "/close/carrier/carrier_name/year/1000/month/1/day/1" []

                let! actual = usageRouter root Mocks.next mockContext

                match actual with
                | Some ctx -> Expect.equal ctx.Response.StatusCode 200 ""
                | None -> failwith "error"
            }

        [<Fact>]
        let ``returns 200 on a upper date boundary`` () =
            task {
                let mockContext =
                    Mocks.getContextWithQueryFields "POST" "/close/carrier/carrier_name/year/9999/month/12/day/1" []

                let! actual = usageRouter root Mocks.next mockContext

                match actual with
                | Some ctx -> Expect.equal ctx.Response.StatusCode 200 ""
                | None -> failwith "error"
            }

        [<Fact>]
        let ``returns 400 when given too large a month`` () =
            task {
                let mockContext =
                    Mocks.getContextWithQueryFields "POST" "/close/carrier/carrier_name/year/2023/month/13/day/1" []

                let! actual = usageRouter root Mocks.next mockContext

                match actual with
                | Some ctx -> Expect.equal ctx.Response.StatusCode 400 ""
                | None -> failwith "error"
            }

        [<Fact>]
        let ``returns 400 when given too small a month`` () =
            task {
                let mockContext =
                    Mocks.getContextWithQueryFields "POST" "/close/carrier/carrier_name/year/2023/month/0/day/1" []

                let! actual = usageRouter root Mocks.next mockContext

                match actual with
                | Some ctx -> Expect.equal ctx.Response.StatusCode 400 ""
                | None -> failwith "error"
            }

        [<Fact>]
        let ``returns 400 when given too large a year`` () =
            task {
                let mockContext =
                    Mocks.getContextWithQueryFields "POST" "/close/carrier/carrier_name/year/10000/month/8/day/1" []

                let! actual = usageRouter root Mocks.next mockContext

                match actual with
                | Some ctx -> Expect.equal ctx.Response.StatusCode 400 ""
                | None -> failwith "error"
            }

    module PreviewUsageByCarrierYearAndMonthHandler =
        [<Fact>]
        let ``returns 200 when successful`` () =
            task {
                let mockContext =
                    Mocks.getContextWithQueryFields "GET" "/preview/carrier/carrier_name" []

                let! actual = usageRouter root Mocks.next mockContext

                match actual with
                | Some ctx -> Expect.equal ctx.Response.StatusCode 200 ""
                | None -> failwith "error"
            }
