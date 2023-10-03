module CertificateUsage.Dto.Events.MemberBenefitClassTransferred

open System
open CertificateUsage.Dto.Events
open BenefitPeriod
open Employment

type MemberBenefitClassTransferredDto =
    { MemberId: int
      PublicId: string option
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
      TaxProvince: string
      From: BenefitPeriodDto
      To: BenefitPeriodDto }
