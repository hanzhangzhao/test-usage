module Usage.Dto.Events.MemberEmploymentUpdate

open System
open MemberAddress
open Update
open Employment
open BenefitPeriod

type EmploymentUpdateDto = {
    Employment: NoIncomeEmploymentDto
    Changes: UpdateDto list
}

type MemberEmploymentUpdatedDto = {
    MemberId: int 
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
    Update: EmploymentUpdateDto
}