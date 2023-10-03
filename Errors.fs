module CertificateUsage.Errors

open Logging

type Carrier = string
type Certificate = string
type ProductLineName = string
type ProductLineOption = string

type Errors =
    | RequiredField of string
    | InvalidDecimalValue of string * string
    | MissingEligibilityPeriods of string * int
    | MissingCertificate of string
    | CarrierMonthClosed of string * int * int
    | BadFormat of string
    | CertificateMissingPlanSelection of string * string * string * string

let toMessage =
    function
    | RequiredField name -> $"Missing required field '{name}'"
    | InvalidDecimalValue(field, value) -> $"Invalid decimal value {value} for {field}"
    | MissingEligibilityPeriods(missingField, dependentId) ->
        $"Missing eligibility periods;
             cannot determine {missingField} for dependent id {dependentId}"
    | MissingCertificate certificateNumber -> $"Missing certificate {certificateNumber}"
    | CarrierMonthClosed(carrier, year, month) ->
        $"""Usage already exist for {carrier} in {year}-{month |> sprintf "%02d"}"""
    | BadFormat message -> message
    | CertificateMissingPlanSelection(certificate, line, option, coverage) ->
        $"Missing plan section {{line={line},option={option},coverage={coverage}}} for certificate {certificate}"

let toErrorLog =
    function
    | RequiredField name -> logError "Missing required field {@name}" [ name ]
    | InvalidDecimalValue(field, value) -> logError "Invalid decimal value {@value} for {@field}" [ value; field ]
    | MissingEligibilityPeriods(missingField, dependentId) ->
        logError
            "Missing eligibility periods; cannot determine {@missingField} for dependent id {@dependentId}"
            [ missingField; dependentId |> string ]
    | MissingCertificate certificateNumber -> logError "Missing certificate {@certificateNumber}" [ certificateNumber ]
    | CarrierMonthClosed(carrier, year, month) ->
        logError
            "Usage already exist for {@carrier} in {@year}-{@month}}"
            [ carrier; year |> string; month |> sprintf "%02d" ]
    | BadFormat message -> logError message []
    | CertificateMissingPlanSelection(certificate, line, option, coverage) ->
        logError
            "Missing plan section {{line={@line},option={@option},coverage={@coverage}}} for certificate {@certificate}"
            [ line; option; coverage; certificate ]

exception InvalidCoverageTypeEnumException of string
exception InvalidCertificateUsageTypeEnumException of string
exception InvalidCertificateStatusEnumException of string
