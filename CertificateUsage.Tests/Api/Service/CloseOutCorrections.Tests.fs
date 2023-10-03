module CertificateUsage.Tests.Api.Service.CloseOutCorrections_Tests

open System
open System.Threading.Tasks

open CertificateUsage.Tests.Domain
open Xunit
open Expecto

open CertificateUsage.Period
open CertificateUsage.Dao
open CertificateUsage.Api.Dependencies
open CertificateUsage.Api.Service.CloseOutCorrections
open CertificateUsage.Tests.Spy
open CertificateUsage.Tests.Stubs

module Terminations =
    module GetOriginalTerminationUsages =
        [<Fact>]
        let ``gets the original termination usage (last usage of a cert that is being terminated retroactively)`` () =
            let id1 = Guid.NewGuid()

            let stub1May =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1May =
                { stub1May with
                    BillingStart = DateTime(2023, 09, 30)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 08, 31)
                    Certificate =
                        { stub1May.Certificate with
                            CertificateNumber = "1" }
                    Usage =
                        Some(
                            { stub1May.Usage.Value with
                                CertificateNumber = "1"
                                DateIncurred = DateTime(2023, 05, 31)
                                BillingEndDate = DateTime(2023, 05, 31) }
                        ) }

            let stub1June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1June =
                { stub1June with
                    BillingStart = DateTime(2023, 09, 30)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 08, 31)
                    Certificate =
                        { stub1June.Certificate with
                            CertificateNumber = "1" }
                    Usage =
                        Some(
                            { stub1June.Usage.Value with
                                CertificateNumber = "1"
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let stub1July =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1July =
                { stub1July with
                    BillingStart = DateTime(2023, 09, 30)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 08, 31)
                    Certificate =
                        { stub1July.Certificate with
                            CertificateNumber = "1" }
                    Usage =
                        Some(
                            { stub1July.Usage.Value with
                                CertificateNumber = "1"
                                DateIncurred = DateTime(2023, 07, 30)
                                BillingEndDate = DateTime(2023, 07, 30) }
                        ) }

            let id2 = Guid.NewGuid()

            let stub2May =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id2 Guid.Empty

            let dao2May =
                { stub2May with
                    BillingStart = DateTime(2023, 09, 30)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 08, 31)
                    Certificate =
                        { stub2May.Certificate with
                            CertificateNumber = "2" }
                    Usage =
                        Some(
                            { stub2May.Usage.Value with
                                CertificateNumber = "2"
                                DateIncurred = DateTime(2023, 05, 31)
                                BillingEndDate = DateTime(2023, 05, 31) }
                        ) }

            let stub2June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id2 Guid.Empty

            let dao2June =
                { stub2June with
                    BillingStart = DateTime(2023, 09, 30)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 08, 31)
                    Certificate =
                        { stub2June.Certificate with
                            CertificateNumber = "2" }
                    Usage =
                        Some(
                            { stub2June.Usage.Value with
                                CertificateNumber = "2"
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let actual =
                [ dao1May; dao1June; dao1July; dao2May; dao2June ]
                |> Terminations.CorrectionGaps.getOriginalTerminationUsages

            let expected =
                [ (dao1July, DateTime(2023, 8, 31), DateTime(2023, 8, 31))
                  (dao2June, DateTime(2023, 7, 31), DateTime(2023, 8, 31)) ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``ignores daos with no usage`` () =
            let id1 = Guid.NewGuid()

            let stub1May =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1May =
                { stub1May with
                    BillingStart = DateTime(2023, 09, 30)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 08, 31)
                    Certificate =
                        { stub1May.Certificate with
                            CertificateNumber = "1" }
                    Usage = None }

            let stub1June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1June =
                { stub1June with
                    BillingStart = DateTime(2023, 09, 30)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 08, 31)
                    Certificate =
                        { stub1June.Certificate with
                            CertificateNumber = "1" }
                    Usage =
                        Some(
                            { stub1June.Usage.Value with
                                CertificateNumber = "1"
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let id2 = Guid.NewGuid()

            let stub2May =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id2 Guid.Empty

            let dao2May =
                { stub2May with
                    BillingStart = DateTime(2023, 09, 30)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 08, 31)
                    Certificate =
                        { stub2May.Certificate with
                            CertificateNumber = "2" }
                    Usage = None }

            let actual =
                [ dao1May; dao1June; dao2May ]
                |> Terminations.CorrectionGaps.getOriginalTerminationUsages

            let expected = []
            Expect.equal actual expected "should equal"

module Enrollment =
    module GetGapCorrectionsForNewRetroEnrollment =
        [<Fact>]
        let ``gets the june data when backdated for may and used in June`` () =
            let id1 = Guid.NewGuid()

            let stub1June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1June =
                { stub1June with
                    BillingStart = DateTime(2023, 09, 1)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 05, 31)
                    Certificate =
                        { stub1June.Certificate with
                            CertificateNumber = "1" }
                    Usage =
                        Some(
                            { stub1June.Usage.Value with
                                CertificateNumber = "1"
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let stub1July =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1July =
                { stub1July with
                    BillingStart = DateTime(2023, 09, 1)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 05, 31)
                    Certificate =
                        { stub1July.Certificate with
                            CertificateNumber = "1" }
                    Usage =
                        Some(
                            { stub1July.Usage.Value with
                                CertificateNumber = "1"
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let id2 = Guid.NewGuid()

            let stub2June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id2 Guid.Empty

            let dao2June =
                { stub2June with
                    BillingStart = DateTime(2023, 09, 1)
                    BillingEnd = DateTime(2023, 09, 30)
                    Backdate = DateTime(2023, 05, 31)
                    Certificate =
                        { stub2June.Certificate with
                            CertificateNumber = "2" }
                    Usage =
                        Some(
                            { stub2June.Usage.Value with
                                CertificateNumber = "2"
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let actual =
                [ dao1June; dao1July; dao2June ]
                |> Enrollments.CorrectionGaps.getGapDates (DateTime(2023, 09, 30))
                |> List.choose id

            let expected =
                [ dao1June, DateTime(2023, 05, 31), DateTime(2023, 05, 31)
                  dao2June, DateTime(2023, 05, 31), DateTime(2023, 05, 31) ]

            Expect.equal actual expected "should equal"

module Acceptance =
    let dependencies : Root.Root =
        { GetUsageForCarrierByDate = fun _ -> Task.lift [ None ]
          GetRateChangesForCarrierOnOrAfter = fun _ -> Task.lift [ None ]
          CloseOutMonth = fun _ _ -> Task.lift (Ok [])
          PreviewUsage = fun _ -> Task.lift []
          GetUsageForCarrierByDateFromClosedBook = fun _ _ -> Task.lift (Ok [])
          GetRetroUpdates = fun _ _ _ -> Task.lift []
          GetRetroEnrollments = fun _ _ -> Task.lift []
          GetRetroTerminations = fun _ _ -> Task.lift []
          InsertCorrections = fun _ -> Task.lift []
          GetClosedBookDates = fun _ -> Task.lift [] }

    module closeOutRetroactiveTerminations =
        [<Fact>]
        let ``terminating requires reversals for each month used after termination date`` () =
            let billingPeriod =
                { Period.Start = DateTime(2023, 09, 1)
                  End = DateTime(2023, 09, 30) }

            let id1 = Guid.NewGuid()

            let stub1May =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1May =
                { stub1May with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 05, 31)
                    Usage =
                        Some(
                            { stub1May.Usage.Value with
                                DateIncurred = DateTime(2023, 05, 31)
                                BillingEndDate = DateTime(2023, 05, 31) }
                        ) }

            let stub1June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1June =
                { stub1June with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 5, 31)
                    Usage =
                        Some(
                            { stub1June.Usage.Value with
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let insertSpy =
                Spy<Task<int list>, CertificateUsageDao option list>(dependencies.InsertCorrections)

            let root =
                { dependencies with
                    InsertCorrections = insertSpy.Function
                    GetRetroTerminations = fun _ _ -> Task.lift [ dao1May; dao1June ] }

            let _ =
                Terminations.closeOutRetroactiveTerminations root stub1May.Certificate.CarrierName billingPeriod

            let actual = insertSpy.Args[0] |> List.choose id

            let expected =
                [ { CertificateUsageDao.Id = actual[0].Id
                    CausationId = dao1May.RetroCertificateUpdateId
                    CorrelatedUsageId = Some dao1June.Usage.Value.Id
                    UsageType = CertificateUsageType.Reversal
                    CertificateNumber = dao1June.Usage.Value.CertificateNumber
                    CarrierName = dao1June.Usage.Value.CarrierName
                    ClientName = dao1June.Usage.Value.ClientName
                    PolicyNumber = dao1June.Usage.Value.PolicyNumber
                    ScbPolicyNumber = dao1June.Usage.Value.ScbPolicyNumber
                    BenefitStartDate = dao1June.Usage.Value.BenefitStartDate
                    BenefitEndDate = dao1June.Usage.Value.BenefitEndDate
                    Division = dao1June.Usage.Value.Division
                    ProductLine = dao1June.Usage.Value.ProductLine
                    ProductLineGroup = dao1June.Usage.Value.ProductLineGroup
                    Coverage = dao1June.Usage.Value.Coverage
                    Option = dao1June.Usage.Value.Option
                    RatePer = dao1June.Usage.Value.RatePer
                    VolumeAmount = dao1June.Usage.Value.VolumeAmount
                    VolumeUnit = dao1June.Usage.Value.VolumeUnit
                    CarrierRate = dao1June.Usage.Value.CarrierRate
                    TaxRate = dao1June.Usage.Value.TaxRate
                    TaxProvince = dao1June.Usage.Value.TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = dao1June.Usage.Value.DateIncurred } ]

            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``moving a termination forward in time requires (gap) corrections`` () =
            let billingPeriod =
                { Period.Start = DateTime(2023, 09, 1)
                  End = DateTime(2023, 09, 30) }

            let id1 = Guid.NewGuid()

            let stub1May =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1May =
                { stub1May with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 08, 31)
                    Usage =
                        Some(
                            { stub1May.Usage.Value with
                                DateIncurred = DateTime(2023, 05, 31)
                                BillingEndDate = DateTime(2023, 05, 31) }
                        ) }

            let stub1June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1June =
                { stub1June with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 08, 31)
                    Usage =
                        Some(
                            { stub1May.Usage.Value with
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let insertSpy =
                Spy<Task<int list>, CertificateUsageDao option list>(dependencies.InsertCorrections)

            let root =
                { dependencies with
                    InsertCorrections = insertSpy.Function
                    GetRetroTerminations = fun _ _ -> Task.lift [ dao1May; dao1June ] }

            let _ =
                Terminations.closeOutRetroactiveTerminations root stub1May.Certificate.CarrierName billingPeriod

            let actual = insertSpy.Args[0] |> List.choose id

            let expected =
                [ { CertificateUsageDao.Id = actual[0].Id
                    CausationId = dao1June.RetroCertificateUpdateId
                    CorrelatedUsageId = None
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1June.Certificate.CertificateNumber
                    CarrierName = dao1June.Certificate.CarrierName
                    ClientName = dao1June.Certificate.ClientName
                    PolicyNumber = dao1June.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1June.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1June.Certificate.StartDate
                    BenefitEndDate = dao1June.Certificate.EndDate
                    Division = dao1June.Certificate.Division
                    ProductLine = dao1June.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1June.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1June.Certificate.PlanSelections[0].Coverage
                    Option = dao1June.Certificate.PlanSelections[0].Option
                    RatePer = dao1June.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1June.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1June.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1June.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1June.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1June.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 07, 31) }
                  { CertificateUsageDao.Id = actual[1].Id
                    CausationId = dao1May.RetroCertificateUpdateId
                    CorrelatedUsageId = None
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1June.Certificate.CertificateNumber
                    CarrierName = dao1June.Certificate.CarrierName
                    ClientName = dao1June.Certificate.ClientName
                    PolicyNumber = dao1June.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1June.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1June.Certificate.StartDate
                    BenefitEndDate = dao1June.Certificate.EndDate
                    Division = dao1June.Certificate.Division
                    ProductLine = dao1June.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1June.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1June.Certificate.PlanSelections[0].Coverage
                    Option = dao1June.Certificate.PlanSelections[0].Option
                    RatePer = dao1June.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1June.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1June.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1June.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1June.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1June.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 08, 31) } ]

            Expect.equal actual expected "should equal"

    module closeOutRetroactiveEnrollments =
        [<Fact>]
        let ``enrolling needs, for months after enrolment date to billing date, corrections for periods not used; otherwise reversal/corrections``
            ()
            =
            let billingPeriod =
                { Period.Start = DateTime(2023, 07, 1)
                  End = DateTime(2023, 07, 31) }

            let id1 = Guid.NewGuid()

            let stub1June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1June =
                { stub1June with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 5, 31)
                    Usage =
                        Some(
                            { stub1June.Usage.Value with
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let insertSpy =
                Spy<Task<int list>, CertificateUsageDao option list>(dependencies.InsertCorrections)

            let root =
                { dependencies with
                    InsertCorrections = insertSpy.Function
                    GetRetroEnrollments = fun _ _ -> Task.lift [ dao1June ] }

            let _ =
                Enrollments.closeOutRetroactiveEnrollments root stub1June.Certificate.CarrierName billingPeriod

            let actual =
                insertSpy.Args[0] |> List.choose id |> List.sortBy (fun x -> x.DateIncurred)

            let expected =
                [ { CertificateUsageDao.Id = actual[0].Id
                    CausationId = stub1June.RetroCertificateUpdateId
                    CorrelatedUsageId = None
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1June.Certificate.CertificateNumber
                    CarrierName = dao1June.Certificate.CarrierName
                    ClientName = dao1June.Certificate.ClientName
                    PolicyNumber = dao1June.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1June.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1June.Certificate.StartDate
                    BenefitEndDate = dao1June.Certificate.EndDate
                    Division = dao1June.Certificate.Division
                    ProductLine = dao1June.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1June.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1June.Certificate.PlanSelections[0].Coverage
                    Option = dao1June.Certificate.PlanSelections[0].Option
                    RatePer = dao1June.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1June.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1June.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1June.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1June.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1June.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 5, 31) }
                  { CertificateUsageDao.Id = actual[1].Id
                    CausationId = dao1June.RetroCertificateUpdateId
                    CorrelatedUsageId = Some dao1June.Usage.Value.Id
                    UsageType = CertificateUsageType.Reversal
                    CertificateNumber = dao1June.Usage.Value.CertificateNumber
                    CarrierName = dao1June.Usage.Value.CarrierName
                    ClientName = dao1June.Usage.Value.ClientName
                    PolicyNumber = dao1June.Usage.Value.PolicyNumber
                    ScbPolicyNumber = dao1June.Usage.Value.ScbPolicyNumber
                    BenefitStartDate = dao1June.Usage.Value.BenefitStartDate
                    BenefitEndDate = dao1June.Usage.Value.BenefitEndDate
                    Division = dao1June.Usage.Value.Division
                    ProductLine = dao1June.Usage.Value.ProductLine
                    ProductLineGroup = dao1June.Usage.Value.ProductLineGroup
                    Coverage = dao1June.Usage.Value.Coverage
                    Option = dao1June.Usage.Value.Option
                    RatePer = dao1June.Usage.Value.RatePer
                    VolumeAmount = dao1June.Usage.Value.VolumeAmount
                    VolumeUnit = dao1June.Usage.Value.VolumeUnit
                    CarrierRate = dao1June.Usage.Value.CarrierRate
                    TaxRate = dao1June.Usage.Value.TaxRate
                    TaxProvince = dao1June.Usage.Value.TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 6, 30) }
                  { CertificateUsageDao.Id = actual[2].Id
                    CausationId = dao1June.RetroCertificateUpdateId
                    CorrelatedUsageId = Some dao1June.Usage.Value.Id
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1June.Certificate.CertificateNumber
                    CarrierName = dao1June.Certificate.CarrierName
                    ClientName = dao1June.Certificate.ClientName
                    PolicyNumber = dao1June.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1June.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1June.Certificate.StartDate
                    BenefitEndDate = dao1June.Certificate.EndDate
                    Division = dao1June.Certificate.Division
                    ProductLine = dao1June.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1June.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1June.Certificate.PlanSelections[0].Coverage
                    Option = dao1June.Certificate.PlanSelections[0].Option
                    RatePer = dao1June.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1June.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1June.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1June.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1June.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1June.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 6, 30) } ]


            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``enrolling needs reversals for periods before enrolment and reversal/corrections on or after`` () =
            let billingPeriod =
                { Period.Start = DateTime(2023, 07, 1)
                  End = DateTime(2023, 07, 31) }

            let id1 = Guid.NewGuid()

            let stub1May =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1May =
                { stub1May with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 6, 30)
                    Usage =
                        Some(
                            { stub1May.Usage.Value with
                                DateIncurred = DateTime(2023, 05, 31)
                                BillingEndDate = DateTime(2023, 05, 31) }
                        ) }

            let stub1June =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1June =
                { stub1June with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 6, 30)
                    Usage =
                        Some(
                            { stub1June.Usage.Value with
                                DateIncurred = DateTime(2023, 06, 30)
                                BillingEndDate = DateTime(2023, 06, 30) }
                        ) }

            let stub1July =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1July =
                { stub1July with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 6, 30)
                    Usage =
                        Some(
                            { stub1July.Usage.Value with
                                DateIncurred = DateTime(2023, 07, 31)
                                BillingEndDate = DateTime(2023, 07, 31) }
                        ) }

            let insertSpy =
                Spy<Task<int list>, CertificateUsageDao option list>(dependencies.InsertCorrections)

            let root =
                { dependencies with
                    InsertCorrections = insertSpy.Function
                    GetRetroEnrollments = fun _ _ -> Task.lift [ dao1May; dao1June; dao1July ] }

            let _ =
                Enrollments.closeOutRetroactiveEnrollments root stub1May.Certificate.CarrierName billingPeriod

            let actual =
                insertSpy.Args[0] |> List.choose id |> List.sortBy (fun x -> x.DateIncurred)

            let expected =
                [ { CertificateUsageDao.Id = actual[0].Id
                    CausationId = dao1May.RetroCertificateUpdateId
                    CorrelatedUsageId = Some dao1May.Usage.Value.Id
                    UsageType = CertificateUsageType.Reversal
                    CertificateNumber = dao1May.Usage.Value.CertificateNumber
                    CarrierName = dao1May.Usage.Value.CarrierName
                    ClientName = dao1May.Usage.Value.ClientName
                    PolicyNumber = dao1May.Usage.Value.PolicyNumber
                    ScbPolicyNumber = dao1May.Usage.Value.ScbPolicyNumber
                    BenefitStartDate = dao1May.Usage.Value.BenefitStartDate
                    BenefitEndDate = dao1May.Usage.Value.BenefitEndDate
                    Division = dao1May.Usage.Value.Division
                    ProductLine = dao1May.Usage.Value.ProductLine
                    ProductLineGroup = dao1May.Usage.Value.ProductLineGroup
                    Coverage = dao1May.Usage.Value.Coverage
                    Option = dao1May.Usage.Value.Option
                    RatePer = dao1May.Usage.Value.RatePer
                    VolumeAmount = dao1May.Usage.Value.VolumeAmount
                    VolumeUnit = dao1May.Usage.Value.VolumeUnit
                    CarrierRate = dao1May.Usage.Value.CarrierRate
                    TaxRate = dao1May.Usage.Value.TaxRate
                    TaxProvince = dao1May.Usage.Value.TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 5, 31) }
                  { CertificateUsageDao.Id = actual[1].Id
                    CausationId = dao1June.RetroCertificateUpdateId
                    CorrelatedUsageId = Some dao1June.Usage.Value.Id
                    UsageType = CertificateUsageType.Reversal
                    CertificateNumber = dao1June.Usage.Value.CertificateNumber
                    CarrierName = dao1June.Usage.Value.CarrierName
                    ClientName = dao1June.Usage.Value.ClientName
                    PolicyNumber = dao1June.Usage.Value.PolicyNumber
                    ScbPolicyNumber = dao1June.Usage.Value.ScbPolicyNumber
                    BenefitStartDate = dao1June.Usage.Value.BenefitStartDate
                    BenefitEndDate = dao1June.Usage.Value.BenefitEndDate
                    Division = dao1June.Usage.Value.Division
                    ProductLine = dao1June.Usage.Value.ProductLine
                    ProductLineGroup = dao1June.Usage.Value.ProductLineGroup
                    Coverage = dao1June.Usage.Value.Coverage
                    Option = dao1June.Usage.Value.Option
                    RatePer = dao1June.Usage.Value.RatePer
                    VolumeAmount = dao1June.Usage.Value.VolumeAmount
                    VolumeUnit = dao1June.Usage.Value.VolumeUnit
                    CarrierRate = dao1June.Usage.Value.CarrierRate
                    TaxRate = dao1June.Usage.Value.TaxRate
                    TaxProvince = dao1June.Usage.Value.TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 6, 30) }
                  { CertificateUsageDao.Id = actual[2].Id
                    CausationId = dao1June.RetroCertificateUpdateId
                    CorrelatedUsageId = Some dao1June.Usage.Value.Id
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1June.Certificate.CertificateNumber
                    CarrierName = dao1June.Certificate.CarrierName
                    ClientName = dao1June.Certificate.ClientName
                    PolicyNumber = dao1June.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1June.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1June.Certificate.StartDate
                    BenefitEndDate = dao1June.Certificate.EndDate
                    Division = dao1June.Certificate.Division
                    ProductLine = dao1June.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1June.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1June.Certificate.PlanSelections[0].Coverage
                    Option = dao1June.Certificate.PlanSelections[0].Option
                    RatePer = dao1June.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1June.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1June.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1June.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1June.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1June.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 6, 30) }
                  { CertificateUsageDao.Id = actual[3].Id
                    CausationId = stub1July.RetroCertificateUpdateId
                    CorrelatedUsageId = Some stub1July.Usage.Value.Id
                    UsageType = CertificateUsageType.Reversal
                    CertificateNumber = stub1July.Usage.Value.CertificateNumber
                    CarrierName = stub1July.Usage.Value.CarrierName
                    ClientName = stub1July.Usage.Value.ClientName
                    PolicyNumber = stub1July.Usage.Value.PolicyNumber
                    ScbPolicyNumber = stub1July.Usage.Value.ScbPolicyNumber
                    BenefitStartDate = stub1July.Usage.Value.BenefitStartDate
                    BenefitEndDate = stub1July.Usage.Value.BenefitEndDate
                    Division = stub1July.Usage.Value.Division
                    ProductLine = stub1July.Usage.Value.ProductLine
                    ProductLineGroup = stub1July.Usage.Value.ProductLineGroup
                    Coverage = stub1July.Usage.Value.Coverage
                    Option = stub1July.Usage.Value.Option
                    RatePer = stub1July.Usage.Value.RatePer
                    VolumeAmount = stub1July.Usage.Value.VolumeAmount
                    VolumeUnit = stub1July.Usage.Value.VolumeUnit
                    CarrierRate = stub1July.Usage.Value.CarrierRate
                    TaxRate = stub1July.Usage.Value.TaxRate
                    TaxProvince = stub1July.Usage.Value.TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 7, 31) }
                  { CertificateUsageDao.Id = actual[4].Id
                    CausationId = stub1July.RetroCertificateUpdateId
                    CorrelatedUsageId = Some stub1July.Usage.Value.Id
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = stub1July.Certificate.CertificateNumber
                    CarrierName = stub1July.Certificate.CarrierName
                    ClientName = stub1July.Certificate.ClientName
                    PolicyNumber = stub1July.Certificate.PolicyNumber
                    ScbPolicyNumber = stub1July.Certificate.ScbPolicyNumber
                    BenefitStartDate = stub1July.Certificate.StartDate
                    BenefitEndDate = stub1July.Certificate.EndDate
                    Division = stub1July.Certificate.Division
                    ProductLine = stub1July.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = stub1July.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = stub1July.Certificate.PlanSelections[0].Coverage
                    Option = stub1July.Certificate.PlanSelections[0].Option
                    RatePer = stub1July.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = stub1July.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = stub1July.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = stub1July.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = stub1July.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = stub1July.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 7, 31) } ]


            Expect.equal actual expected "should equal"

        [<Fact>]
        let ``enrolling needs corrections (filling the gaps) after the enrolment date to the billing period when there are no usages``
            ()
            =
            let billingPeriod =
                { Period.Start = DateTime(2023, 09, 1)
                  End = DateTime(2023, 09, 30) }

            let id1 = Guid.NewGuid()

            let stub1July =
                RetroactiveCertificateUsageTransitionDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1July =
                { stub1July with
                    BillingStart = billingPeriod.Start
                    BillingEnd = billingPeriod.End
                    Backdate = DateTime(2023, 07, 31)
                    Usage = None
                    ProductLine = stub1July.Certificate.PlanSelections[0].ProductLine
                    Coverage = stub1July.Certificate.PlanSelections[0].Coverage
                    Option = stub1July.Certificate.PlanSelections[0].Option }

            let insertSpy =
                Spy<Task<int list>, CertificateUsageDao option list>(dependencies.InsertCorrections)

            let root =
                { dependencies with
                    InsertCorrections = insertSpy.Function
                    GetRetroEnrollments = fun _ _ -> Task.lift [ dao1July ] }

            let _ =
                Enrollments.closeOutRetroactiveEnrollments root stub1July.Certificate.CarrierName billingPeriod

            let actual = insertSpy.Args[0] |> List.choose id

            let expected =
                [ { CertificateUsageDao.Id = actual[0].Id
                    CausationId = dao1July.RetroCertificateUpdateId
                    CorrelatedUsageId = None
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1July.Certificate.CertificateNumber
                    CarrierName = dao1July.Certificate.CarrierName
                    ClientName = dao1July.Certificate.ClientName
                    PolicyNumber = dao1July.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1July.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1July.Certificate.StartDate
                    BenefitEndDate = dao1July.Certificate.EndDate
                    Division = dao1July.Certificate.Division
                    ProductLine = dao1July.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1July.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1July.Certificate.PlanSelections[0].Coverage
                    Option = dao1July.Certificate.PlanSelections[0].Option
                    RatePer = dao1July.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1July.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1July.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1July.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1July.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1July.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 07, 31) }
                  { CertificateUsageDao.Id = actual[1].Id
                    CausationId = dao1July.RetroCertificateUpdateId
                    CorrelatedUsageId = None
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1July.Certificate.CertificateNumber
                    CarrierName = dao1July.Certificate.CarrierName
                    ClientName = dao1July.Certificate.ClientName
                    PolicyNumber = dao1July.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1July.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1July.Certificate.StartDate
                    BenefitEndDate = dao1July.Certificate.EndDate
                    Division = dao1July.Certificate.Division
                    ProductLine = dao1July.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1July.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1July.Certificate.PlanSelections[0].Coverage
                    Option = dao1July.Certificate.PlanSelections[0].Option
                    RatePer = dao1July.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1July.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1July.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1July.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1July.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1July.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 08, 31) } ]

            Expect.equal actual expected "should equal"

    module CloseOutCorrections =
        [<Fact>]
        let ``updating needs correction and reversal from backdate to billing date`` () =
            let billingPeriod =
                { Period.Start = DateTime(2023, 09, 1)
                  End = DateTime(2023, 09, 30) }

            let id1 = Guid.NewGuid()

            let stub1July =
                RetroactiveCertificateUsageUpdateDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1July =
                { stub1July with
                    Usage =
                        { stub1July.Usage with
                            DateIncurred = DateTime(2023, 7, 31)
                            BillingEndDate = billingPeriod.End } }

            let stub1Aug =
                RetroactiveCertificateUsageUpdateDao.Dao.stub Guid.Empty id1 Guid.Empty

            let dao1Aug =
                { stub1Aug with
                    Usage =
                        { stub1July.Usage with
                            DateIncurred = DateTime(2023, 8, 31)
                            BillingEndDate = billingPeriod.End } }

            let insertSpy =
                Spy<Task<int list>, CertificateUsageDao option list>(dependencies.InsertCorrections)

            let root =
                { dependencies with
                    InsertCorrections = insertSpy.Function
                    GetRetroUpdates = fun _ _ _ -> Task.lift [ dao1July; dao1Aug ] }

            let _ =
                closeOutRetroactiveUpdates root stub1July.Certificate.CarrierName billingPeriod

            let actual = insertSpy.Args[0] |> List.choose id

            let expected =
                [ { CertificateUsageDao.Id = actual[0].Id
                    CausationId = dao1July.Usage.CausationId
                    CorrelatedUsageId = Some dao1July.Usage.Id
                    UsageType = CertificateUsageType.Reversal
                    CertificateNumber = dao1July.Usage.CertificateNumber
                    CarrierName = dao1July.Usage.CarrierName
                    ClientName = dao1July.Usage.ClientName
                    PolicyNumber = dao1July.Usage.PolicyNumber
                    ScbPolicyNumber = dao1July.Usage.ScbPolicyNumber
                    BenefitStartDate = dao1July.Usage.BenefitStartDate
                    BenefitEndDate = dao1July.Usage.BenefitEndDate
                    Division = dao1July.Usage.Division
                    ProductLine = dao1July.Usage.ProductLine
                    ProductLineGroup = dao1July.Usage.ProductLineGroup
                    Coverage = dao1July.Usage.Coverage
                    Option = dao1July.Usage.Option
                    RatePer = dao1July.Usage.RatePer
                    VolumeAmount = dao1July.Usage.VolumeAmount
                    VolumeUnit = dao1July.Usage.VolumeUnit
                    CarrierRate = dao1July.Usage.CarrierRate
                    TaxRate = dao1July.Usage.TaxRate
                    TaxProvince = dao1July.Usage.TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 07, 31) }
                  { CertificateUsageDao.Id = actual[1].Id
                    CausationId = dao1July.Usage.CausationId
                    CorrelatedUsageId = Some dao1July.Usage.Id
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1July.Certificate.CertificateNumber
                    CarrierName = dao1July.Certificate.CarrierName
                    ClientName = dao1July.Certificate.ClientName
                    PolicyNumber = dao1July.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1July.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1July.Certificate.StartDate
                    BenefitEndDate = dao1July.Certificate.EndDate
                    Division = dao1July.Certificate.Division
                    ProductLine = dao1July.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1July.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1July.Certificate.PlanSelections[0].Coverage
                    Option = dao1July.Certificate.PlanSelections[0].Option
                    RatePer = dao1July.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1July.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1July.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1July.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1July.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1July.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 07, 31) }
                  { CertificateUsageDao.Id = actual[2].Id
                    CausationId = dao1Aug.Usage.CausationId
                    CorrelatedUsageId = Some dao1Aug.Usage.Id
                    UsageType = CertificateUsageType.Reversal
                    CertificateNumber = dao1Aug.Usage.CertificateNumber
                    CarrierName = dao1Aug.Usage.CarrierName
                    ClientName = dao1Aug.Usage.ClientName
                    PolicyNumber = dao1Aug.Usage.PolicyNumber
                    ScbPolicyNumber = dao1Aug.Usage.ScbPolicyNumber
                    BenefitStartDate = dao1Aug.Usage.BenefitStartDate
                    BenefitEndDate = dao1Aug.Usage.BenefitEndDate
                    Division = dao1Aug.Usage.Division
                    ProductLine = dao1Aug.Usage.ProductLine
                    ProductLineGroup = dao1Aug.Usage.ProductLineGroup
                    Coverage = dao1Aug.Usage.Coverage
                    Option = dao1Aug.Usage.Option
                    RatePer = dao1Aug.Usage.RatePer
                    VolumeAmount = dao1Aug.Usage.VolumeAmount
                    VolumeUnit = dao1Aug.Usage.VolumeUnit
                    CarrierRate = dao1Aug.Usage.CarrierRate
                    TaxRate = dao1Aug.Usage.TaxRate
                    TaxProvince = dao1Aug.Usage.TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 08, 31) }
                  { CertificateUsageDao.Id = actual[3].Id
                    CausationId = dao1Aug.Usage.CausationId
                    CorrelatedUsageId = Some dao1Aug.Usage.Id
                    UsageType = CertificateUsageType.Correction
                    CertificateNumber = dao1Aug.Certificate.CertificateNumber
                    CarrierName = dao1Aug.Certificate.CarrierName
                    ClientName = dao1Aug.Certificate.ClientName
                    PolicyNumber = dao1Aug.Certificate.PolicyNumber
                    ScbPolicyNumber = dao1Aug.Certificate.ScbPolicyNumber
                    BenefitStartDate = dao1Aug.Certificate.StartDate
                    BenefitEndDate = dao1Aug.Certificate.EndDate
                    Division = dao1Aug.Certificate.Division
                    ProductLine = dao1Aug.Certificate.PlanSelections[0].ProductLine
                    ProductLineGroup = dao1Aug.Certificate.PlanSelections[0].ProductLineGroup
                    Coverage = dao1Aug.Certificate.PlanSelections[0].Coverage
                    Option = dao1Aug.Certificate.PlanSelections[0].Option
                    RatePer = dao1Aug.Certificate.PlanSelections[0].RatePer
                    VolumeAmount = dao1Aug.Certificate.PlanSelections[0].Volume.Amount
                    VolumeUnit = dao1Aug.Certificate.PlanSelections[0].Volume.Unit
                    CarrierRate = dao1Aug.Certificate.PlanSelections[0].CarrierRate
                    TaxRate = dao1Aug.Certificate.PlanSelections[0].TaxRate
                    TaxProvince = dao1Aug.Certificate.PlanSelections[0].TaxProvince
                    BillingEndDate = billingPeriod.End
                    DateIncurred = DateTime(2023, 08, 31) } ]

            Expect.equal actual expected "should equal"
