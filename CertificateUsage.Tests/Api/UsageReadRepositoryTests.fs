module CertificateUsage.Tests.UsageReadRepositoryTests

open System
open System.Threading.Tasks

open Xunit
open Foq
open Expecto

open CertificateUsage.IRowReader
open CertificateUsage.Dao
open CertificateUsage.Api.Repository.Usage
open CertificateUsage.Api.Dao
open CertificateUsage.Tests.Stubs
open CertificateUsage.Tests.Spy

module MapRowToCertificateUsageChangeDao =
    [<Fact>]
    let ``maps a row into a CertificateUsageChangeDao`` () =
        let effective = DateTime(2023, 8, 21)

        let coverageData =
            """
             {
              "Carrier": "carrier",
              "Division": "division",
              "Effective": "2023-08-21T00:00:00",
              "ClientName": "client_name",
              "PolicyNumber": "policy_number",
              "PlanSelections": [
                {
                  "Option": "option",
                  "Volume": {
                    "Unit": "quantity",
                    "Amount": 1.23
                  },
                  "RatePer": 2.34,
                  "TaxRate": 3.45,
                  "Coverage": "single",
                  "CarrierRate": 99.1,
                  "ProductLine": "health",
                  "TaxProvince": "British Columbia",
                  "ProductLineGroup": "product_line_group"
                }
              ],
              "CertificateNumber": "2"
             }
             """

        let metadata =
            """
             {
               "EventId": "20259135-9135-9135-9135-169220259135",
               "EventNo": 1,
               "EventDate": "2023-08-10T00:07:00",
               "EventType": "MemberEnrollmentConfirmed",
               "EventStream": "members",
               "EventVersion": "1.0.0"
             }
             """

        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.text "certificate_number" @>)
                .Returns("certificate_number")
                .Setup(fun x -> <@ x.text "carrier" @>)
                .Returns("carrier")
                .Setup(fun x -> <@ x.text "policy_number" @>)
                .Returns("policy_number")
                .Setup(fun x -> <@ x.text "scb_policy_number" @>)
                .Returns("scb_policy_number")
                .Setup(fun x -> <@ x.dateTime "effective" @>)
                .Returns(effective)
                .Setup(fun x -> <@ x.text "type" @>)
                .Returns("covered")
                .Setup(fun x -> <@ x.text "coverage_data" @>)
                .Returns(coverageData)
                .Setup(fun x -> <@ x.text "event_meta" @>)
                .Returns(metadata)
                .Create()

        let actual = mapRowToCertificateUsageChangeDao readerMock

        let expected =
            { CertificateUsageChangeDao.CertificateNumber = "certificate_number"
              Carrier = "carrier"
              ScbPolicyNumber = "scb_policy_number"
              PolicyNumber = "policy_number"
              Effective = effective
              Type = CoverageType.Covered
              CoverageData =
                { CertificateNumber = "2"
                  Carrier = "carrier"
                  ClientName = "client_name"
                  PolicyNumber = "policy_number"
                  Division = "division"
                  Effective = effective
                  PlanSelections =
                    [ { ProductLine = "health"
                        ProductLineGroup = "product_line_group"
                        Coverage = Some "single"
                        Option = "option"
                        RatePer = 2.34m
                        Volume = { Amount = 1.23m; Unit = "quantity" }
                        CarrierRate = 99.1m
                        TaxRate = 3.45m
                        TaxProvince = "British Columbia" }

                      ] }
              EventMetadata =
                { EventId = Guid("20259135-9135-9135-9135-169220259135")
                  EventNo = 1UL
                  EventType = "MemberEnrollmentConfirmed"
                  EventDate = DateTime(2023, 8, 10, 0, 7, 0)
                  EventVersion = "1.0.0"
                  EventStream = "members" } }
            |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``returns None when failing to map a row into a CertificateUsageChangeDao`` () =
        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.text (any ()) @>)
                .Raises(Exception("test"))
                .Create()

        let actual = mapRowToCertificateUsageChangeDao readerMock
        let expected = None

        Expect.equal actual expected "should equal"

module MapRowToRateChangeDataDto =
    [<Fact>]
    let ``maps a row into a RateChangeDataDto`` () =
        let effective = DateTime(2023, 8, 21)

        let rateData =
            """
             {
              "Carrier": "carrier",
              "Effective": "2023-08-21T00:00:00",
              "PolicyNumber": "policy_number",
              "Option": "option",
              "Coverage": "coverage",
              "ProductLine": "product_line",
              "CarrierRate": 1.23,
              "ChangedBy": {
                "Id": 1,
                "Name": "name"
              }
             }
             """

        let metadata =
            """
             {
               "EventId": "20259135-9135-9135-9135-169220259135",
               "EventNo": 1,
               "EventDate": "2023-08-10T00:07:00",
               "EventType": "MemberEnrollmentConfirmed",
               "EventStream": "members",
               "EventVersion": "1.0.0"
             }
             """

        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.text "carrier" @>)
                .Returns("carrier")
                .Setup(fun x -> <@ x.text "policy_number" @>)
                .Returns("policy_number")
                .Setup(fun x -> <@ x.text "option" @>)
                .Returns("option")
                .Setup(fun x -> <@ x.text "product_line" @>)
                .Returns("product_line")
                .Setup(fun x -> <@ x.textOrNone "coverage" @>)
                .Returns(Some "coverage")
                .Setup(fun x -> <@ x.dateTime "effective" @>)
                .Returns(effective)
                .Setup(fun x -> <@ x.text "rate_data" @>)
                .Returns(rateData)
                .Setup(fun x -> <@ x.text "event_meta" @>)
                .Returns(metadata)
                .Create()

        let actual = mapRowToRateChangeDataDto readerMock

        let expected =
            { RateUpdateDataDao.Carrier = "carrier"
              PolicyNumber = "policy_number"
              Option = "option"
              Coverage = Some "coverage"
              ProductLine = "product_line"
              Effective = effective
              RateUpdateData =
                { Carrier = "carrier"
                  PolicyNumber = "policy_number"
                  Effective = effective
                  Option = "option"
                  Coverage = Some "coverage"
                  ProductLine = "product_line"
                  CarrierRate = 1.23m
                  ChangedBy = { Id = 1; Name = "name" } }
              EventMetadata =
                { EventId = Guid("20259135-9135-9135-9135-169220259135")
                  EventNo = 1UL
                  EventType = "MemberEnrollmentConfirmed"
                  EventDate = DateTime(2023, 8, 10, 0, 7, 0)
                  EventVersion = "1.0.0"
                  EventStream = "members" } }
            |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``returns None when failing to map a row into a RateChangeDataDto`` () =
        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.text (any ()) @>)
                .Raises(Exception("test"))
                .Create()

        let actual = mapRowToRateChangeDataDto readerMock
        let expected = None

        Expect.equal actual expected "should equal"

module PreviewMapper =
    [<Fact>]
    let ``maps a row into a UsagePreviewDto`` () =
        let benefitStartDate = DateTime(2023, 8, 21)
        let benefitEndDate = DateTime(2024, 8, 21)
        let ratePer = 1.23m
        let volumeAmount = 2.34m
        let carrierRate = 3.45m
        let taxRate = 4.56m

        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.text "certificate_number" @>)
                .Returns("certificate_number")
                .Setup(fun x -> <@ x.text "carrier_name" @>)
                .Returns("carrier_name")
                .Setup(fun x -> <@ x.text "client_name" @>)
                .Returns("client_name")
                .Setup(fun x -> <@ x.text "policy_number" @>)
                .Returns("policy_number")
                .Setup(fun x -> <@ x.text "scb_policy_number" @>)
                .Returns("scb_policy_number")
                .Setup(fun x -> <@ x.dateTime "benefit_start_date" @>)
                .Returns(benefitStartDate)
                .Setup(fun x -> <@ x.dateTimeOrNone "benefit_end_date" @>)
                .Returns(Some benefitEndDate)
                .Setup(fun x -> <@ x.text "division" @>)
                .Returns("division")
                .Setup(fun x -> <@ x.text "product_line" @>)
                .Returns("product_line")
                .Setup(fun x -> <@ x.textOrNone "product_line_group" @>)
                .Returns(Some "product_line_group")
                .Setup(fun x -> <@ x.textOrNone "coverage" @>)
                .Returns(Some "coverage")
                .Setup(fun x -> <@ x.text "option" @>)
                .Returns("option")
                .Setup(fun x -> <@ x.decimalOrNone "rate_per" @>)
                .Returns(Some ratePer)
                .Setup(fun x -> <@ x.decimal "volume_amount" @>)
                .Returns(volumeAmount)
                .Setup(fun x -> <@ x.text "volume_unit" @>)
                .Returns("volume_unit")
                .Setup(fun x -> <@ x.decimal "carrier_rate" @>)
                .Returns(carrierRate)
                .Setup(fun x -> <@ x.decimalOrNone "tax_rate" @>)
                .Returns(Some taxRate)
                .Setup(fun x -> <@ x.text "tax_province" @>)
                .Returns("tax_province")
                .Create()

        let actual = previewMapper readerMock

        let expected =
            { CertificateNumber = "certificate_number"
              CarrierName = "carrier_name"
              ClientName = "client_name"
              PolicyNumber = "policy_number"
              ScbPolicyNumber = "scb_policy_number"
              BenefitStartDate = benefitStartDate
              BenefitEndDate = Some benefitEndDate
              Division = "division"
              ProductLine = "product_line"
              ProductLineGroup = Some "product_line_group"
              Coverage = Some "coverage"
              Option = "option"
              RatePer = Some ratePer
              VolumeAmount = volumeAmount
              VolumeUnit = "volume_unit"
              CarrierRate = carrierRate
              TaxRate = Some taxRate
              TaxProvince = "tax_province" }
            |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``returns None when failing to map a row into a UsagePreviewDto`` () =
        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.text (any ()) @>)
                .Raises(Exception("test"))
                .Create()

        let actual = previewMapper readerMock
        let expected = None

        Expect.equal actual expected "should equal"

module PreviewUsage =
    [<Fact>]
    let ``queries with correct parameters`` () =
        let queryParameterSpy = Spy<int, (string * SqlValue) list>(fun _ -> 1)
        let queryMock = (fun _ parameters _ -> queryParameterSpy.Function parameters)

        let _ = previewUsage queryMock "mapper" "carrierName"
        let expected = [ "carrierName", Sql.string "carrierName" ]
        let actual = queryParameterSpy.Args[0]

        Expect.equal actual expected "should equal"

module MapCertificateRowToUsageLineDao =
    [<Fact>]
    let ``maps a row into a UsageLineDao`` () =
        let volumeAmount = 2.34m
        let carrierRate = 3.45m
        let taxRate = 4.56m

        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.text "certificate_number" @>)
                .Returns("certificate_number")
                .Setup(fun x -> <@ x.text "carrier_name" @>)
                .Returns("carrier_name")
                .Setup(fun x -> <@ x.text "client_name" @>)
                .Returns("client_name")
                .Setup(fun x -> <@ x.text "policy_number" @>)
                .Returns("policy_number")
                .Setup(fun x -> <@ x.text "division" @>)
                .Returns("division")
                .Setup(fun x -> <@ x.text "product_line" @>)
                .Returns("product_line")
                .Setup(fun x -> <@ x.textOrNone "coverage" @>)
                .Returns(Some "coverage")
                .Setup(fun x -> <@ x.text "option" @>)
                .Returns("option")
                .Setup(fun x -> <@ x.decimal "volume_amount" @>)
                .Returns(volumeAmount)
                .Setup(fun x -> <@ x.decimal "tax_rate" @>)
                .Returns(taxRate)
                .Setup(fun x -> <@ x.text "tax_province" @>)
                .Returns("tax_province")
                .Setup(fun x -> <@ x.decimal "carrier_rate" @>)
                .Returns(carrierRate)
                .Setup(fun x -> <@ x.text "division" @>)
                .Returns("division")
                .Setup(fun x -> <@ x.dateTime "date_incurred" @>)
                .Returns(DateTime(2023, 8, 1))
                .Setup(fun x -> <@ x.text "volume_unit" @>)
                .Returns("quantity")
                .Create()

        let actual = mapCertificateRowToUsageLineDao readerMock

        let expected =
            { UsageLineDao.UsageType = "charge"
              CertificateNumber = "certificate_number"
              CarrierCode = "carrier_name"
              ClientName = "client_name"
              PolicyNumber = "policy_number"
              ProductLine = "product_line"
              Coverage = Some "coverage"
              ProductOption = "option"
              Volume = volumeAmount
              Lives = 0m
              TaxRate = taxRate
              TaxProvince = "tax_province"
              Year = 2023
              Month = 08
              CarrierRate = carrierRate
              ClientRate = 0.0m
              Division = "division"
              VolumeUnit = "quantity" }
            |> Some

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``returns None when failing to map a row into a UsageLineDao`` () =
        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.text (any ()) @>)
                .Raises(Exception("test"))
                .Create()

        let actual = mapCertificateRowToUsageLineDao readerMock
        let expected = None

        Expect.equal actual expected "should equal"

module mapClosedBookDate =
    [<Fact>]
    let ``maps a row to a data`` () =
        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.dateTime "billing_date" @>)
                .Returns(DateTime(2023, 8, 31))
                .Create()

        let actual = mapClosedBookDate readerMock
        let expected = DateTime(2023, 8, 31)

        Expect.equal actual expected "should equal"

module GetUsageForCarrierByDateFromClosedBook =
    [<Fact>]
    let ``query has correct parameters`` () =
        let parameterSpy =
            Spy<Task<DateTime list>, (string * SqlValue) list>(fun _ -> Task.lift [])

        let selectMock _ parameters _ = parameterSpy.Function parameters
        let mapper _ = id
        let carrierName = "equitable_life"
        let billingPeriodEndDate = DateTime(2023, 08, 31)

        let _ =
            getUsageForCarrierByDateFromClosedBook selectMock mapper carrierName billingPeriodEndDate

        let actual = parameterSpy.Args[0]
        Expect.equal actual[0] ("carrierName", Sql.string carrierName) "should equal"
        Expect.equal actual[1] ("billingDate", Sql.timestamp billingPeriodEndDate) "should equal"

module GetClosedBookDates =
    [<Fact>]
    let ``query has correct parameters`` () =
        let parameterSpy =
            Spy<Task<DateTime list>, (string * SqlValue) list>(fun _ -> Task.lift [])

        let selectMock _ parameters _ = parameterSpy.Function parameters
        let carrierName = "equitable_life"
        let mapper _ = DateTime(2023, 8, 31)

        let _ = getClosedBookDates selectMock mapper carrierName

        let actual = parameterSpy.Args[0]
        Expect.equal actual[0] ("carrierName", Sql.string carrierName) "should equal"
