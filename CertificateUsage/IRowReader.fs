module CertificateUsage.IRowReader

open System

type IRowReader =
    abstract member text : string -> string
    abstract member textOrNone : string -> string option
    abstract member dateTime : string -> DateTime
    abstract member dateTimeOrNone : string -> DateTime option
    abstract member decimal : string -> decimal
    abstract member decimalOrNone : string -> decimal option
    abstract member int : string -> int
    abstract member uuid : string -> Guid
    abstract member uuidOrNone : string -> Guid option
    abstract member enum : string -> 'a

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "integration")>]
type NpgsqlFSharpRowReader(reader : RowReader) =
    interface IRowReader with
        member this.text column = reader.text column
        member this.textOrNone column = reader.textOrNone column
        member this.dateTime column = reader.dateTime column
        member this.dateTimeOrNone column = reader.dateTimeOrNone column
        member this.decimal column = reader.decimal column
        member this.decimalOrNone column = reader.decimalOrNone column
        member this.int column = reader.int column
        member ths.uuid column = reader.uuid column
        member ths.uuidOrNone column = reader.uuidOrNone column
        member ths.enum<'a> column = reader.fieldValue<'a> column
