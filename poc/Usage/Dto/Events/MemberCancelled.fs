module Usage.Dto.Events.MemberCancelled

open System
open Usage.Dto.Events
open MemberAddress
open Employment
open Usage.Dto.Events.BenefitPeriod
open Usage.Dto.Events.Spouse
open Usage.Dto.Events.Dependent

type MemberCancelledDto =
    { BenefitsStartDate: DateTime
      BenefitsEndedDate: DateTime option
      IsEnrolled: bool
      BenefitPeriodId: int
      MemberId: int 
      PublicId: string option
      PreferredContactMethod: string
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
      MemberAddress: MemberAddress
      TaxProvince: string
      Employment: Employment
      Spouse: Spouse option
      Dependents: Dependent list
      CertificateNumber: string
      PolicyNumber: string
      ExternalPolicyNumber: string option
      CarrierClientCode: string option
      Carrier: string
      BenefitClassCode: string
      CarrierMapping: CarrierMapping option }
    