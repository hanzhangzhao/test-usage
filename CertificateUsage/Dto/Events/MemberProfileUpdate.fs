module CertificateUsage.Dto.Events.MemberProfileUpdate

open System
open Update
open BenefitPeriod
open Employment
open System.Text.Json.Serialization

type MemberProfileDto =
    { PreferredContactMethod: string option
      FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime option
      Gender: string
      Email: string option
      AlternateEmail: string option
      KnownAs: string option
      HomePhoneNumber: string option
      WorkPhoneNumber: string option
      MobilePhoneNumber: string option
      PreferredLanguage: string option
      DrugCardDescription: string option
      [<JsonPropertyName("address_1")>]
      AddressLine1: string option
      [<JsonPropertyName("address_2")>]
      AddressLine2: string option
      City: string option
      State: string option
      PostalCode: string option
      Country: string option }

type MemberProfileUpdateDto =
    { Member: MemberProfileDto
      Changes: UpdateDto list }

type MemberProfileUpdatedDto =
    { MemberId: int
      PublicId: string option
      ActiveBenefitPeriod: BenefitPeriodDto
      Employment: Employment option
      Update: MemberProfileUpdateDto }
