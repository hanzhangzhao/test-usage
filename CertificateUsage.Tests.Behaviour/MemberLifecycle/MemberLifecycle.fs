module CertificateUsage.Tests.Behaviour.MemberLifecycle.MemberLifecycle

open System

open FsToolkit.ErrorHandling
open Microsoft.FSharp.Core

open global.Xunit
open FsUnit.Xunit
open TickSpec

open CertificateUsage.Period
open CertificateUsage.Dao
open CertificateUsage.Api.Dao
open CertificateUsage.Tests.Behaviour
open CertificateUsage.Tests.Behaviour.ListenerProxy
open CertificateUsage.Tests.Behaviour.Db

module Matching =
    let isMetered (certificate: CertificateRecordDao) (line: UsageLineDao) =
        line.CertificateNumber = certificate.CertificateNumber &&
        line.PolicyNumber = certificate.PolicyNumber &&
        line.CarrierCode = certificate.Carrier &&
        line.Division = certificate.Certificate.Division &&
        line.ClientName = certificate.ClientName

    let matchProduct coverage line opt (usage: UsageLineDao)  =
        usage.Coverage = coverage &&
        usage.ProductLine = line &&
        usage.ProductOption = opt

    let mapBool f =
        Option.map f >> Option.defaultValue false

open Matching

let defaultCertificate : CertificateRecordDao =
    { CertificateNumber = "123456"
      Carrier = "equitable_life"
      PolicyNumber = "2345"
      ClientName = "Acme Inc."
      Status = CertificateStatus.Active
      Certificate =
        { CertificateNumber = "123456"
          CarrierName = "equitable_life"
          ClientName = "Acme Inc."
          ScbPolicyNumber = "1234"
          PolicyNumber = "2345"
          StartDate = DateTime.Now
          EndDate = None
          Division = "001"
          PlanSelections = []
          CertificateStatus = "active" } }

let defaultPlanSelection : PlanSelectionDao =
    { ProductLine = ""
      ProductLineGroup = ""
      Coverage = None
      Option = ""
      RatePer = 1.0m
      CarrierRate = 0.0m
      TaxRate = 0.0m
      TaxProvince = "British Columbia"
      Volume = { Amount = 1.0m; Unit = "quantity" } }

[<Given>]
let ``a certificate with certificate number (.*)`` (cert : string)  =
    { defaultCertificate with
        CertificateNumber = cert
        Certificate =
            { defaultCertificate.Certificate with
                CertificateNumber = cert } }

[<Given>]
let ``is under carrier (.*)`` (carrierName : string) (theCertificate : CertificateRecordDao)=
    { theCertificate with
        Carrier = carrierName
        Certificate =
            { theCertificate.Certificate with
                CarrierName = carrierName } }

[<Given>]
let ``is sponsored by client (.*)`` (clientName : string) (theCertificate : CertificateRecordDao) =
    { theCertificate with
        ClientName =  clientName
        Certificate =
            { theCertificate.Certificate with
                ClientName = clientName } }

[<Given>]
let ``has policy number (.*)`` (policyNumber : string) (theCertificate : CertificateRecordDao) =
    { theCertificate with
        PolicyNumber = policyNumber
        Certificate =
            { theCertificate.Certificate with
                PolicyNumber = policyNumber } }

[<Given>]
let ``has division (.*)`` (division : string) (theCertificate : CertificateRecordDao) =
    { theCertificate with
        Certificate =
            { theCertificate.Certificate with
                Division = division } }

[<Given>]
let ``the certificate has a no coverage (.*) option (.*) at a rate of (.*) CAD``
    (productLine : string)
    (option : string)
    (rate : decimal)
    (theCertificate : CertificateRecordDao)
    =
    let planSelection =
        { defaultPlanSelection with
            ProductLine = productLine
            Option = option
            CarrierRate = rate }

    let updatedCertificate =
        { theCertificate with
           Certificate =
            { theCertificate.Certificate with
               PlanSelections = planSelection :: theCertificate.Certificate.PlanSelections } }

    updatedCertificate

[<Given>]
let ``the certificate has a (.*) (.*) option (.*) at a rate of (.*) CAD``
    (coverage: string)
    (productLine : string)
    (option : string)
    (rate : decimal)
    (theCertificate : CertificateRecordDao)
    =
    let planSelection =
        { defaultPlanSelection with
            Coverage = Some coverage
            ProductLine = productLine
            Option = option
            CarrierRate = rate }

    let updatedCertificate =
        { theCertificate with
           Certificate =
            { theCertificate.Certificate with
               PlanSelections = planSelection :: theCertificate.Certificate.PlanSelections } }

    updatedCertificate


[<Given>]
let ``the certificate has a no coverage (.*) option (.*) at a rate of (.*) CAD and a volume of (.*) (.*)``
    (productLine : string)
    (option : string)
    (rate : decimal)
    (volume: decimal)
    (volumeUnits: string)
    (theCertificate : CertificateRecordDao)
    =
    let planSelection =
        { defaultPlanSelection with
            ProductLine = productLine
            Option = option
            Volume = {Amount = volume; Unit = volumeUnits }
            CarrierRate = rate }

    let updatedCertificate =
        { theCertificate with
           Certificate =
            { theCertificate.Certificate with
               PlanSelections = planSelection :: theCertificate.Certificate.PlanSelections } }

    updatedCertificate

[<Given>]
let ``the certificate has a (.*) (.*) option (.*) at a rate of (.*) CAD and a volume of (.*) (.*)``
    (coverage: string)
    (productLine : string)
    (option : string)
    (rate : decimal)
    (volume: decimal)
    (volumeUnits: string)
    (theCertificate : CertificateRecordDao)
    =
    let planSelection =
        { defaultPlanSelection with
            Coverage = Some coverage
            ProductLine = productLine
            Option = option
            Volume = {Amount = volume; Unit = volumeUnits }
            CarrierRate = rate }

    let updatedCertificate =
        { theCertificate with
           Certificate =
            { theCertificate.Certificate with
               PlanSelections = planSelection :: theCertificate.Certificate.PlanSelections } }

    updatedCertificate

[<When>]
let ``the member is enrolled`` (theCertificate : CertificateRecordDao) =
    let _ = (root.PutCertificate theCertificate).Result

    theCertificate

[<When>]
let ``it is terminated`` (theCertificate : CertificateRecordDao) =
    let certificate =
        { theCertificate with
           Status = CertificateStatus.Terminated
           Certificate =
             { theCertificate.Certificate with
                CertificateStatus = "terminated" } }

    let _ = (putCertificate certificate).Result

    certificate

[<When>]
let ``the books are closed for this month`` (theCertificate : CertificateRecordDao) =
    let now = nextBillingPeriod()
    let period =
        { Period.Start = now.AddMonths(-1).AddDays(1)
          End = now }

    let _ = (Api.root.CloseOutMonth theCertificate.Carrier period).Result

    (theCertificate, now)

[<Then>]
let ``it is metered for this month`` (theCertificate : CertificateRecordDao, endOfBillingPeriod: DateTime) =
    let usages = (Api.getBilling theCertificate.Carrier endOfBillingPeriod).Result

    let actual = usages |> Result.map (List.filter (isMetered theCertificate)) |> Result.defaultValue []

    actual |> should not' (haveLength 0)
    actual

[<Then>]
let ``it is not metered for this month`` (theCertificate : CertificateRecordDao, endOfBillingPeriod: DateTime) =
    let usages =(Api.getBilling theCertificate.Carrier endOfBillingPeriod).Result
    let actual = usages |> Result.map (List.filter (isMetered theCertificate)) |> Result.defaultValue []

    actual |> should haveLength 0
    actual

[<Then>]
let ``the no coverage (.*) option (.*) rate is (.*) CAD``
    (productLine : string)
    (option : string)
    (rate : decimal)
    (usages: UsageLineDao list)
    =
    let product = usages |> List.find (matchProduct None productLine option)
    product.CarrierRate |> should equal rate

    usages

[<Then>]
let ``the no coverage (.*) option (.*) volume is (.*) (.*)``
    (productLine : string)
    (option : string)
    (volume : decimal)
    (_: string)

    (usages: UsageLineDao list)
    =
    let product = usages |> List.find (matchProduct None productLine option)
    product.Volume |> should equal volume

    usages

[<Then>]
let ``the (.*) (.*) option (.*) rate is (.*) CAD``
    (coverage: string)
    (productLine : string)
    (option : string)
    (rate : decimal)
    (usages: UsageLineDao list)
    =
    let product = usages |> List.find (matchProduct (Some coverage) productLine option)
    product.CarrierRate |> should equal rate

    usages

[<Then>]
let ``the (.*) (.*) option (.*) volume is (.*) (.*)``
    (coverage: string)
    (productLine : string)
    (option : string)
    (volume : decimal)
    (_: string)
    (usages: UsageLineDao list)
    =
    let product = usages |> List.find (matchProduct (Some coverage) productLine option)
    product.Volume |> should equal volume

    usages

[<Then>]
let ``member benefits terminate`` () = failwith "Test Not Implemented"

[<Then>]
let ``member changes benefit period`` () = failwith "Test Not Implemented"

[<Then>]
let ``member hits reduction calc age`` () = failwith "Test Not Implemented"

[<Then>]
let ``member hits termination age`` () = failwith "Test Not Implemented"

[<Then>]
let ``member has address change`` () = failwith "Test Not Implemented"

[<Then>]
let ``member has salary increase`` () = failwith "Test Not Implemented"

[<Then>]
let ``member has salary decrease`` () = failwith "Test Not Implemented"

[<Then>]
let ``member is approved for coverage above NEM`` () = failwith "Test Not Implemented"
