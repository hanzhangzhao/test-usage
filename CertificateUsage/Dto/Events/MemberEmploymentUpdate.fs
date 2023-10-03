module CertificateUsage.Dto.Events.MemberEmploymentUpdate

open CertificateUsage.Dto.Events.Update
open CertificateUsage.Dto.Events.BenefitPeriod
open CertificateUsage.Dto.Events.Client
open CertificateUsage.Dto.Events.PlanSelection

type EmploymentUpdateDto = { Changes: UpdateDto list }

type MemberEmploymentUpdatedDto =
    { ActiveBenefitPeriod: BenefitPeriodDto
      Update: EmploymentUpdateDto
      Client: Client
      PlanSelections: PlanSelectionDto list }
