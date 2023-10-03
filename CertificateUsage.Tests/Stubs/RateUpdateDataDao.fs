module CertificateUsage.Tests.Stubs.RateUpdateDataDao

open System

open CertificateUsage.Dao

let effective = DateTime(2023, 07, 19)

let dao =
    { RateUpdateDataDao.Carrier = "Carrier"
      PolicyNumber = "PolicyNumber"
      Option = "Option"
      Coverage = Some "Coverage"
      ProductLine = "ProductLine"
      Effective = effective
      RateUpdateData =
        { Carrier = "Carrier"
          PolicyNumber = "PolicyNumber"
          Option = "Option"
          Coverage = Some "Coverage"
          ProductLine = "ProductLine"
          Effective = effective
          CarrierRate = 1.23m
          ChangedBy = { Id = 1; Name = "ChangedBy.Name" } }
      EventMetadata =
        { EventId = Guid.Empty
          EventNo = 0UL
          EventType = "EventType"
          EventDate = DateTime()
          EventVersion = "EventVersion"
          EventStream = "EventStream" } }
