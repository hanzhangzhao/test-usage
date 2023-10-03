module CertificateUsage.Tests.MemberListener.CarrierAliasMappingService_Tests

open Xunit
open Expecto

open CertificateUsage.Dto.Events.Dto

open CertificateUsage.Dependencies
open CertificateUsage.Listener.MembersListener.CarrierAliasMappingService
open CertificateUsage.Tests

let root : Root.Root =
    { InsertCertificateUsage = fun _ -> Stubs.Task.lift 1
      GetCertificate = fun _ -> Stubs.Task.lift None
      PutCertificate = fun _ -> Stubs.Task.lift 1
      EventStoreIO =
        { CreateMembersSubscription = fun _ -> Stubs.Task.lift ()
          SubscribeToMembersStream = fun _ -> Stubs.Task.lift ()
          CreateRateSubscription = fun _ -> Stubs.Task.lift ()
          SubscribeToRateStream = fun _ -> Stubs.Task.lift () }
      InsertRate = fun _ -> Stubs.Task.lift 1
      InsertRetroactiveCertificateUpdate = fun _ -> Stubs.Task.lift 1
      MapCarrierName = fun _ -> Stubs.Task.lift None }

module UpdateCarrierName =
    // forcing function to ensure we don't miss this test when adding new DTOs
    let updateCarrierName' root dto =
        match dto with
        | MemberEnrollmentDto _ -> updateCarrierName root dto
        | MemberReinstatementConfirmed _ -> updateCarrierName root dto
        | MemberTerminatedDto _ -> updateCarrierName root dto
        | MemberCancelledDto _ -> updateCarrierName root dto
        | MemberTaxProvinceUpdatedDto _ -> updateCarrierName root dto
        | MemberSpouseAddedDto _ -> updateCarrierName root dto
        | MemberDependentAddedDto _ -> updateCarrierName root dto
        | SpouseTerminatedDto _ -> updateCarrierName root dto
        | SpouseCohabUpdatedDto _ -> updateCarrierName root dto
        | MemberCobUpdatedDto _ -> updateCarrierName root dto
        | DependentCobUpdatedDto _ -> updateCarrierName root dto
        | SpouseCobUpdatedDto _ -> updateCarrierName root dto
        | DependentTerminatedDto _ -> updateCarrierName root dto
        | DependentPostSecondaryEducationUpdatedDto _ -> updateCarrierName root dto
        | DependentDisabilityUpdatedDto _ -> updateCarrierName root dto
        | MemberEmploymentUpdatedDto _ -> updateCarrierName root dto
        | MemberIncomeUpdatedDto _ -> updateCarrierName root dto
        | MemberEnrolledSnapshotDto _ -> updateCarrierName root dto

    module MemberEnrollmentDto =
        [<Fact>]
        let ``Update a MemberEnrollmentDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let dto =
                { Stubs.MemberEnrollment.Dto.stub with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName' root' (MemberEnrollmentDto dto)) with
                | MemberEnrollmentDto dto' -> Some dto'
                | _ -> None

            let expected = { dto with Carrier = "NewCarrier" }
            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberEnrollmentDto`` () =
            let dto =
                { Stubs.MemberEnrollment.Dto.stub with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName' root (MemberEnrollmentDto dto)) with
                | MemberEnrollmentDto dto' -> Some dto'
                | _ -> None

            let expected = dto
            Expect.equal actual (Some expected) "should equal"

    module MemberReinstatementConfirmed =
        [<Fact>]
        let ``Update a MemberReinstatementConfirmed carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let dto =
                { Stubs.MemberReinstatementConfirmed.Dto.stub with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName root' (MemberReinstatementConfirmed dto)) with
                | MemberReinstatementConfirmed dto' -> Some dto'
                | _ -> None

            let expected = { dto with Carrier = "NewCarrier" }
            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberReinstatementConfirmed`` () =
            let dto =
                { Stubs.MemberReinstatementConfirmed.Dto.stub with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName root (MemberReinstatementConfirmed dto)) with
                | MemberReinstatementConfirmed dto' -> Some dto'
                | _ -> None

            let expected = dto
            Expect.equal actual (Some expected) "should equal"

    module MemberTerminatedDto =
        [<Fact>]
        let ``Update a MemberTerminatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let dto =
                { Stubs.MemberTerminatedDto.dto with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName root' (MemberTerminatedDto dto)) with
                | MemberTerminatedDto dto' -> Some dto'
                | _ -> None

            let expected = { dto with Carrier = "NewCarrier" }
            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberTerminatedDto`` () =
            let dto =
                { Stubs.MemberTerminatedDto.dto with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName root (MemberTerminatedDto dto)) with
                | MemberTerminatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto
            Expect.equal actual (Some expected) "should equal"

    module MemberCancelledDto =
        [<Fact>]
        let ``Update a MemberCancelledDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let dto =
                { Stubs.MemberCancelledDto.dto with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName root' (MemberCancelledDto dto)) with
                | MemberCancelledDto dto' -> Some dto'
                | _ -> None

            let expected = { dto with Carrier = "NewCarrier" }
            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberCancelledDto`` () =
            let dto =
                { Stubs.MemberCancelledDto.dto with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName root (MemberCancelledDto dto)) with
                | MemberCancelledDto dto' -> Some dto'
                | _ -> None

            let expected = dto
            Expect.equal actual (Some expected) "should equal"

    module MemberTaxProvinceUpdatedDto =
        [<Fact>]
        let ``Update a MemberTaxProvinceUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.MemberTaxProvinceUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root' (MemberTaxProvinceUpdatedDto dto)) with
                | MemberTaxProvinceUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "NewCarrier" } } }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberTaxProvinceUpdatedDto`` () =
            let stub = Stubs.MemberTaxProvinceUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root (MemberTaxProvinceUpdatedDto dto)) with
                | MemberTaxProvinceUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module MemberSpouseAddedDto =
        [<Fact>]
        let ``Update a MemberSpouseAddedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.MemberSpouseAddedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root' (MemberSpouseAddedDto dto)) with
                | MemberSpouseAddedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "NewCarrier" } } ] }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberSpouseAddedDto`` () =
            let stub = Stubs.MemberSpouseAddedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root (MemberSpouseAddedDto dto)) with
                | MemberSpouseAddedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module MemberDependentAddedDto =
        [<Fact>]
        let ``Update a MemberDependentAddedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.MemberDependentAddedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root' (MemberDependentAddedDto dto)) with
                | MemberDependentAddedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "NewCarrier" } } ] }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberDependentAddedDto`` () =
            let stub = Stubs.MemberDependentAddedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root (MemberDependentAddedDto dto)) with
                | MemberDependentAddedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module SpouseTerminatedDto =
        [<Fact>]
        let ``Update a SpouseTerminatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.SpouseTerminatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root' (SpouseTerminatedDto dto)) with
                | SpouseTerminatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "NewCarrier" } } ] }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a SpouseTerminatedDto`` () =
            let stub = Stubs.SpouseTerminatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root (SpouseTerminatedDto dto)) with
                | SpouseTerminatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module SpouseCohabUpdatedDto =
        [<Fact>]
        let ``Update a SpouseCohabUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.SpouseCohabUpdatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root' (SpouseCohabUpdatedDto dto)) with
                | SpouseCohabUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "NewCarrier" } } ] }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a SpouseCohabUpdatedDto`` () =
            let stub = Stubs.SpouseCohabUpdatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root (SpouseCohabUpdatedDto dto)) with
                | SpouseCohabUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module MemberCobUpdatedDto =
        [<Fact>]
        let ``Update a MemberCobUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.MemberCobUpdatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root' (MemberCobUpdatedDto dto)) with
                | MemberCobUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "NewCarrier" } } ] }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberCobUpdatedDto`` () =
            let stub = Stubs.MemberCobUpdatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root (MemberCobUpdatedDto dto)) with
                | MemberCobUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module DependentCobUpdatedDto =
        [<Fact>]
        let ``Update a DependentCobUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.DependentCobUpdatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root' (DependentCobUpdatedDto dto)) with
                | DependentCobUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "NewCarrier" } } ] }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a DependentCobUpdatedDto`` () =
            let stub = Stubs.DependentCobUpdatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root (DependentCobUpdatedDto dto)) with
                | DependentCobUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module SpouseCobUpdatedDto =
        [<Fact>]
        let ``Update a SpouseCobUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.SpouseCobUpdatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root' (SpouseCobUpdatedDto dto)) with
                | SpouseCobUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "NewCarrier" } } ] }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a DependentCobUpdatedDto`` () =
            let stub = Stubs.SpouseCobUpdatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root (SpouseCobUpdatedDto dto)) with
                | SpouseCobUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module DependentTerminatedDto =
        [<Fact>]
        let ``Update a SpouseCobUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.DependentTerminatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root' (DependentTerminatedDto dto)) with
                | DependentTerminatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "NewCarrier" } } ] }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a DependentTerminatedDto`` () =
            let stub = Stubs.DependentTerminatedDto.dto

            let dto =
                { stub with
                    BenefitPeriods =
                        [ { stub.BenefitPeriods[0] with
                              ProductConfiguration =
                                  { stub.BenefitPeriods[0].ProductConfiguration with
                                      Carrier = "Carrier" } } ] }

            let actual =
                match (updateCarrierName root (DependentTerminatedDto dto)) with
                | DependentTerminatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module DependentPostSecondaryEducationUpdatedDto =
        [<Fact>]
        let ``Update a DependentPostSecondaryEducationUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.DependentPostSecondaryEducationUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root' (DependentPostSecondaryEducationUpdatedDto dto)) with
                | DependentPostSecondaryEducationUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "NewCarrier" } } }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a DependentPostSecondaryEducationUpdatedDto`` () =
            let stub = Stubs.DependentPostSecondaryEducationUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root (DependentPostSecondaryEducationUpdatedDto dto)) with
                | DependentPostSecondaryEducationUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module DependentDisabilityUpdatedDto =
        [<Fact>]
        let ``Update a DependentDisabilityUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.DependentDisabilityUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root' (DependentDisabilityUpdatedDto dto)) with
                | DependentDisabilityUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "NewCarrier" } } }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a DependentDisabilityUpdatedDto`` () =
            let stub = Stubs.DependentDisabilityUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root (DependentDisabilityUpdatedDto dto)) with
                | DependentDisabilityUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module MemberEmploymentUpdatedDto =
        [<Fact>]
        let ``Update a MemberEmploymentUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.MemberEmploymentUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root' (MemberEmploymentUpdatedDto dto)) with
                | MemberEmploymentUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "NewCarrier" } } }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberEmploymentUpdatedDto`` () =
            let stub = Stubs.MemberEmploymentUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root (MemberEmploymentUpdatedDto dto)) with
                | MemberEmploymentUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module MemberIncomeUpdatedDto =
        [<Fact>]
        let ``Update a MemberIncomeUpdatedDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let stub = Stubs.MemberIncomeUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root' (MemberIncomeUpdatedDto dto)) with
                | MemberIncomeUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "NewCarrier" } } }

            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberIncomeUpdatedDto`` () =
            let stub = Stubs.MemberIncomeUpdatedDto.dto

            let dto =
                { stub with
                    ActiveBenefitPeriod =
                        { stub.ActiveBenefitPeriod with
                            ProductConfiguration =
                                { stub.ActiveBenefitPeriod.ProductConfiguration with
                                    Carrier = "Carrier" } } }

            let actual =
                match (updateCarrierName root (MemberIncomeUpdatedDto dto)) with
                | MemberIncomeUpdatedDto dto' -> Some dto'
                | _ -> None

            let expected = dto

            Expect.equal actual (Some expected) "should equal"

    module MemberEnrolledSnapshotDto =
        [<Fact>]
        let ``Update a MemberEnrolledSnapshotDto carrier name`` =
            let root' =
                { root with
                    MapCarrierName = fun _ -> Stubs.Task.lift (Some "NewCarrier") }

            let dto =
                { Stubs.MemberEnrolledSnapshotDto.dto with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName' root' (MemberEnrolledSnapshotDto dto)) with
                | MemberEnrolledSnapshotDto dto' -> Some dto'
                | _ -> None

            let expected = { dto with Carrier = "NewCarrier" }
            Expect.equal actual (Some expected) "should equal"

        [<Fact>]
        let ``Use the default carrier name for a MemberEnrolledSnapshotDto`` () =
            let dto =
                { Stubs.MemberEnrolledSnapshotDto.dto with
                    Carrier = "Carrier" }

            let actual =
                match (updateCarrierName' root (MemberEnrolledSnapshotDto dto)) with
                | MemberEnrolledSnapshotDto dto' -> Some dto'
                | _ -> None

            let expected = dto
            Expect.equal actual (Some expected) "should equal"
