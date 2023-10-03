module Usage.Dto.Events.MemberIncomeUpdate

open System
open MemberAddress
open Update
open Income
open BenefitPeriod
open Employment

type IncomeUpdateDto =
    { Income: IncomeDto
      Changes: UpdateDto list }

type MemberIncomeUpdatedDto =
    { MemberId: int
      PublicId: string option
      MemberAddress: MemberAddress
      FirstName: string
      MiddleName: string option
      LastName: string
      DateOfBirth: DateTime option
      Gender: string
      Email: string option
      HomePhoneNumber: string option
      WorkPhoneNumber: string option
      MobilePhoneNumber: string option
      Employment: Employment option
      ActiveBenefitPeriod: BenefitPeriodDto
      Update: IncomeUpdateDto }
