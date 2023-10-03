module Usage.Dto.Events.MemberAddress

open System.Text.Json.Serialization

type MemberAddress =
    { [<JsonPropertyName("address_line1")>]
      AddressLine1: string option
      [<JsonPropertyName("address_line2")>]
      AddressLine2: string option
      City: string option
      Province: string option
      PostalCode: string option
      Country: string option }
    