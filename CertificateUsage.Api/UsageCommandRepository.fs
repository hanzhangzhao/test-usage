module CertificateUsage.Api.UsageCommandRepository

open Npgsql

open CertificateUsage.Period
open CertificateUsage.Dao
open CertificateUsage.IRowReader

type CertificateDeserializer = string -> CertificateDao

let closeOutMonth transaction carrierName billingPeriod =
    let copySql =
        "
        insert into certificate_usage
        (
             correlated_usage_id,
             causation_id,
             usage_type,
             billing_date,
             date_incurred,
             certificate_number,
             carrier_name,
             client_name,
             policy_number,
             scb_policy_number,
             benefit_start_date,
             benefit_end_date,
             division,
             product_line,
             product_line_group,
             coverage,
             option,
             rate_per,
             volume_amount,
             volume_unit,
             carrier_rate,
             tax_rate,
             tax_province
        )
        select
            null as correlated_usage_id,
            certificate.id as causation_id,
            @usageType as usage_type,
            @billingEnd,
            @billingEnd as dateIncurred,
            (certificate ->> 'CertificateNumber')::varchar(50) as certificate_number,
            (certificate ->> 'CarrierName')::varchar(256) as carrier_name,
            (certificate ->> 'ClientName')::varchar(256) as client_name,
            (certificate ->> 'PolicyNumber')::varchar(50) as policy_number,
            (certificate ->> 'ScbPolicyNumber')::varchar(50) as scb_policy_number,
            (certificate ->> 'StartDate')::timestamp as benefit_start_date,
            (certificate ->> 'EndDate')::timestamp as benefit_end_date,
            (certificate ->> 'Division')::varchar(256) as division,
            (ps ->> 'ProductLine')::varchar(256) as product_line,
            (ps ->> 'ProductLineGroup')::varchar(256) as product_line_group,
            (ps ->> 'Coverage')::varchar(24) as coverage,
            (ps ->> 'Option')::varchar(24) as \"option\",
            (ps ->> 'RatePer')::decimal as rate_per,
            (ps #>> '{{Volume, Amount}}')::decimal as volume_amount,
            (ps #>> '{{Volume, Unit}}')::varchar(24) as volume_unit,
            (ps ->> 'CarrierRate')::decimal as carrier_rate,
            (ps ->> 'TaxRate')::decimal as tax_rate,
            (ps ->> 'TaxProvince')::varchar(32) as tax_province
        from certificate
        cross join lateral jsonb_array_elements((certificate ->> 'PlanSelections')::jsonb) as ps
        where carrier = @carrierName
            and (
                status = 'active'

                -- include those certs that are terminated during the billing period
                or (
                    status = 'terminated'
                    and (certificate ->> 'EndDate')::timestamp <= @billingEnd
                    and (certificate ->> 'EndDate')::timestamp > @billingEnd - (@billingPeriod || ' days')::interval
                )
            )
        "

    let copyParameters =
        [ "carrierName", Sql.string carrierName
          "billingStart", Sql.timestamp billingPeriod.Start
          "billingEnd", Sql.timestamp billingPeriod.End
          "usageType", Sql.parameter (NpgsqlParameter("usage_type", CertificateUsageType.Charge))
          "billingPeriod", Sql.string (getDays billingPeriod |> string) ]

    task {
        let! result = transaction [ (copySql, [ copyParameters ]) ]
        return Ok result
    }

let mapRetroUpdate deserialize (reader : IRowReader) =
    let certificate = reader.text "certificate" |> deserialize

    { RetroactiveCertificateUsageUpdateDao.Usage =
        { CertificateUsageDao.Id = reader.uuid "id"
          CorrelatedUsageId = reader.uuidOrNone "correlated_usage_id"
          CausationId = reader.uuid "retroactive_certificate_update_id"
          UsageType = reader.enum<CertificateUsageType> "usage_type"
          CertificateNumber = reader.text "certificate_number"
          CarrierName = reader.text "carrier_name"
          ClientName = reader.text "client_name"
          PolicyNumber = reader.text "policy_number"
          ScbPolicyNumber = reader.text "scb_policy_number"
          BenefitStartDate = reader.dateTime "benefit_start_date"
          BenefitEndDate = reader.dateTimeOrNone "benefit_end_date"
          Division = reader.text "division"
          ProductLine = reader.text "product_line"
          ProductLineGroup = reader.text "product_line_group"
          Coverage = reader.textOrNone "coverage"
          Option = reader.text "option"
          RatePer = reader.decimal "rate_per"
          VolumeAmount = reader.decimal "volume_amount"
          VolumeUnit = reader.text "volume_unit"
          CarrierRate = reader.decimal "carrier_rate"
          TaxRate = reader.decimal "tax_rate"
          TaxProvince = reader.text "tax_province"
          BillingEndDate = reader.dateTime "billing_date"
          DateIncurred = reader.dateTime "date_incurred" }
      Certificate = certificate
      ProductLine = reader.text "retro_update_product_line"
      Coverage = reader.textOrNone "retro_update_coverage"
      Option = reader.text "retro_update_option" }

let getRetroUpdates reader mapper (updateType : RetroactiveCertificateUpdateType) carrierName billingPeriod =
    let sql =
        "
        select
            rcu.id as retroactive_certificate_update_id,
            rcu.backdate as backdate,
            rcu.product_line as retro_update_product_line,
            rcu.coverage as retro_update_coverage,
            rcu.option as retro_update_option,

            -- usages to be corrected
            u.id,
            u.correlated_usage_id,
            u.usage_type,
            u.certificate_number,
            u.carrier_name,
            u.client_name,
            u.policy_number,
            u.scb_policy_number,
            u.benefit_start_date,
            u.benefit_end_date,
            u.division,
            u.product_line,
            u.product_line_group,
            u.coverage,
            u.option,
            u.rate_per,
            u.volume_amount,
            u.volume_unit,
            u.carrier_rate,
            u.tax_rate,
            u.tax_province,
            u.billing_date,
            u.date_incurred,

            -- certificate
            cert.certificate
        from retroactive_certificate_update rcu
        join certificate_usage u

            -- find, all past, usages match cert, carrier, product, coverage, and line
            on  rcu.certificate_number = u.certificate_number
            and rcu.carrier_name = u.carrier_name
            and rcu.policy_number = u.policy_number
            and rcu.product_line = u.product_line
            and rcu.coverage = u.coverage
            and rcu.option = u.option

	        -- find only those after the retroactive update date
            --   and before the current billing period
            and u.billing_date >= rcu.backdate
            and @billingStart > u.date_incurred

            -- don't correct corrections
            and usage_type = @usageType

        join certificate cert
            on  rcu.certificate_number = cert.certificate_number
            and rcu.carrier_name = cert.carrier
            and rcu.policy_number = cert.policy_number

        where
            rcu.type = @updateType
            -- only look at retroactive updates for the given carrier
            --   in the given billing period
            and rcu.update_date <= @billingEnd
            and rcu.update_date > @billingStart
            and rcu.carrier_name = @carrierName
        "

    let parameters =
        [ "updateType", Sql.parameter (NpgsqlParameter("retro_cert_update_type", updateType))
          "carrierName", Sql.string carrierName
          "billingStart", Sql.timestamp billingPeriod.Start
          "billingEnd", Sql.timestamp billingPeriod.End
          "usageType", Sql.parameter (NpgsqlParameter("usage_type", CertificateUsageType.Charge)) ]

    reader sql parameters mapper

let mapRetroTransition deserialize billingPeriod (reader : IRowReader) =
    let certificate = reader.text "certificate" |> deserialize
    let backdate = reader.dateTime "backdate"

    let usage =
        reader.textOrNone "certificate_number"
        |> Option.map (fun certificate_number ->

            { CertificateUsageDao.Id = reader.uuid "id"
              CorrelatedUsageId = reader.uuidOrNone "correlated_usage_id"
              CausationId = reader.uuid "retroactive_certificate_update_id"
              UsageType = reader.enum<CertificateUsageType> "usage_type"
              CertificateNumber = certificate_number
              CarrierName = reader.text "carrier_name"
              ClientName = reader.text "client_name"
              PolicyNumber = reader.text "policy_number"
              ScbPolicyNumber = reader.text "scb_policy_number"
              BenefitStartDate = reader.dateTime "benefit_start_date"
              BenefitEndDate = reader.dateTimeOrNone "benefit_end_date"
              Division = reader.text "division"
              ProductLine = reader.text "product_line"
              ProductLineGroup = reader.text "product_line_group"
              Coverage = reader.textOrNone "coverage"
              Option = reader.text "option"
              RatePer = reader.decimal "rate_per"
              VolumeAmount = reader.decimal "volume_amount"
              VolumeUnit = reader.text "volume_unit"
              CarrierRate = reader.decimal "carrier_rate"
              TaxRate = reader.decimal "tax_rate"
              TaxProvince = reader.text "tax_province"
              BillingEndDate = reader.dateTime "billing_date"
              DateIncurred = reader.dateTime "date_incurred" })

    { RetroactiveCertificateUsageTransitionDao.RetroCertificateUpdateId =
        reader.uuid "retroactive_certificate_update_id"
      Certificate = certificate
      Usage = usage
      ProductLine = reader.text "retro_update_product_line"
      Coverage = reader.textOrNone "retro_update_coverage"
      Option = reader.text "retro_update_option"
      BillingStart = billingPeriod.Start
      BillingEnd = billingPeriod.End
      Backdate = backdate }

let getRetroTerminations reader mapper carrierName billingPeriod =
    let sql =
        "
        select
            rcu.id as retroactive_certificate_update_id,
            rcu.backdate as backdate,
            rcu.product_line as retro_update_product_line,
            rcu.coverage as retro_update_coverage,
            rcu.option as retro_update_option,

            -- usages to be corrected
            u.id,
            u.correlated_usage_id,
            u.usage_type,
            u.certificate_number,
            u.carrier_name,
            u.client_name,
            u.policy_number,
            u.scb_policy_number,
            u.benefit_start_date,
            u.benefit_end_date,
            u.division,
            u.product_line,
            u.product_line_group,
            u.coverage,
            u.option,
            u.rate_per,
            u.volume_amount,
            u.volume_unit,
            u.carrier_rate,
            u.tax_rate,
            u.tax_province,
            u.billing_date,
            u.date_incurred,

            -- certificate
            cert.certificate
        from retroactive_certificate_update rcu
        left join certificate_usage u

            -- find, all past, usages match cert, carrier, product, coverage, and line
            on  rcu.certificate_number = u.certificate_number
            and rcu.carrier_name = u.carrier_name
            and rcu.policy_number = u.policy_number
            and rcu.product_line = u.product_line
            and rcu.coverage = u.coverage
            and rcu.option = u.option

	        -- find only those before the current billing period
            and u.date_incurred < @billingStart

            -- don't correct corrections
            and usage_type = @usageType

        join certificate cert
            on  rcu.certificate_number = cert.certificate_number
            and rcu.carrier_name = cert.carrier
            and rcu.policy_number = cert.policy_number

        where
            rcu.type = @terminationType
            -- only look at retroactive updates for the given carrier
            --   in the given billing period
            and rcu.update_date <= @billingEnd
            and rcu.update_date > @billingStart
            and rcu.carrier_name = @carrierName
        "

    let parameters =
        [ "terminationType",
          Sql.parameter (NpgsqlParameter("retro_cert_update_type", RetroactiveCertificateUpdateType.Termination))
          "carrierName", Sql.string carrierName
          "billingStart", Sql.timestamp billingPeriod.Start
          "billingEnd", Sql.timestamp billingPeriod.End
          "usageType", Sql.parameter (NpgsqlParameter("usage_type", CertificateUsageType.Charge)) ]

    reader sql parameters (mapper billingPeriod)

let getRetroEnrollments reader mapper carrierName billingPeriod =
    let sql =
        "
        select
            rcu.id as retroactive_certificate_update_id,
            rcu.backdate as backdate,
            rcu.product_line as retro_update_product_line,
            rcu.coverage as retro_update_coverage,
            rcu.option as retro_update_option,

            -- usages to be corrected
            u.id,
            u.correlated_usage_id,
            u.usage_type,
            u.certificate_number,
            u.carrier_name,
            u.client_name,
            u.policy_number,
            u.scb_policy_number,
            u.benefit_start_date,
            u.benefit_end_date,
            u.division,
            u.product_line,
            u.product_line_group,
            u.coverage,
            u.option,
            u.rate_per,
            u.volume_amount,
            u.volume_unit,
            u.carrier_rate,
            u.tax_rate,
            u.tax_province,
            u.billing_date,
            u.date_incurred,

            -- certificate
            cert.certificate
        from retroactive_certificate_update rcu
        left join certificate_usage u

            -- find, all past, usages match cert, carrier, product, coverage, and line
            on  rcu.certificate_number = u.certificate_number
            and rcu.carrier_name = u.carrier_name
            and rcu.policy_number = u.policy_number
            and rcu.product_line = u.product_line
            and rcu.coverage = u.coverage
            and rcu.option = u.option

	        -- find only those before the current billing period
            and u.date_incurred < @billingStart

            -- don't correct corrections
            and usage_type = @usageType

        join certificate cert
            on  rcu.certificate_number = cert.certificate_number
            and rcu.carrier_name = cert.carrier
            and rcu.policy_number = cert.policy_number

        where
            type = @enrollmentType
            -- only look at retroactive updates for the given carrier
            --   in the given billing period
            and rcu.update_date <= @billingEnd
            and rcu.update_date > @billingStart
            and rcu.carrier_name = @carrierName
        "

    let parameters =
        [ "enrollmentType",
          Sql.parameter (NpgsqlParameter("retro_cert_update_type", RetroactiveCertificateUpdateType.Enrollment))
          "carrierName", Sql.string carrierName
          "billingStart", Sql.timestamp billingPeriod.Start
          "billingEnd", Sql.timestamp billingPeriod.End
          "usageType", Sql.parameter (NpgsqlParameter("usage_type", CertificateUsageType.Charge)) ]

    reader sql parameters (mapper billingPeriod)

let insertCorrections transact (daos : CertificateUsageDao option list) =
    let sql =
        "
        insert into certificate_usage
        (
            id,
            correlated_usage_id,
            causation_id,
            usage_type,
            billing_date,
            date_incurred,
            certificate_number,
            carrier_name,
            client_name,
            policy_number,
            scb_policy_number,
            benefit_start_date,
            benefit_end_date,
            division,
            product_line,
            product_line_group,
            coverage,
            option,
            rate_per,
            volume_amount,
            volume_unit,
            carrier_rate,
            tax_rate,
            tax_province
        )
        values
        (
            @id,
            @correlatedUsageId,
            @causationId,
            @usageType,
            @billingDate,
            @dateIncurred,
            @certificateNumber,
            @carrierName,
            @clientName,
            @policyNumber,
            @scbPolicyNumber,
            @benefitStartDate,
            @benefitEndDate,
            @division,
            @productLine,
            @productLineGroup,
            @coverage,
            @option,
            @ratePer,
            @volumeAmount,
            @volumeUnit,
            @carrierRate,
            @taxRate,
            @taxProvince
        )
        "

    let parameters =
        daos
        |> List.map (fun optionalDao ->
            optionalDao
            |> Option.map (fun dao ->
                [ "id", Sql.uuid dao.Id
                  "correlatedUsageId", Sql.uuidOrNone dao.CorrelatedUsageId
                  "causationId", Sql.uuid dao.CausationId
                  "usageType", Sql.parameter (NpgsqlParameter("certificate_usage_type", dao.UsageType))
                  "certificateNumber", Sql.text dao.CertificateNumber
                  "carrierName", Sql.text dao.CarrierName
                  "clientName", Sql.text dao.ClientName
                  "policyNumber", Sql.text dao.PolicyNumber
                  "scbPolicyNumber", Sql.text dao.ScbPolicyNumber
                  "benefitStartDate", Sql.date dao.BenefitStartDate
                  "benefitEndDate", Sql.dateOrNone dao.BenefitEndDate
                  "division", Sql.text dao.Division
                  "productLine", Sql.text dao.ProductLine
                  "productLineGroup", Sql.text dao.ProductLineGroup
                  "coverage", Sql.textOrNone dao.Coverage
                  "option", Sql.text dao.Option
                  "ratePer", Sql.decimal dao.RatePer
                  "volumeAmount", Sql.decimal dao.VolumeAmount
                  "volumeUnit", Sql.text dao.VolumeUnit
                  "carrierRate", Sql.decimal dao.CarrierRate
                  "taxRate", Sql.decimal dao.TaxRate
                  "taxProvince", Sql.text dao.TaxProvince
                  "billingDate", Sql.timestamp dao.BillingEndDate
                  "dateIncurred", Sql.timestamp dao.DateIncurred ]))
        |> List.choose id

    transact [ sql, parameters ]
