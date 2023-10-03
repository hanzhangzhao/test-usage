module Usage.Models

open System

type CertificateState =
    | Covered
    | Excluded

type Volume =
    { Amount: decimal
      Unit: string }

type ProductSelection =
    { ProductLine: string
      Coverage: string option
      Option: string
      RatePer: decimal
      Volume: Volume }
    
type CertificateHistory =
    { State: CertificateState
      Effective: DateTime
      PolicyNumber: string
      ProductSelections: ProductSelection list }

type Certificate =
    { CertificateNumber: string
      State: CertificateState
      Carrier: string
      ClientName: string
      PolicyNumber: string
      Effective: DateTime
      ProductSelections: ProductSelection list
      History: CertificateHistory list }


// Most strict way to represent a cert
type CoveredCert =
    { CertificateNumber: string
      Carrier: string
      ClientName: string
      Effective: string
      ProductSelections: ProductSelection list
      History: CertificateHistory list }

type ExcludedCert =
    { CertificateNumber: string
      Effective: DateTime
      History: CertificateHistory }

// A cert is either covered or excluded
// Force the unwrapping of either case
type Cert =
    | CoveredCertificate of CoveredCert
    | ExcludedCertificate of ExcludedCert

// How could we represent this in a DB?
// Flat table structure
type CertificateEventsDao =
    { CertificateNumber: string
      // etc...
      Effective: DateTime
      State: CertificateState }

// Trying out the Dao
let bob_V2 =
    [
        { CertificateNumber = "1234"
          Effective = DateTime(2021, 07, 21)
          State = Covered }
        
        { CertificateNumber = "1234"
          Effective = DateTime(2022, 07, 15)
          State = Excluded }
        
        { CertificateNumber = "1234"
          Effective = DateTime(2021, 07, 01)
          State = Covered }
    ]
    
type BillingPeriod =
    { Year: int
      Month: int }
    member this.Intersects (date: DateTime) =
        match this.Year, this.Month with
        | y, m when y = date.Year && m = date.Month -> true
        | _ -> false
    
    member this.InsideOrBefore (date: DateTime) =
        date < DateTime(this.Year, this.Month, 01).AddMonths(1)