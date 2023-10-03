module CertificateUsage.Dto.Events.MemberTaxProvinceUpdate

open System
open BenefitPeriod
open Client
open PlanSelection

type MemberTaxProvinceDto =
    { TaxProvince: string
      EffectiveDate: DateTime }

type TaxProfileUpdateDto = { Member: MemberTaxProvinceDto }

type MemberTaxProvinceUpdatedDto =
    { ActiveBenefitPeriod: BenefitPeriodDto
      Client: Client
      PlanSelections: PlanSelectionDto list
      Update: TaxProfileUpdateDto }
