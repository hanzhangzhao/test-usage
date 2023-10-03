module Usage.Dto.Events.Income

open System

type IncomeDto =
    { IncomeAmount: decimal
      IncomeCurrency: string
      IncomeEffectiveDate: DateTime
      IncomeFrequency: string
      IncomeUnits: string }
