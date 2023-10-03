module CertificateUsage.Tests.Stubs.DependentPostSecondaryEducationUpdatedDto

open System
open CertificateUsage.Dto.Events.DependentPostSecondaryEducationUpdated

let dto =
    { DependentPostSecondaryEducationUpdatedDto.ActiveBenefitPeriod =
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
      Dependent =
        { Id = 123
          EligibilityPeriods =
            [ { EligibilityStartDate = DateTime(2023, 06, 14)
                EmployeeBenefitPeriodId = 123 } ] }
      Client = { Name = Some "ClientName" }
      Member =
        { PlanSelections =
            [ { ProductLine = "ProductLine"
                LineGroup = "LineGroup"
                Coverage = Some "Coverage"
                Selection = "Option"
                PricePer = 1.1m
                Volume = { Amount = 1.2m; Units = "CAD" }
                CarrierRate = "0.8"
                TaxProvince = "TaxProvince"
                TaxRate = "1.3" } ] } }
