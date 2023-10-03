module CertificateUsage.Tests.Api.Dao_Tests

open Xunit
open Expecto

open CertificateUsage.Api.Dao
open CertificateUsage.Api.Dto
open CertificateUsage.Tests

module UsagePreviewDaoToUsagePreviewDto =
    [<Fact>]
    let ``UsagePreviewDao successfully transforms to a UsagePreviewDto`` () =
        let dao = Stubs.UsagePreviewDao.dao
        let actual = usagePreviewDaoToUsagePreviewDto dao

        let expected =
            { UsagePreviewDto.CertificateNumber = "CertificateNumber"
              CarrierName = "CarrierName"
              ClientName = "ClientName"
              PolicyNumber = "PolicyNumber"
              ScbPolicyNumber = "ScbPolicyNumber"
              BenefitStartDate = Stubs.UsagePreviewDao.benefitStartDate
              BenefitEndDate = Some Stubs.UsagePreviewDao.benefitEndDate
              Division = "Division"
              ProductLine = "ProductLine"
              ProductLineGroup = Some "ProductLineGroup"
              Coverage = Some "Coverage"
              Option = "Option"
              RatePer = Some Stubs.UsagePreviewDao.ratePer
              VolumeAmount = Stubs.UsagePreviewDao.volumeAmount
              VolumeUnit = "VolumeUnit"
              CarrierRate = Stubs.UsagePreviewDao.carrierRate
              TaxRate = Some Stubs.UsagePreviewDao.taxRate
              TaxProvince = "TaxProvince" }

        Expect.equal actual expected "should equal"

module usageLineDaoToBillingReadModelDto =
    [<Fact>]
    let ``UsagePreviewDao successfully transforms to a UsagePreviewDto`` () =
        let dao = Stubs.UsageLineDao.dao
        let actual = usageLineDaoToBillingReadModelDto dao

        let expected =
            { BillingReadModelDto.CarrierCode = "CarrierName"
              CertificateNumber = "CertificateNumber"
              ClientName = "ClientName"
              PolicyNumber = "PolicyNumber"
              ProductLine = "ProductLine"
              Coverage = Some "Coverage"
              ProductOption = "Option"
              Volume = Stubs.UsageLineDao.volumeAmount
              Lives = Stubs.UsageLineDao.lives
              TaxRate = Stubs.UsageLineDao.taxRate
              TaxProvince = "TaxProvince"
              Year = Stubs.UsageLineDao.year
              Month = Stubs.UsageLineDao.month
              CarrierRate = Stubs.UsageLineDao.carrierRate
              ClientRate = Stubs.UsageLineDao.clientRate
              Division = "Division" }

        Expect.equal actual expected "should equal"
