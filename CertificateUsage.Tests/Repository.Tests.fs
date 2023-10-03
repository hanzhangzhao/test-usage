module CertificateUsage.Tests.Repository

open System.Threading.Tasks

open System

open Xunit
open Expecto

open CertificateUsage.Repository
open CertificateUsage.Dao
open CertificateUsage.Tests
open CertificateUsage.Tests.Spy

module Stubs =
    let certificateNumber = "CertificateNumber"
    let carrier = "Carrier"
    let policyNumber = "PolicyNumber"
    let scbPolicyNumber = "ScbPolicyNumber"
    let effective = DateTime(2023, 6, 12)

    let certificateUsageChangeDao =
        { CertificateNumber = certificateNumber
          Carrier = carrier
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = policyNumber
          Effective = effective
          Type = CoverageType.Covered
          CoverageData =
            { CertificateNumber = ""
              Carrier = ""
              ClientName = ""
              PolicyNumber = ""
              Division = ""
              Effective = DateTime()
              PlanSelections = [] }
          EventMetadata =
            { EventId = Guid.Empty
              EventNo = 0UL
              EventType = ""
              EventDate = DateTime()
              EventVersion = ""
              EventStream = "" } }

module InsertCertificateUsage =
    [<Fact>]
    let ``inserts a certificate usage write model`` () =
        let serializeMock = fun _ -> "{}"

        let insertCommandSpy =
            Spy<Task<int>, string * (string * SqlValue) list>(fun _ -> task { return 0 })

        let insertMock =
            fun (command : string) (parameters : (string * SqlValue) list) ->
                let _ = insertCommandSpy.Function(command, parameters)
                task { return 0 }

        let _ =
            insertCertificateUsage serializeMock insertMock Stubs.certificateUsageChangeDao

        let actual = insertCommandSpy.CalledOnce
        Expect.isTrue actual ""

    [<Fact>]
    let ``serializes the data and metadata`` () =
        let serializeSpy = Spy<string, obj>(fun _ -> "")
        let insertMock = fun _ _ -> task { return 0 }

        let _ =
            insertCertificateUsage serializeSpy.Function insertMock Stubs.certificateUsageChangeDao

        let actual = serializeSpy.CalledNTimes 2
        Expect.isTrue actual ""

module insertRate =
    [<Fact>]
    let ``inserts a rate write model`` () =
        let serializeMock = fun _ -> "{}"

        let insertSpy =
            Spy<Task<int>, string * (string * SqlValue) list>(fun _ -> task { return 0 })

        let insertMock =
            fun (command : string) (parameters : (string * SqlValue) list) ->
                let _ = insertSpy.Function(command, parameters)
                task { return 0 }

        let _ = insertRate serializeMock insertMock Stubs.RateUpdateDataDao.dao

        let actual = insertSpy.CalledOnce
        Expect.isTrue actual ""

    [<Fact>]
    let ``serializes the data and metadata`` () =
        let serializeSpy = Spy<string, obj>(fun _ -> "")
        let insertMock = fun _ _ -> task { return 0 }

        let _ = insertRate serializeSpy.Function insertMock Stubs.RateUpdateDataDao.dao

        let actual = serializeSpy.CalledNTimes 2
        Expect.isTrue actual ""

module InsertRetroactiveCertificateUpdate =
    [<Fact>]
    let ``insert a retroactive update with correct parameters`` () =
        let insertSpy =
            Spy<Task<int>, string * (string * SqlValue) list>(fun _ -> Stubs.Task.lift 1)

        let insertMock =
            fun (command : string) (parameters : (string * SqlValue) list) ->
                let _ = insertSpy.Function(command, parameters)
                task { return 0 }

        let dao = Stubs.RetroactiveUpdate.dao
        let _ = insertRetroactiveCertificateUpdate insertMock dao

        let actual = insertSpy.Args[0] |> snd

        let expected =
            [ "certificateNumber", Sql.string dao.CertificateNumber
              "carrierName", Sql.string dao.CarrierName
              "clientName", Sql.string dao.ClientName
              "policyNumber", Sql.string dao.PolicyNumber
              "productLine", Sql.string dao.ProductLine
              "coverage", Sql.stringOrNone dao.Coverage
              "option", Sql.string dao.Option
              "backdate", Sql.timestamp dao.Backdate
              "updateDate", Sql.timestamp dao.UpdateDate ]

        Expect.equal actual[1] ("certificateNumber", Sql.string dao.CertificateNumber) "should equal"
        Expect.equal actual[2] ("carrierName", Sql.string dao.CarrierName) "should equal"
        Expect.equal actual[3] ("clientName", Sql.string dao.ClientName) "should equal"
        Expect.equal actual[4] ("policyNumber", Sql.string dao.PolicyNumber) "should equal"
        Expect.equal actual[5] ("productLine", Sql.string dao.ProductLine) "should equal"
        Expect.equal actual[6] ("coverage", Sql.stringOrNone dao.Coverage) "should equal"
        Expect.equal actual[7] ("option", Sql.string dao.Option) "should equal"
        Expect.equal actual[8] ("backdate", Sql.timestamp dao.Backdate) "should equal"
        Expect.equal actual[9] ("updateDate", Sql.timestamp dao.UpdateDate) "should equal"
