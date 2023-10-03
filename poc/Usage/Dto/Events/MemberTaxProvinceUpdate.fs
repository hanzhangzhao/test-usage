module Usage.Dto.Events.MemberTaxProvinceUpdate

open System
open Update
open BenefitPeriod
open Employment

type MemberTaxProvinceDto =
    { TaxProvince: string
      EffectiveDate: DateTime }

type TaxProfileUpdateDto =
    { Member: MemberTaxProvinceDto
      Changes: UpdateDto list }

type MemberTaxProvinceUpdatedDto =
    { MemberId: int
      PublicId: string option
      ActiveBenefitPeriod: BenefitPeriodDto
      Employment: Employment option
      Update: TaxProfileUpdateDto }
