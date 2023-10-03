module CertificateUsage.Tests.ToDomain

open System
open Xunit
open Expecto

open CertificateUsage.Domain
open CertificateUsage.Errors
open CertificateUsage.ToDomain
open CertificateUsage.Dto.Events.MemberEnrolled
open CertificateUsage.Dto.Events.MemberReinstatementConfirmed
open CertificateUsage.Dto.Events.MemberTaxProvinceUpdate
open CertificateUsage.Dto.Events.Dto
open CertificateUsage.Dao
open CertificateUsage.Tests

module Stubs =
    let benefitStartDate = DateTime(2023, 6, 14)
    let taxProvince = "TaxProvince"
    let certificateNumber = "CertificateNumber"
    let scbPolicyNumber = "Number"
    let externalPolicyNumber = "PolicyNumber"
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

    let eventId = Guid.NewGuid()
    let eventType = "EventType"
    let version = "Version"
    let createDate = DateTime(2023, 06, 14)
    let eventNumber = 1UL
    let streamId = "StreamId"
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

    let memberReinstatementDto =
        { MemberReinstatementConfirmedDto.BenefitsStartDate = benefitStartDate
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

    let coveredCertificate =
        { CoveredCertificate.CertificateNumber = certificateNumber
          Carrier = carrier
          ClientName = clientName
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = externalPolicyNumber
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
                CarrierRate = 0.8m
                TaxRate = 1.3m
                TaxProvince = taxProvince } ] }

    let coveredEvent = coveredCertificate |> CoveredEvent

    let excludedCertificate =
        { ExcludedCertificate.CertificateNumber = certificateNumber
          Carrier = carrier
          ClientName = clientName
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = externalPolicyNumber
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
                CarrierRate = 0.8m
                TaxProvince = taxProvince
                TaxRate = 1.3m } ] }

    let excludedEvent = excludedCertificate |> ExclusionEvent

    let coveredDao =
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

    let excludedDao =
        { CertificateNumber = certificateNumber
          Carrier = carrier
          ScbPolicyNumber = scbPolicyNumber
          PolicyNumber = externalPolicyNumber
          Effective = benefitStartDate
          Type = CoverageType.Excluded
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

module MemberEnrollmentDtoToDomain =
    [<Fact>]
    let ``it successfully maps and MemberEnrollmentDto`` () =
        let actual = memberEnrollmentDtoToDomain Stubs.memberEnrollmentDto
        let expected = Ok Stubs.coveredEvent
        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a MemberEnrollmentDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.memberEnrollmentDto with
                CarrierClientCode = Some "CarrierClientCode"
                CarrierMapping = None }

        let actual = memberEnrollmentDtoToDomain dto

        let expected =
            { Stubs.coveredCertificate with
                Division = "CarrierClientCode" }
            |> CoveredEvent
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberEnrollmentDto with no division or carrier client code`` () =
        let dto =
            { Stubs.memberEnrollmentDto with
                CarrierMapping = Some(Map [ "", "" ]) }

        let actual = memberEnrollmentDtoToDomain dto

        let expected : Result<CertificateUsage, Errors list> =
            Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberEnrollmentDto with no Client.Name`` () =
        let dto =
            { Stubs.memberEnrollmentDto with
                Client = { Name = None } }

        let actual = memberEnrollmentDtoToDomain dto

        let expected : Result<CertificateUsage, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module MemberReinstatementConfirmedDtoToDomain =
    [<Fact>]
    let ``it successfully maps and MemberReinstatementConfirmedDtoToDomain`` () =
        let actual =
            Stubs.memberReinstatementDto |> MemberReinstatementConfirmed |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a MemberReinstatementConfirmedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.memberReinstatementDto with
                CarrierClientCode = Some "CarrierClientCode"
                CarrierMapping = None }

        let actual = dto |> MemberReinstatementConfirmed |> toDomain

        let expectedEvent =
            { Stubs.coveredCertificate with
                Division = "CarrierClientCode" }
            |> CoveredEvent

        let expected = Ok [ expectedEvent ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberReinstatementConfirmedDto with no division or carrier client code`` () =
        let dto =
            { Stubs.memberReinstatementDto with
                CarrierMapping = Some(Map [ "", "" ]) }

        let actual = dto |> MemberReinstatementConfirmed |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberReinstatementConfirmedDto with no Client.Name`` () =
        let dto =
            { Stubs.memberReinstatementDto with
                Client = { Name = None } }

        let actual = dto |> MemberReinstatementConfirmed |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module MemberCancelledDtoToDomain =
    [<Fact>]
    let ``should convert memberCancelledDto to domain`` () =
        let dto = Stubs.MemberCancelledDto.dto

        let expected : ExcludedCertificate =
            { CertificateNumber = dto.CertificateNumber
              Carrier = dto.Carrier
              ClientName = "Test"
              ScbPolicyNumber = dto.PolicyNumber
              PolicyNumber = dto.ExternalPolicyNumber.Value
              Division = dto.CarrierClientCode |> Option.defaultValue ""
              Effective = dto.BenefitsEndedDate.Value
              PlanSelections =
                [ { ProductLine = "ProductLine"
                    ProductLineGroup = "LineGroup"
                    Coverage = Some "Coverage"
                    Option = "Option"
                    RatePer = 1.1m
                    Volume = { Amount = 1.2m; Unit = "CAD" }
                    CarrierRate = 0.8m
                    TaxProvince = "TaxProvince"
                    TaxRate = 1.3m } ] }


        match dto |> MemberCancelledDto |> toDomain with
        | Error e -> failwith $"Should have succeeded. {e}"
        | Ok domain -> Expect.equal domain [ expected |> ExclusionEvent ] "should equal"

    [<Fact>]
    let ``should produce error if missing benefits ended date`` () =
        let dto =
            { Stubs.MemberCancelledDto.dto with
                BenefitsEndedDate = None }

        let expected = Errors.RequiredField "BenefitsEndedDate"

        match dto |> MemberCancelledDto |> toDomain with
        | Error e -> Expect.equal e [ expected ] "should equal"
        | Ok _ -> failwith "Should have failed"

    [<Fact>]
    let ``should produce error if missing division`` () =
        let dto =
            { Stubs.MemberCancelledDto.dto with
                CarrierClientCode = None }

        let expected = RequiredField "division"

        match dto |> MemberCancelledDto |> toDomain with
        | Error e -> Expect.equal e [ expected ] "should equal"
        | Ok _ -> failwith "Should have failed"

module MemberTerminatedDtoToDomain =
    [<Fact>]
    let ``should convert memberTerminatedDto to domain `` () =
        let dto = Stubs.MemberTerminatedDto.dto

        let expected : ExcludedCertificate =
            { CertificateNumber = dto.CertificateNumber
              Division = dto.CarrierClientCode |> Option.defaultValue ""
              Carrier = dto.Carrier
              ClientName = "Test"
              ScbPolicyNumber = dto.PolicyNumber
              PolicyNumber = dto.ExternalPolicyNumber.Value
              Effective = dto.BenefitsEndedDate.Value
              PlanSelections =
                [ { ProductLine = "ProductLine"
                    ProductLineGroup = "LineGroup"
                    Coverage = Some "Coverage"
                    Option = "Option"
                    RatePer = 1.1m
                    Volume = { Amount = 1.2m; Unit = "CAD" }
                    CarrierRate = 0.8m
                    TaxProvince = "TaxProvince"
                    TaxRate = 1.3m } ] }

        match dto |> MemberTerminatedDto |> toDomain with
        | Error e -> failwith $"Should have succeeded. {e}"
        | Ok domain -> Expect.equal domain [ expected |> ExclusionEvent ] "should equal"

    [<Fact>]
    let ``should produce error if missing benefits ended date`` () =
        let dto =
            { Stubs.MemberTerminatedDto.dto with
                BenefitsEndedDate = None }

        let expected = Errors.RequiredField "BenefitsEndedDate"

        match dto |> MemberTerminatedDto |> toDomain with
        | Error e -> Expect.equal e [ expected ] "should equal"
        | Ok _ -> failwith $"Should have failed"

module MemberEnrolledSnapshotDtoToDomain =
    [<Fact>]
    let ``it successfully maps MemberEnrolledSnapshotDto`` () =
        let dto = Stubs.MemberEnrolledSnapshotDto.dto

        let expected : CoveredCertificate =
            { CertificateNumber = dto.CertificateNumber
              Division = dto.CarrierClientCode |> Option.defaultValue ""
              Carrier = dto.Carrier
              ClientName = "Test"
              ScbPolicyNumber = dto.PolicyNumber
              PolicyNumber = dto.ExternalPolicyNumber.Value
              Effective = dto.BenefitsStartDate
              PlanSelections = [] }

        match dto |> MemberEnrolledSnapshotDto |> toDomain with
        | Error e -> failwith $"Should have succeeded. {e}"
        | Ok domain -> Expect.equal domain ([ expected ] |> List.map CoveredEvent) "should equal"

module FromDaoToCoveredEvent =
    [<Fact>]
    let ``transforms a CertificateUsageChangeDao to a CoveredEvent domain model`` () =
        let dao = Stubs.coveredDao
        let actual = fromDao dao
        let expected = Stubs.coveredEvent
        Expect.equal actual expected ""

    [<Fact>]
    let ``transforms a CertificateUsageChangeDao to a ExclusionEvent domain model`` () =
        let dao = Stubs.excludedDao
        let actual = fromDao dao
        let expected = Stubs.excludedEvent
        Expect.equal actual expected ""

    [<Fact>]
    let ``raises an exception given a invalid usage type`` () =
        let dao = { Stubs.excludedDao with Type = enum 3 }
        Expect.throws (fun _ -> fromDao dao |> ignore) ""

module MemberTaxProvinceUpdatedDto =
    [<Fact>]
    let ``it successfully maps and MemberTaxProvinceUpdatedDto`` () =
        let actual =
            Stubs.MemberTaxProvinceUpdatedDto.dto |> MemberTaxProvinceUpdatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and MemberTaxProvinceUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.MemberTaxProvinceUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.MemberTaxProvinceUpdatedDto.dto.ActiveBenefitPeriod with
                        CarrierMapping = Some(Map [ "division", "division" ]) } }

        let actual = dto |> MemberTaxProvinceUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a MemberTaxProvinceUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.MemberTaxProvinceUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.MemberTaxProvinceUpdatedDto.dto.ActiveBenefitPeriod with
                        ProductConfiguration =
                            { Stubs.MemberTaxProvinceUpdatedDto.dto.ActiveBenefitPeriod.ProductConfiguration with
                                CarrierClientCode = None }
                        CarrierMapping = None } }

        let actual = dto |> MemberTaxProvinceUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberTaxProvinceUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.MemberTaxProvinceUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> MemberTaxProvinceUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module MemberDependentAddedDto =
    [<Fact>]
    let ``it successfully maps and MemberDependentAddedDto`` () =
        let actual =
            Stubs.MemberDependentAddedDto.dto |> MemberDependentAddedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and MemberDependentAddedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.MemberDependentAddedDto.dto with
                BenefitPeriods =
                    [ { Stubs.MemberDependentAddedDto.dto.BenefitPeriods[0] with
                          CarrierMapping = Some(Map [ "division", "division" ]) } ] }

        let actual = dto |> MemberDependentAddedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a MemberDependentAddedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.MemberDependentAddedDto.dto with
                BenefitPeriods =
                    [ { Stubs.MemberDependentAddedDto.dto.BenefitPeriods[0] with
                          ProductConfiguration =
                              { Stubs.MemberDependentAddedDto.dto.BenefitPeriods[0].ProductConfiguration with
                                  CarrierClientCode = None }
                          CarrierMapping = None } ] }

        let actual = dto |> MemberDependentAddedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberDependentAddedDto with no Client.Name`` () =
        let dto =
            { Stubs.MemberDependentAddedDto.dto with
                Client = { Name = None } }

        let actual = dto |> MemberDependentAddedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module MemberSpouseAddedDto =
    [<Fact>]
    let ``it successfully maps and MemberSpouseAddedDto`` () =
        let actual = Stubs.MemberSpouseAddedDto.dto |> MemberSpouseAddedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""

    [<Fact>]
    let ``it defaults null carrier_rate to zero`` () =
        let planSelection =
            { Stubs.MemberSpouseAddedDto.dto.PlanSelections[0] with
                CarrierRate = null }

        let stub =
            { Stubs.MemberSpouseAddedDto.dto with
                PlanSelections = [ planSelection ] }

        let actual = stub |> MemberSpouseAddedDto |> toDomain

        let expectedPlanSelections =
            { Stubs.coveredCertificate.PlanSelections[0] with
                CarrierRate = 0.0m }

        let expectedCoveredCert =
            { Stubs.coveredCertificate with
                PlanSelections = [ expectedPlanSelections ] }

        let expected = Ok [ CoveredEvent expectedCoveredCert ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``it successfully maps and MemberSpouseAddedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.MemberSpouseAddedDto.dto with
                BenefitPeriods =
                    [ { Stubs.MemberSpouseAddedDto.dto.BenefitPeriods[0] with
                          CarrierMapping = Some(Map [ "division", "division" ]) } ] }

        let actual = dto |> MemberSpouseAddedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a MemberSpouseAddedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.MemberSpouseAddedDto.dto with
                BenefitPeriods =
                    [ { Stubs.MemberSpouseAddedDto.dto.BenefitPeriods[0] with
                          ProductConfiguration =
                              { Stubs.MemberSpouseAddedDto.dto.BenefitPeriods[0].ProductConfiguration with
                                  CarrierClientCode = None }
                          CarrierMapping = None } ] }

        let actual = dto |> MemberSpouseAddedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberSpouseAddedDto with no Client.Name`` () =
        let dto =
            { Stubs.MemberSpouseAddedDto.dto with
                Client = { Name = None } }

        let actual = dto |> MemberSpouseAddedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module SpouseTerminatedDto =
    [<Fact>]
    let ``it successfully maps and SpouseTerminatedDto`` () =
        let actual = Stubs.SpouseTerminatedDto.dto |> SpouseTerminatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and SpouseTerminatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.SpouseTerminatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.SpouseTerminatedDto.dto.BenefitPeriods[0] with
                          CarrierMapping = Some(Map [ "division", "division" ]) } ] }

        let actual = dto |> SpouseTerminatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a SpouseTerminatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.SpouseTerminatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.SpouseTerminatedDto.dto.BenefitPeriods[0] with
                          ProductConfiguration =
                              { Stubs.SpouseTerminatedDto.dto.BenefitPeriods[0].ProductConfiguration with
                                  CarrierClientCode = None }
                          CarrierMapping = None } ] }

        let actual = dto |> SpouseTerminatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a SpouseTerminatedDto with no Client.Name`` () =
        let dto =
            { Stubs.SpouseTerminatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> SpouseTerminatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module DependentTerminatedDto =
    [<Fact>]
    let ``it successfully maps and DependentTerminatedDto`` () =
        let actual = Stubs.DependentTerminatedDto.dto |> DependentTerminatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and DependentTerminatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.DependentTerminatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.DependentTerminatedDto.dto.BenefitPeriods[0] with
                          CarrierMapping = Some(Map [ "division", "division" ]) } ] }

        let actual = dto |> DependentTerminatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a DependentTerminatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.DependentTerminatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.DependentTerminatedDto.dto.BenefitPeriods[0] with
                          ProductConfiguration =
                              { Stubs.DependentTerminatedDto.dto.BenefitPeriods[0].ProductConfiguration with
                                  CarrierClientCode = None }
                          CarrierMapping = None } ] }

        let actual = dto |> DependentTerminatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a DependentTerminatedDto with no Client.Name`` () =
        let dto =
            { Stubs.DependentTerminatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> DependentTerminatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module MemberCobUpdatedDto =
    [<Fact>]
    let ``it successfully maps and MemberCobUpdatedDto`` () =
        let actual = Stubs.MemberCobUpdatedDto.dto |> MemberCobUpdatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and MemberCobUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.MemberCobUpdatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.MemberCobUpdatedDto.dto.BenefitPeriods[0] with
                          CarrierMapping = Some(Map [ "division", "division" ]) } ] }

        let actual = dto |> MemberCobUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a MemberCobUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.MemberCobUpdatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.MemberCobUpdatedDto.dto.BenefitPeriods[0] with
                          ProductConfiguration =
                              { Stubs.MemberCobUpdatedDto.dto.BenefitPeriods[0].ProductConfiguration with
                                  CarrierClientCode = None }
                          CarrierMapping = None } ] }

        let actual = dto |> MemberCobUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberCobUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.MemberCobUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> MemberCobUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module SpouseCobUpdatedDto =
    [<Fact>]
    let ``it successfully maps and SpouseCobUpdatedDto`` () =
        let actual = Stubs.SpouseCobUpdatedDto.dto |> SpouseCobUpdatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and SpouseCobUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.SpouseCobUpdatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.SpouseCobUpdatedDto.dto.BenefitPeriods[0] with
                          CarrierMapping = Some(Map [ "division", "division" ]) } ] }

        let actual = dto |> SpouseCobUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a SpouseCobUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.SpouseCobUpdatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.SpouseCobUpdatedDto.dto.BenefitPeriods[0] with
                          ProductConfiguration =
                              { Stubs.SpouseCobUpdatedDto.dto.BenefitPeriods[0].ProductConfiguration with
                                  CarrierClientCode = None }
                          CarrierMapping = None } ] }

        let actual = dto |> SpouseCobUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a SpouseCobUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.SpouseCobUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> SpouseCobUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module DependentCobUpdatedDto =
    [<Fact>]
    let ``it successfully maps and DependentCobUpdatedDto`` () =
        let actual = Stubs.DependentCobUpdatedDto.dto |> DependentCobUpdatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and DependentCobUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.DependentCobUpdatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.DependentCobUpdatedDto.dto.BenefitPeriods[0] with
                          CarrierMapping = Some(Map [ "division", "division" ]) } ] }

        let actual = dto |> DependentCobUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a DependentCobUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.DependentCobUpdatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.DependentCobUpdatedDto.dto.BenefitPeriods[0] with
                          ProductConfiguration =
                              { Stubs.DependentCobUpdatedDto.dto.BenefitPeriods[0].ProductConfiguration with
                                  CarrierClientCode = None }
                          CarrierMapping = None } ] }

        let actual = dto |> DependentCobUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a DependentCobUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.DependentCobUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> DependentCobUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module SpouseCohabUpdatedDto =
    [<Fact>]
    let ``it successfully maps and SpouseCohabUpdatedDto`` () =
        let actual = Stubs.SpouseCohabUpdatedDto.dto |> SpouseCohabUpdatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and SpouseCohabUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.SpouseCohabUpdatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.SpouseCohabUpdatedDto.dto.BenefitPeriods[0] with
                          CarrierMapping = Some(Map [ "division", "division" ]) } ] }

        let actual = dto |> SpouseCohabUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a SpouseCohabUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.SpouseCohabUpdatedDto.dto with
                BenefitPeriods =
                    [ { Stubs.SpouseCohabUpdatedDto.dto.BenefitPeriods[0] with
                          ProductConfiguration =
                              { Stubs.SpouseCohabUpdatedDto.dto.BenefitPeriods[0].ProductConfiguration with
                                  CarrierClientCode = None }
                          CarrierMapping = None } ] }

        let actual = dto |> SpouseCohabUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a SpouseCohabUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.SpouseCohabUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> SpouseCohabUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""


    [<Fact>]
    let ``defaults Cohabitation Date to now`` () =
        let dto =
            { Stubs.SpouseCohabUpdatedDto.dto with
                Spouse = { CohabitationDate = None } }

        let domain = dto |> SpouseCohabUpdatedDto |> toDomain

        let actual =
            match domain with
            | Ok value -> (DateTime.Now - value.Head.Effective).Seconds
            | _ -> failwith "Expected valid domain model"

        let expected = 1
        Expect.isLessThan actual expected ""

module DependentPostSecondaryEducationUpdatedDto =
    [<Fact>]
    let ``it successfully maps and DependentPostSecondaryEducationUpdatedDto`` () =
        let actual =
            Stubs.DependentPostSecondaryEducationUpdatedDto.dto
            |> DependentPostSecondaryEducationUpdatedDto
            |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and DependentPostSecondaryEducationUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.DependentPostSecondaryEducationUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.DependentPostSecondaryEducationUpdatedDto.dto.ActiveBenefitPeriod with
                        CarrierMapping = Some(Map [ "division", "division" ]) } }

        let actual = dto |> DependentPostSecondaryEducationUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a DependentPostSecondaryEducationUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.DependentPostSecondaryEducationUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.DependentPostSecondaryEducationUpdatedDto.dto.ActiveBenefitPeriod with
                        ProductConfiguration =
                            { Stubs.DependentPostSecondaryEducationUpdatedDto.dto.ActiveBenefitPeriod.ProductConfiguration with
                                CarrierClientCode = None }
                        CarrierMapping = None } }

        let actual = dto |> DependentPostSecondaryEducationUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a DependentPostSecondaryEducationUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.DependentPostSecondaryEducationUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> DependentPostSecondaryEducationUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module DependentDisabilityUpdatedDto =
    [<Fact>]
    let ``it successfully maps and DependentDisabilityUpdatedDto`` () =
        let actual =
            Stubs.DependentDisabilityUpdatedDto.dto
            |> DependentDisabilityUpdatedDto
            |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and DependentDisabilityUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.DependentDisabilityUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.DependentDisabilityUpdatedDto.dto.ActiveBenefitPeriod with
                        CarrierMapping = Some(Map [ "division", "division" ]) } }

        let actual = dto |> DependentDisabilityUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a DependentDisabilityUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.DependentDisabilityUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.DependentDisabilityUpdatedDto.dto.ActiveBenefitPeriod with
                        ProductConfiguration =
                            { Stubs.DependentDisabilityUpdatedDto.dto.ActiveBenefitPeriod.ProductConfiguration with
                                CarrierClientCode = None }
                        CarrierMapping = None } }

        let actual = dto |> DependentDisabilityUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a DependentDisabilityUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.DependentDisabilityUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> DependentDisabilityUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module MemberEmploymentUpdatedDto =
    [<Fact>]
    let ``it successfully maps and MemberEmploymentUpdatedDto`` () =
        let actual =
            Stubs.MemberEmploymentUpdatedDto.dto |> MemberEmploymentUpdatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and MemberEmploymentUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.MemberEmploymentUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.MemberEmploymentUpdatedDto.dto.ActiveBenefitPeriod with
                        CarrierMapping = Some(Map [ "division", "division" ]) } }

        let actual = dto |> MemberEmploymentUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a MemberEmploymentUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.MemberEmploymentUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.MemberEmploymentUpdatedDto.dto.ActiveBenefitPeriod with
                        ProductConfiguration =
                            { Stubs.MemberEmploymentUpdatedDto.dto.ActiveBenefitPeriod.ProductConfiguration with
                                CarrierClientCode = None }
                        CarrierMapping = None } }

        let actual = dto |> MemberEmploymentUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberEmploymentUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.MemberEmploymentUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> MemberEmploymentUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module MemberIncomeUpdatedDto =
    [<Fact>]
    let ``it successfully maps and MemberIncomeUpdatedDto`` () =
        let actual = Stubs.MemberIncomeUpdatedDto.dto |> MemberIncomeUpdatedDto |> toDomain

        let expected = Ok [ Stubs.coveredEvent ]
        Expect.equal actual expected ""


    [<Fact>]
    let ``it successfully maps and MemberIncomeUpdatedDto with division as a carrier mapping`` () =
        let dto =
            { Stubs.MemberIncomeUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.MemberIncomeUpdatedDto.dto.ActiveBenefitPeriod with
                        CarrierMapping = Some(Map [ "division", "division" ]) } }

        let actual = dto |> MemberIncomeUpdatedDto |> toDomain

        let expected =
            [ { Stubs.coveredCertificate with
                  Division = "division" }
              |> CoveredEvent ]
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``error outs with a MemberEmploymentUpdatedDto with no division and a carrier client code`` () =
        let dto =
            { Stubs.MemberIncomeUpdatedDto.dto with
                ActiveBenefitPeriod =
                    { Stubs.MemberIncomeUpdatedDto.dto.ActiveBenefitPeriod with
                        ProductConfiguration =
                            { Stubs.MemberIncomeUpdatedDto.dto.ActiveBenefitPeriod.ProductConfiguration with
                                CarrierClientCode = None }
                        CarrierMapping = None } }

        let actual = dto |> MemberIncomeUpdatedDto |> toDomain
        let expected = Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberIncomeUpdatedDto with no Client.Name`` () =
        let dto =
            { Stubs.MemberIncomeUpdatedDto.dto with
                Client = { Name = None } }

        let actual = dto |> MemberIncomeUpdatedDto |> toDomain

        let expected : Result<CertificateUsage list, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module CarrierRateModifiedDtoToDomain =
    [<Fact>]
    let ``transform a CarrierRateModifiedDto to a CarrierRateModification domain model`` () =
        let dto = Stubs.CarrierRateModifiedDto.dto |> CarrierRateModifiedDto

        let expected : CarrierRate.CarrierRateModification =
            { CarrierRate.CarrierRateModification.Carrier = "Carrier"
              PolicyNumber = "PolicyNumber"
              Option = "Option"
              Coverage = Some "Coverage"
              ProductLine = "ProductLine"
              Effective = Stubs.CarrierRateModifiedDto.effectiveDate
              CarrierRate = 1.23m
              ChangedBy = { Id = 2; Name = "ChangedBy.Name" } }

        let actual = Rate.toDomain dto
        Expect.equal actual (Ok [ expected ]) ""

module MemberRateChangedDtoToDomain =
    [<Fact>]
    let ``it successfully maps and MemberRateChangedDto`` () =
        let actual = memberRateChangedDtoToDomain Stubs.MemberRateChanged.Dto.stub

        let expected =
            { Stubs.coveredCertificate with
                CertificateNumber = Stubs.MemberRateChanged.certificateNumber
                Carrier = Stubs.MemberRateChanged.carrier
                ClientName = Stubs.MemberRateChanged.clientName
                ScbPolicyNumber = Stubs.MemberRateChanged.policyNumber
                PolicyNumber = Stubs.MemberRateChanged.externalPolicyNumber
                Effective = Stubs.MemberRateChanged.benefitStartDate
                Division = Stubs.MemberRateChanged.carrierClientCode
                PlanSelections =
                    [ { ProductLine = Stubs.MemberRateChanged.productLine
                        ProductLineGroup = Stubs.MemberRateChanged.productLineGroup
                        Coverage = Some Stubs.MemberRateChanged.coverage
                        Option = Stubs.MemberRateChanged.option
                        RatePer = Stubs.MemberRateChanged.ratePer
                        Volume =
                          { Amount = Stubs.MemberRateChanged.volumeAmount
                            Unit = Stubs.MemberRateChanged.volumeUnit }
                        CarrierRate = Stubs.MemberRateChanged.carrierRate
                        TaxRate = Stubs.MemberRateChanged.taxRate
                        TaxProvince = Stubs.MemberRateChanged.taxProvince } ] }
            |> CoveredEvent
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``it successfully maps and MemberRateChangedDto no division but with a carrier client code`` () =
        let dto =
            { Stubs.MemberRateChanged.Dto.stub with
                CarrierClientCode = Some "CarrierClientCode"
                CarrierMapping = None }

        let actual = memberRateChangedDtoToDomain dto

        let expected =
            { Stubs.coveredCertificate with
                CertificateNumber = Stubs.MemberRateChanged.certificateNumber
                Carrier = Stubs.MemberRateChanged.carrier
                ClientName = Stubs.MemberRateChanged.clientName
                ScbPolicyNumber = Stubs.MemberRateChanged.policyNumber
                PolicyNumber = Stubs.MemberRateChanged.externalPolicyNumber
                Effective = Stubs.MemberRateChanged.benefitStartDate
                Division = Stubs.MemberRateChanged.carrierClientCode
                PlanSelections =
                    [ { ProductLine = Stubs.MemberRateChanged.productLine
                        ProductLineGroup = Stubs.MemberRateChanged.productLineGroup
                        Coverage = Some Stubs.MemberRateChanged.coverage
                        Option = Stubs.MemberRateChanged.option
                        RatePer = Stubs.MemberRateChanged.ratePer
                        Volume =
                          { Amount = Stubs.MemberRateChanged.volumeAmount
                            Unit = Stubs.MemberRateChanged.volumeUnit }
                        CarrierRate = Stubs.MemberRateChanged.carrierRate
                        TaxRate = Stubs.MemberRateChanged.taxRate
                        TaxProvince = Stubs.MemberRateChanged.taxProvince } ] }
            |> CoveredEvent
            |> Ok

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberRateChangedDto with no division or carrier client code`` () =
        let dto =
            { Stubs.MemberRateChanged.Dto.stub with
                CarrierClientCode = None
                CarrierMapping = Some(Map [ "", "" ]) }

        let actual = memberRateChangedDtoToDomain dto

        let expected : Result<CertificateUsage, Errors list> =
            Error [ (RequiredField "division") ]

        Expect.equal actual expected ""

    [<Fact>]
    let ``errors out with a MemberRateChangedDto with no Client.Name`` () =
        let dto =
            { Stubs.MemberRateChanged.Dto.stub with
                Client = { Name = None } }

        let actual = memberRateChangedDtoToDomain dto

        let expected : Result<CertificateUsage, Errors list> =
            Error [ (RequiredField "Client.Name") ]

        Expect.equal actual expected ""

module Division =
    [<Fact>]
    let ``it gets the division from the carrier mapping`` () =
        let carrierMapping = Map.ofSeq (seq { ("division", "001") }) |> Some
        let actual = division carrierMapping None
        let expected = Ok "001"

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``it gets the division from the carrier client code if there is no carrier mapping`` () =
        let carrierMapping = None
        let actual = division carrierMapping (Some "001")
        let expected = Ok "001"

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``it gets the division from the carrier client code if there is no division in the carrier mapping`` () =
        let carrierMapping = Map.ofSeq (seq { () }) |> Some
        let actual = division carrierMapping (Some "001")
        let expected = Ok "001"

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``it returns a required field error if there is no division in the carrier mapping or carrier client code`` () =
        let carrierMapping = Map.ofSeq (seq { () }) |> Some
        let actual = division carrierMapping None
        let expected = Error(RequiredField "division")

        Expect.equal actual expected "should equal"

[<Fact>]
let ``it returns a required field error if there is no carrier mapping or carrier client code`` () =
    let carrierMapping = None
    let actual = division carrierMapping None
    let expected = Error(RequiredField "division")

    Expect.equal actual expected "should equal"
