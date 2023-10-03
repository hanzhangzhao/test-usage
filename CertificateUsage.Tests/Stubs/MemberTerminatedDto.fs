module CertificateUsage.Tests.Stubs.MemberTerminatedDto

open System
open CertificateUsage.Dto.Events.MemberTerminated

let dto =
    { MemberTerminatedDto.BenefitsStartDate = DateTime(2021, 01, 01)
      BenefitsEndedDate = DateTime(2023, 07, 01) |> Some
      MemberId = 1234
      DateOfBirth = DateTime(1989, 01, 01) |> Some
      TaxProvince = "BC"
      CertificateNumber = "1000012"
      PolicyNumber = "9999"
      ExternalPolicyNumber = Some "9998"
      CarrierClientCode = "9105" |> Some
      Carrier = "great_west_life"
      BenefitClassCode = "employees"
      CarrierMapping = None
      Client = { Name = Some "Test" }
      PublicId = None
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
