module CertificateUsage.Tests.Api.UsageCommandRepository_Tests

open System
open System.Threading.Tasks

open CertificateUsage.Tests.Domain
open Npgsql
open Npgsql.FSharp

open Xunit
open Expecto
open Foq

open CertificateUsage.Period
open CertificateUsage.IRowReader
open CertificateUsage.Dao
open CertificateUsage.Api.UsageCommandRepository
open CertificateUsage.Api.Dependencies
open CertificateUsage.Tests.Spy
open CertificateUsage.Tests.Stubs

let dependencies : Root.Root =
    { GetUsageForCarrierByDate = fun _ -> Task.lift [ None ]
      GetRateChangesForCarrierOnOrAfter = fun _ -> Task.lift [ None ]
      CloseOutMonth = fun _ _ -> Task.lift (Ok [])
      PreviewUsage = fun _ -> Task.lift []
      GetUsageForCarrierByDateFromClosedBook = fun _ _ -> Task.lift (Ok [])
      GetRetroUpdates = fun _ _ _ -> Task.lift []
      GetRetroTerminations = fun _ _ -> Task.lift []
      GetRetroEnrollments = fun _ _ -> Task.lift []
      InsertCorrections = fun _ -> Task.lift []
      GetClosedBookDates = fun _ -> Task.lift [] }

module CloseOutMonth =
    [<Fact>]
    let ``insert with the correct parameters`` () =
        let transactSpy =
            Spy<Task<int list>, (string * (string * SqlValue) list list) list>(fun _ -> Task.lift [])

        let billingPeriod =
            { Period.Start = DateTime(2023, 8, 01)
              End = DateTime(2023, 8, 31) }

        let _ = closeOutMonth transactSpy.Function "carrier" billingPeriod

        let actual = (transactSpy.Args[0][0] |> snd)[0]

        Expect.equal actual[0] ("carrierName", Sql.string "carrier") "should equal"
        Expect.equal actual[1] ("billingStart", Sql.timestamp (DateTime(2023, 8, 1))) "should equal"
        Expect.equal actual[2] ("billingEnd", Sql.timestamp (DateTime(2023, 8, 31))) "should equal"
        Expect.equal actual[4] ("billingPeriod", Sql.string "31") "should equal"


    [<Fact>]
    let ``rethrows unexpected postgres exception`` () =
        let transactMock =
            function
            | _ -> raise (PostgresException("m", "s", "is", "42P08"))

        let billingPeriod =
            { Period.Start = DateTime(2023, 8, 01)
              End = DateTime(2023, 8, 31) }

        Expect.throws (fun () -> closeOutMonth transactMock "carrier" billingPeriod |> ignore)

module RetroactiveCertificateUpdateMapper =
    [<Fact>]
    let ``map a row into a RetroactiveCertificateUpdateRecordDao`` () =
        let id = Guid.NewGuid()
        let correlatedUsageId = Guid.NewGuid()
        let CausationId = Guid.NewGuid()
        let benefitStartDate = DateTime(2022, 5, 15)
        let billingDate = DateTime(2023, 08, 31)
        let dateIncurred = DateTime(2023, 05, 31)
        let ratePer = 1.2m
        let volumeAmount = 2.3m
        let carrierRate = 3.4m
        let taxRate = 4.5m

        let certificate =
            { CertificateNumber = "certificate_number"
              CarrierName = "carrier_name"
              ClientName = "client_name"
              ScbPolicyNumber = "scb_policy_number"
              PolicyNumber = "policy_number"
              StartDate = benefitStartDate
              EndDate = None
              Division = "division"
              PlanSelections = []
              CertificateStatus = "active" }

        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.uuid "id" @>)
                .Returns(id)
                .Setup(fun x -> <@ x.uuidOrNone "correlated_usage_id" @>)
                .Returns(Some correlatedUsageId)
                .Setup(fun x -> <@ x.uuid "retroactive_certificate_update_id" @>)
                .Returns(CausationId)
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
                .Returns(None)
                .Setup(fun x -> <@ x.text "division" @>)
                .Returns("division")
                .Setup(fun x -> <@ x.text "product_line" @>)
                .Returns("product_line")
                .Setup(fun x -> <@ x.text "product_line_group" @>)
                .Returns("product_line_group")
                .Setup(fun x -> <@ x.textOrNone "coverage" @>)
                .Returns(Some "coverage")
                .Setup(fun x -> <@ x.text "option" @>)
                .Returns("option")
                .Setup(fun x -> <@ x.decimal "rate_per" @>)
                .Returns(ratePer)
                .Setup(fun x -> <@ x.decimal "volume_amount" @>)
                .Returns(volumeAmount)
                .Setup(fun x -> <@ x.text "volume_unit" @>)
                .Returns("volume_unit")
                .Setup(fun x -> <@ x.decimal "carrier_rate" @>)
                .Returns(carrierRate)
                .Setup(fun x -> <@ x.decimal "tax_rate" @>)
                .Returns(taxRate)
                .Setup(fun x -> <@ x.text "tax_province" @>)
                .Returns("tax_province")
                .Setup(fun x -> <@ x.dateTime "billing_date" @>)
                .Returns(billingDate)
                .Setup(fun x -> <@ x.dateTime "date_incurred" @>)
                .Returns(dateIncurred)
                .Setup(fun x -> <@ x.text "retro_update_product_line" @>)
                .Returns("retro_update_product_line")
                .Setup(fun x -> <@ x.textOrNone "retro_update_coverage" @>)
                .Returns(Some "retro_update_coverage")
                .Setup(fun x -> <@ x.text "retro_update_option" @>)
                .Returns("retro_update_option")
                .Create()

        let deserializerMock _ = certificate
        let actual = mapRetroUpdate deserializerMock readerMock

        let expected =
            { RetroactiveCertificateUsageUpdateDao.Usage =
                { CertificateUsageDao.Id = id
                  CorrelatedUsageId = Some correlatedUsageId
                  CausationId = CausationId
                  UsageType = CertificateUsageType.Charge
                  CertificateNumber = "certificate_number"
                  CarrierName = "carrier_name"
                  ClientName = "client_name"
                  PolicyNumber = "policy_number"
                  ScbPolicyNumber = "scb_policy_number"
                  BenefitStartDate = benefitStartDate
                  BenefitEndDate = None
                  Division = "division"
                  ProductLine = "product_line"
                  ProductLineGroup = "product_line_group"
                  Coverage = Some "coverage"
                  Option = "option"
                  RatePer = ratePer
                  VolumeAmount = volumeAmount
                  VolumeUnit = "volume_unit"
                  CarrierRate = carrierRate
                  TaxRate = taxRate
                  TaxProvince = "tax_province"
                  BillingEndDate = billingDate
                  DateIncurred = dateIncurred }
              Certificate = certificate
              ProductLine = "retro_update_product_line"
              Coverage = Some "retro_update_coverage"
              Option = "retro_update_option" }

        Expect.equal actual expected "should equal"

module GetRetroactiveUpdatesForCarrier =
    [<Fact>]
    let ``query has correct parameters`` () =
        let insertSpy =
            Spy<Task<RetroactiveCertificateUsageUpdateDao list>, (string * SqlValue) list>(fun _ -> Task.lift [])

        let insertMock _ parameters _ = insertSpy.Function parameters

        let carrierName = "equitable_life"
        let mapperMock = id

        let billingPeriod =
            { Period.Start = DateTime(2023, 08, 31)
              End = DateTime(2023, 08, 31) }

        let _ =
            getRetroUpdates insertMock mapperMock RetroactiveCertificateUpdateType.Update carrierName billingPeriod

        let actual = insertSpy.Args[0]
        Expect.equal actual[1] ("carrierName", Sql.string carrierName) "should equal"
        Expect.equal actual[2] ("billingStart", Sql.timestamp billingPeriod.Start) "should equal"
        Expect.equal actual[3] ("billingEnd", Sql.timestamp billingPeriod.End) "should equal"


module InsertCorrections =

    [<Fact>]
    let ``query has correct parameters`` () =
        let dao = CertificateUsageDao.dao
        let data = [ Some dao ]

        let transactSpy =
            Spy<Task<int list>, (string * (string * SqlValue) list list) list>(fun _ -> Task.lift [])

        let _ = insertCorrections transactSpy.Function data
        let actual = (transactSpy.Args[0][0] |> snd)[0]

        Expect.equal actual[0] ("id", Sql.uuid dao.Id) "should equal"
        Expect.equal actual[1] ("correlatedUsageId", Sql.uuidOrNone dao.CorrelatedUsageId) "should equal"
        Expect.equal actual[2] ("causationId", Sql.uuid dao.CausationId) "should equal"
        Expect.equal actual[4] ("certificateNumber", Sql.text dao.CertificateNumber) "should equal"
        Expect.equal actual[5] ("carrierName", Sql.text dao.CarrierName) "should equal"
        Expect.equal actual[6] ("clientName", Sql.text dao.ClientName) "should equal"
        Expect.equal actual[7] ("policyNumber", Sql.text dao.PolicyNumber) "should equal"
        Expect.equal actual[8] ("scbPolicyNumber", Sql.text dao.ScbPolicyNumber) "should equal"
        Expect.equal actual[9] ("benefitStartDate", Sql.date dao.BenefitStartDate) "should equal"
        Expect.equal actual[10] ("benefitEndDate", Sql.dateOrNone dao.BenefitEndDate) "should equal"
        Expect.equal actual[11] ("division", Sql.text dao.Division) "should equal"
        Expect.equal actual[12] ("productLine", Sql.text dao.ProductLine) "should equal"
        Expect.equal actual[13] ("productLineGroup", Sql.text dao.ProductLineGroup) "should equal"
        Expect.equal actual[14] ("coverage", Sql.textOrNone dao.Coverage) "should equal"
        Expect.equal actual[15] ("option", Sql.text dao.Option) "should equal"
        Expect.equal actual[16] ("ratePer", Sql.decimal dao.RatePer) "should equal"
        Expect.equal actual[17] ("volumeAmount", Sql.decimal dao.VolumeAmount) "should equal"
        Expect.equal actual[18] ("volumeUnit", Sql.text dao.VolumeUnit) "should equal"
        Expect.equal actual[19] ("carrierRate", Sql.decimal dao.CarrierRate) "should equal"
        Expect.equal actual[20] ("taxRate", Sql.decimal dao.TaxRate) "should equal"
        Expect.equal actual[21] ("taxProvince", Sql.text dao.TaxProvince) "should equal"
        Expect.equal actual[22] ("billingDate", Sql.timestamp dao.BillingEndDate) "should equal"
        Expect.equal actual[23] ("dateIncurred", Sql.timestamp dao.DateIncurred) "should equal"

module MapRetroTransition =
    [<Fact>]
    let ``map a row to a RetroactiveCertificateUsageTransitionDao`` () =
        let id = Guid.NewGuid()
        let correlatedUsageId = Guid.NewGuid()
        let causationId = Guid.NewGuid()
        let benefitStartDate = DateTime(2022, 5, 15)
        let billingDate = DateTime(2023, 08, 31)
        let dateIncurred = DateTime(2023, 05, 31)
        let backDate = DateTime(2023, 06, 15)
        let ratePer = 1.2m
        let volumeAmount = 2.3m
        let carrierRate = 3.4m
        let taxRate = 4.5m

        let certificate =
            { CertificateNumber = "certificate_number"
              CarrierName = "carrier_name"
              ClientName = "client_name"
              ScbPolicyNumber = "scb_policy_number"
              PolicyNumber = "policy_number"
              StartDate = benefitStartDate
              EndDate = None
              Division = "division"
              PlanSelections = []
              CertificateStatus = "active" }

        let readerMock =
            Mock<IRowReader>()
                .Setup(fun x -> <@ x.uuid "id" @>)
                .Returns(id)
                .Setup(fun x -> <@ x.uuidOrNone "correlated_usage_id" @>)
                .Returns(Some correlatedUsageId)
                .Setup(fun x -> <@ x.uuid "retroactive_certificate_update_id" @>)
                .Returns(causationId)
                .Setup(fun x -> <@ x.enum<CertificateUsageType> "usage_type" @>)
                .Returns(CertificateUsageType.Charge)
                .Setup(fun x -> <@ x.textOrNone "certificate_number" @>)
                .Returns(Some "certificate_number")
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
                .Returns(None)
                .Setup(fun x -> <@ x.text "division" @>)
                .Returns("division")
                .Setup(fun x -> <@ x.text "product_line" @>)
                .Returns("product_line")
                .Setup(fun x -> <@ x.text "product_line_group" @>)
                .Returns("product_line_group")
                .Setup(fun x -> <@ x.textOrNone "coverage" @>)
                .Returns(Some "coverage")
                .Setup(fun x -> <@ x.text "option" @>)
                .Returns("option")
                .Setup(fun x -> <@ x.decimal "rate_per" @>)
                .Returns(ratePer)
                .Setup(fun x -> <@ x.decimal "volume_amount" @>)
                .Returns(volumeAmount)
                .Setup(fun x -> <@ x.text "volume_unit" @>)
                .Returns("volume_unit")
                .Setup(fun x -> <@ x.decimal "carrier_rate" @>)
                .Returns(carrierRate)
                .Setup(fun x -> <@ x.decimal "tax_rate" @>)
                .Returns(taxRate)
                .Setup(fun x -> <@ x.text "tax_province" @>)
                .Returns("tax_province")
                .Setup(fun x -> <@ x.dateTime "billing_date" @>)
                .Returns(billingDate)
                .Setup(fun x -> <@ x.dateTime "date_incurred" @>)
                .Returns(dateIncurred)
                .Setup(fun x -> <@ x.dateTime "date_incurred" @>)
                .Returns(dateIncurred)
                .Setup(fun x -> <@ x.dateTime "backdate" @>)
                .Returns(backDate)
                .Setup(fun x -> <@ x.text "retro_update_product_line" @>)
                .Returns("retro_update_product_line")
                .Setup(fun x -> <@ x.textOrNone "retro_update_coverage" @>)
                .Returns(Some "retro_update_coverage")
                .Setup(fun x -> <@ x.text "retro_update_option" @>)
                .Returns("retro_update_option")
                .Create()

        let deserializerMock _ = certificate

        let billingPeriod =
            { Period.Start = DateTime(2023, 8, 1)
              Period.End = billingDate }

        let actual = mapRetroTransition deserializerMock billingPeriod readerMock

        let expected =
            { RetroactiveCertificateUsageTransitionDao.RetroCertificateUpdateId = causationId
              Usage =
                { CertificateUsageDao.Id = id
                  CorrelatedUsageId = Some correlatedUsageId
                  CausationId = causationId
                  UsageType = CertificateUsageType.Charge
                  CertificateNumber = "certificate_number"
                  CarrierName = "carrier_name"
                  ClientName = "client_name"
                  PolicyNumber = "policy_number"
                  ScbPolicyNumber = "scb_policy_number"
                  BenefitStartDate = benefitStartDate
                  BenefitEndDate = None
                  Division = "division"
                  ProductLine = "product_line"
                  ProductLineGroup = "product_line_group"
                  Coverage = Some "coverage"
                  Option = "option"
                  RatePer = ratePer
                  VolumeAmount = volumeAmount
                  VolumeUnit = "volume_unit"
                  CarrierRate = carrierRate
                  TaxRate = taxRate
                  TaxProvince = "tax_province"
                  BillingEndDate = billingDate
                  DateIncurred = dateIncurred }
                |> Some
              Certificate = certificate
              BillingEnd = billingDate
              BillingStart = DateTime(2023, 8, 01)
              Backdate = backDate
              ProductLine = "retro_update_product_line"
              Coverage = Some "retro_update_coverage"
              Option = "retro_update_option" }

        Expect.equal actual expected "should equal"

module GetRetroTerminations =
    [<Fact>]
    let ``query has correct parameters`` () =
        let readerSpy =
            Spy<Task<RetroactiveCertificateUsageTransitionDao list>, (string * SqlValue) list>(fun _ -> Task.lift [])

        let readerMock _ parameters _ = readerSpy.Function parameters

        let mapperMock _ =
            CertificateUsage.Tests.Stubs.RetroactiveCertificateUsageTransitionDao.Dao.stub

        let carrierName = "equitable_life"

        let billingPeriod =
            { Period.Start = DateTime(2023, 8, 1)
              Period.End = DateTime(2023, 8, 31) }

        let _ = getRetroTerminations readerMock mapperMock carrierName billingPeriod
        let actual = readerSpy.Args[0]

        Expect.equal actual[1] ("carrierName", Sql.string carrierName) "should equal"
        Expect.equal actual[2] ("billingStart", Sql.timestamp billingPeriod.Start) "should equal"
        Expect.equal actual[3] ("billingEnd", Sql.timestamp billingPeriod.End) "should equal"


module GetRetroEnrollments =
    [<Fact>]
    let ``query has correct parameters`` () =
        let readerSpy =
            Spy<Task<RetroactiveCertificateUsageTransitionDao list>, (string * SqlValue) list>(fun _ -> Task.lift [])

        let readerMock _ parameters _ = readerSpy.Function parameters

        let mapperMock _ =
            CertificateUsage.Tests.Stubs.RetroactiveCertificateUsageTransitionDao.Dao.stub

        let carrierName = "equitable_life"

        let billingPeriod =
            { Period.Start = DateTime(2023, 8, 1)
              Period.End = DateTime(2023, 8, 31) }

        let _ = getRetroEnrollments readerMock mapperMock carrierName billingPeriod
        let actual = readerSpy.Args[0]

        Expect.equal actual[1] ("carrierName", Sql.string carrierName) "should equal"
        Expect.equal actual[2] ("billingStart", Sql.timestamp billingPeriod.Start) "should equal"
        Expect.equal actual[3] ("billingEnd", Sql.timestamp billingPeriod.End) "should equal"
