module CertificateUsage.Tests.Dao

open Xunit
open Expecto

open CertificateUsage
open CertificateUsage.Dao

module CertificateStatus =
    module FromString =
        [<Fact>]
        let ``'active' return Active`` () =
            let actual = CertificateStatus.fromString "active"
            let expected = CertificateStatus.Active

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``'terminated' return Terminated`` () =
            let actual = CertificateStatus.fromString "terminated"
            let expected = CertificateStatus.Terminated

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``not 'active' or 'terminated' throws an exception`` () =
            Expect.throws (fun () -> (CertificateStatus.fromString "invalid") |> ignore) "should throw"

module CertificateUsageType =
    module Value =
        [<Fact>]
        let ``map a CertificateUsageType.Usage to usage`` () =
            let actual = CertificateUsageType.value CertificateUsageType.Charge
            let expected = "charge"
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``map a CertificateUsageType.Reversal to reversal`` () =
            let actual = CertificateUsageType.value CertificateUsageType.Reversal
            let expected = "reversal"
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``map a CertificateUsageType.Correction to correction`` () =
            let actual = CertificateUsageType.value CertificateUsageType.Correction
            let expected = "correction"
            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``fail when mapping an invalid CertificateUsageType enum value`` () =
            let testcase () =
                CertificateUsageType.value (enum<CertificateUsageType> (999999)) |> ignore
                ()

            Expect.throws testcase "should throw"

module RetroactiveCertificateUpdateType =
    module FromDomain =
        [<Fact>]
        let ``maps (domain) Enrollment to (dao) Enrollment`` () =
            let testcase = Domain.RetroactiveCertificateUpdateType.Enrollment
            let actual = RetroactiveCertificateUpdateType.fromDomain testcase
            let expected = RetroactiveCertificateUpdateType.Enrollment

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``maps (domain) Termination to (dao) Termination`` () =
            let testcase = Domain.RetroactiveCertificateUpdateType.Termination
            let actual = RetroactiveCertificateUpdateType.fromDomain testcase
            let expected = RetroactiveCertificateUpdateType.Termination

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``maps (domain) Update to (dao) Update`` () =
            let testcase = Domain.RetroactiveCertificateUpdateType.Update
            let actual = RetroactiveCertificateUpdateType.fromDomain testcase
            let expected = RetroactiveCertificateUpdateType.Update

            Expect.equal actual expected "should equal"
