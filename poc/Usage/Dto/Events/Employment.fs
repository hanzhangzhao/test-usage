module Usage.Dto.Events.Employment

open System

type Employment =
    { EmploymentType: string option
      Occupation: string option
      PayrollCycle: string option
      FirstHireDate: DateTime option
      WorkingProvince: string option
      WorkingCountry: string option
      TaxExempt: bool option
      Smoker: bool option
      Disabled: bool option
      PayrollFileNumber: string option
      AdpPayrollGroupCode: string option
      WeeklyWorkSchedule: int option
      IncomeAmount: decimal option
      IncomeCurrency: string
      IncomeEffectiveDate: DateTime }

type NoIncomeEmploymentDto =
    { EmploymentType: string
      Occupation: string option
      PayrollCycle: string option
      FirstHireDate: DateTime option
      WorkingProvince: string option
      WorkingCountry: string option
      TaxExempt: bool option
      Smoker: bool option
      Disabled: bool option
      PayrollFileNumber: string option
      AdpPayrollGroupCode: string option
      WeeklyWorkSchedule: int option }
