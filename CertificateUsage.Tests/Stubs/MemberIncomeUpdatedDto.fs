module CertificateUsage.Tests.Stubs.MemberIncomeUpdatedDto

open System
open CertificateUsage.Dto.Events.MemberIncomeUpdate
open CertificateUsage.Dto.Events.Certificate
open CertificateUsage.Dto.Events.ProductConfiguration

let dto =
    { MemberIncomeUpdatedDto.ActiveBenefitPeriod =
        { Id = 123
          IsEnrolled = true
          ProductConfiguration =
            { CarrierClientCode = "Division" |> Some
              Carrier = "Carrier" }
          Certificate =
            { CertificateNumber = "CertificateNumber"
              PolicyNumber =
                { Number = "Number"
                  ExternalPolicyNumber = "PolicyNumber" |> Some } }
          CarrierMapping = None
          BenefitsStartDate = DateTime(2023, 06, 12) |> Some
          BenefitsEndedDate = DateTime(2024, 06, 12) |> Some
          Coverages = None
          EnrollmentDate = DateTime(2023, 06, 12) |> Some }
      Client = { Name = Some "ClientName" }
      PlanSelections =
        [ { ProductLine = "ProductLine"
            LineGroup = "LineGroup"
            Coverage = Some "Coverage"
            Selection = "Option"
            PricePer = 1.1m
            Volume = { Amount = 1.2m; Units = "CAD" }
            CarrierRate = "0.8"
            TaxProvince = "TaxProvince"
            TaxRate = "1.3" } ]
      Update = { Income = { IncomeEffectiveDate = DateTime(2023, 6, 14) } } }
