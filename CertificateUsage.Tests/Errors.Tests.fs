module CertificateUsage.Tests.Errors

open Serilog.Sinks.TestCorrelator
open Xunit
open Expecto

open CertificateUsage.Errors

module ToMessage =
    [<Fact>]
    let ``generates the correct message for a RequireField error`` () =
        let actual = RequiredField "Field" |> toMessage

        let expected = "Missing required field 'Field'"
        Expect.equal actual expected ""

    [<Fact>]
    let ``generates the correct message for a InvalidDecimalValue error`` () =
        let actual = InvalidDecimalValue("Field", "Value") |> toMessage

        let expected = "Invalid decimal value Value for Field"
        Expect.equal actual expected "Should Equal"

    [<Fact>]
    let ``generates the correct message for a MissingEligibilityPeriods error`` () =
        let actual = MissingEligibilityPeriods("Field", 1) |> toMessage

        let expected =
            "Missing eligibility periods;
             cannot determine Field for dependent id 1"

        Expect.equal actual expected "Should Equal"

    [<Fact>]
    let ``generates the correct message for a MissingCertificate error`` () =
        let actual = MissingCertificate("CertificateNumber") |> toMessage

        let expected = "Missing certificate CertificateNumber"
        Expect.equal actual expected "Should Equal"

    [<Fact>]
    let ``generates the correct message for a CarrierMonthClosed error`` () =
        let actual = CarrierMonthClosed("carrier", 2023, 8) |> toMessage

        let expected = "Usage already exist for carrier in 2023-08"
        Expect.equal actual expected "Should Equal"

    [<Fact>]
    let ``BadFormat`` () =
        let actual = BadFormat("passthrough") |> toMessage

        let expected = "passthrough"
        Expect.equal actual expected "Should Equal"

    [<Fact>]
    let ``CertificateMissingPlanSelection`` () =
        let actual = CertificateMissingPlanSelection ("a", "b", "c", "d") |> toMessage

        let expected = "Missing plan section {line=b,option=c,coverage=d} for certificate a"
        Expect.equal actual expected "Should Equal"

module ToErrorLog =
    open Serilog

    let withTestLogger (test : unit -> unit) =
        Log.Logger <- LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger()
        use logger = TestCorrelator.CreateContext()
        test ()

    [<Fact>]
    let ``Should log correct message for RequireField`` () =
        withTestLogger (fun _ ->
            RequiredField "FirstName" |> toErrorLog

            let actual = TestCorrelator.GetLogEventsFromCurrentContext() |> Seq.head

            Expect.equal actual.MessageTemplate.Text "Missing required field {@name}" "Should equal")

    [<Fact>]
    let ``Should log correct message for InvalidDecimalValue`` () =
        withTestLogger (fun _ ->
            InvalidDecimalValue("field", "value") |> toErrorLog

            let actual = TestCorrelator.GetLogEventsFromCurrentContext() |> Seq.head

            Expect.equal actual.MessageTemplate.Text "Invalid decimal value {@value} for {@field}" "Should equal")

    [<Fact>]
    let ``logs correct message for MissingEligibilityPeriods`` () =
        withTestLogger (fun _ ->
            MissingEligibilityPeriods("Field", 1) |> toErrorLog

            let actual = TestCorrelator.GetLogEventsFromCurrentContext() |> Seq.head

            Expect.equal
                actual.MessageTemplate.Text
                "Missing eligibility periods; cannot determine {@missingField} for dependent id {@dependentId}"
                "Should equal")

    [<Fact>]
    let ``logs correct message for MissingCertificate`` () =
        withTestLogger (fun _ ->
            MissingCertificate "CertificateNumber" |> toErrorLog

            let actual = TestCorrelator.GetLogEventsFromCurrentContext() |> Seq.head

            Expect.equal actual.MessageTemplate.Text "Missing certificate {@certificateNumber}" "Should equal")

    [<Fact>]
    let ``logs correct message for a CarrierMonthClosed error`` () =
        withTestLogger (fun _ ->
            CarrierMonthClosed("carrier", 2023, 8) |> toErrorLog

            let expected = "Usage already exist for {@carrier} in {@year}-{@month}}"
            let actual = TestCorrelator.GetLogEventsFromCurrentContext() |> Seq.head

            Expect.equal actual.MessageTemplate.Text expected "Should equal")

    [<Fact>]
    let ``BadFormat`` () =
        withTestLogger (fun _ ->
            BadFormat "passthrough" |> toErrorLog

            let actual = TestCorrelator.GetLogEventsFromCurrentContext() |> Seq.head
            let expected = "passthrough"
            Expect.equal actual.MessageTemplate.Text expected "Should Equal")
    [<Fact>]
    let ``CertificateMissingPlanSelection`` () =
        withTestLogger (fun _ ->
            CertificateMissingPlanSelection ("a", "b", "c", "d") |> toErrorLog

            let log = TestCorrelator.GetLogEventsFromCurrentContext() |> Seq.head
            let actual = log.MessageTemplate.Text
            let expected = "Missing plan section {{line={@line},option={@option},coverage={@coverage}}} for certificate {@certificate}"

            Expect.equal actual expected "Should Equal"
        )
