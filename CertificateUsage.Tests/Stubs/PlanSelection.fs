module CertificateUsage.Tests.Stubs.PlanSelection

open CertificateUsage.Dao
open CertificateUsage.Domain

let dao: PlanSelectionDao =
    { PlanSelectionDao.ProductLine = "ProductLine"
      ProductLineGroup = "ProductLineGroup"
      Coverage = Some "Coverage"
      Option = "Option"
      RatePer = 1.0m
      Volume = { Amount = 1.23m; Unit = "CAD" }
      CarrierRate = 1.23m
      TaxRate = 2.34m
      TaxProvince = "TaxProvince" }

let domain: PlanSelection =
    { PlanSelection.ProductLine = "ProductLine"
      ProductLineGroup = "ProductLineGroup"
      Coverage = Some "Coverage"
      Option = "Option"
      RatePer = 1.0m
      Volume = { Amount = 1.23m; Unit = "CAD" }
      CarrierRate = 1.23m
      TaxRate = 2.34m
      TaxProvince = "TaxProvince" }
