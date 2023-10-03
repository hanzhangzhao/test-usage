module CertificateUsage.Dto.Events.Certificate

open PolicyNumber

type Certificate =
      { CertificateNumber: string
        PolicyNumber: PolicyNumber }
