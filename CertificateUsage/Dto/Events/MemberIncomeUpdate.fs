module CertificateUsage.Dto.Events.MemberIncomeUpdate

open CertificateUsage.Dto.Events.Income
open CertificateUsage.Dto.Events.BenefitPeriod
open CertificateUsage.Dto.Events.Client
open CertificateUsage.Dto.Events.PlanSelection

type IncomeUpdateDto = { Income: IncomeDto }

type MemberIncomeUpdatedDto =
    { ActiveBenefitPeriod: BenefitPeriodDto
      Update: IncomeUpdateDto
      Client: Client
      PlanSelections: PlanSelectionDto list }
