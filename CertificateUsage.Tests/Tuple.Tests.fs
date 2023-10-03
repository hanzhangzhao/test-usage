module CertificateUsage.Tests.Tuple

open Xunit
open Expecto

open FsToolkit.ErrorHandling

open CertificateUsage.Tuple

module TraverseValidationFst =
    [<Fact>]
    let ``transverse (Ok _, _)`` () =
        let value = (Validation.Ok 1, 2)

        let actual = traverseValidationFst ((+) 1) value
        let expected = Validation.Ok(2, 2)

        Expect.equal actual expected "should equal"

    [<Fact>]
    let ``transverse (Error _, _)`` () =
        let value = (Validation.Error([ "error" ]), 2)

        let actual = traverseValidationFst ((+) 1) value
        let expected = Validation.Error [ "error" ]

        Expect.equal actual expected "should equal"

module Pipe =
    [<Fact>]
    let ``pipes successfully`` () =
        let testcase = ((+) 2, (*) 2)
        let actual = pipe testcase 3
        let expected = (5, 6)

        Expect.equal actual expected "should equal"
