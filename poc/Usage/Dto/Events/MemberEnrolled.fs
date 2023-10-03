module Usage.Dto.Events.MemberEnrolled

open System
open Usage.Dto.Events.BenefitPeriod
open MemberAddress
open Employment
open CoverageCoordinationDto
open Dependent
open Spouse

type MetaData = { Version: string }

type PlanSelectionDto =
    { EffectiveDate: DateTime
      EmployeeBenefitPeriodId: int
      PlanSelectionId: int
      LineGroup: string
      Selection: string option }

type MemberEnrollmentDto =
    { BenefitsStartDate: DateTime
      BenefitPeriodId: int
      BenefitPeriods: BenefitPeriodDto list option
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
      Coverages: Coverages option
      PlanSelections: PlanSelectionDto list
      CertificateNumber: string
      PolicyNumber: string
      ExternalPolicyNumber: string option
      CarrierClientCode: string option
      Carrier: string
      BenefitClassCode: string
      CoverageCoordination: CoverageCoordinationDto option
      CarrierMapping: CarrierMapping option }
