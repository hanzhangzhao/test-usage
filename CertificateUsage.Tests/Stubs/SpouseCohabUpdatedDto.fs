module CertificateUsage.Tests.Stubs.SpouseCohabUpdatedDto

open System
open CertificateUsage.Dto.Events.SpouseCohabUpdated

let dto =
    { SpouseCohabUpdatedDto.BenefitPeriods =
        [ { Id = 123
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
            EnrollmentDate = DateTime(2023, 06, 12) |> Some } ]
      Spouse = { CohabitationDate = DateTime(2023, 06, 14) |> Some }
      Client = { Name = Some "ClientName" }
      IsEnrolled = true
      PlanSelections =
        [ { ProductLine = "ProductLine"
            LineGroup = "LineGroup"
            Coverage = Some "Coverage"
            Selection = "Option"
            PricePer = 1.1m
            Volume = { Amount = 1.2m; Units = "CAD" }
            CarrierRate = "0.8"
            TaxProvince = "TaxProvince"
            TaxRate = "1.3" } ] }
