module Feeds.Dto.Models.MemberBenefitClassTransferred

open System
open Usage.Dto.Events
open BenefitPeriod
open MemberAddress
open Employment
open Dependent
open Spouse
open CoverageCoordinationDto

type MemberBenefitClassTransferredDto =
    { MemberId: int
      PublicId: string option
      MemberAddress: MemberAddress
      Employment: Employment
      FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime option
      Gender: string
      Email: string option
      HomePhoneNumber: string option
      WorkPhoneNumber: string option
      MobilePhoneNumber: string option
      PreferredLanguage: string option
      Dependents: Dependent list
      Spouse: Spouse option
      CoverageCoordination: CoverageCoordinationDto option
      TaxProvince: string
      From: BenefitPeriodDto
      To: BenefitPeriodDto }