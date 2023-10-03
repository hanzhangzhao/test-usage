module CertificateUsage.Period

open System

type Period = { Start : DateTime; End : DateTime }

let getDays (period : Period) =
    let includeLimitPointWhenCounting = 1
    (period.End - period.Start).Days + includeLimitPointWhenCounting
