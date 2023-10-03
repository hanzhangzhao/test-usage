namespace CertificateUsage.Api.Repository

open System

open Npgsql.FSharp

open Serilog
open FsToolkit.ErrorHandling

open CertificateUsage.Dao
open CertificateUsage.Serialization
open CertificateUsage.Api.Dao

open CertificateUsage.IRowReader

module Usage =
    type UsageForCarrierByDateParameters = { Carrier : string; Cutoff : DateTime }

    let mapRowToCertificateUsageChangeDao (reader : IRowReader) : CertificateUsageChangeDao option =
        option {
            let! data =
                try
                    { CertificateNumber = reader.text "certificate_number"
                      Carrier = reader.text "carrier"
                      PolicyNumber = reader.text "policy_number"
                      ScbPolicyNumber = reader.text "scb_policy_number"
                      Effective = reader.dateTime "effective"
                      Type = reader.text "type" |> CoverageType.fromString
                      CoverageData = reader.text "coverage_data" |> deserialize
                      EventMetadata = reader.text "event_meta" |> deserialize }
                    |> Some
                with ex ->
                    Log.Logger.Error(ex.Message)
                    None

            return data
        }

    [<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
    let getUsageForCarrierByDate selector mapper =
        fun (usageForCarrierByDateParameters : UsageForCarrierByDateParameters) ->
            let query =
                "
                SELECT
                id,
                certificate_number,
                carrier,
                policy_number,
                scb_policy_number,
                date_trunc('month',effective)::date as effective,
                type,
                coverage_data,
                event_meta,
                created
				FROM certificate_usage_changes
				WHERE date_trunc('month',effective)::date <= date_trunc('month',@effective)::date
				AND carrier = @carrier
				ORDER BY effective DESC"

            let parameters =
                [ "carrier", Sql.string usageForCarrierByDateParameters.Carrier
                  "effective", Sql.timestamp usageForCarrierByDateParameters.Cutoff ]

            selector query parameters mapper

    let mapCertificateRowToUsageLineDao (reader : IRowReader) =
        option {
            let! data =
                try
                    { UsageLineDao.UsageType =
                        reader.enum<CertificateUsageType> "usage_type" |> CertificateUsageType.value
                      CertificateNumber = reader.text "certificate_number"
                      CarrierCode = reader.text "carrier_name"
                      ClientName = reader.text "client_name"
                      PolicyNumber = reader.text "policy_number"
                      ProductLine = reader.text "product_line"
                      Coverage = reader.textOrNone "coverage"
                      ProductOption = reader.text "option"
                      Volume = reader.decimal "volume_amount"
                      Lives = 0m
                      TaxRate = reader.decimal "tax_rate"
                      TaxProvince = reader.text "tax_province"
                      Year = (reader.dateTime "date_incurred").Year
                      Month = (reader.dateTime "date_incurred").Month
                      CarrierRate = reader.decimal "carrier_rate"
                      ClientRate = 0.0m
                      Division = reader.text "division"
                      VolumeUnit = reader.text "volume_unit" }
                    |> Some
                with ex ->
                    Log.Logger.Error(ex.Message)
                    None

            return data
        }

    [<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
    let getUsageForCarrierByDateFromClosedBook selector mapper carrierName billingPeriodEndDate =
        let sql =
            "
            SELECT
                usage_type,
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
                tax_province,
                date_incurred,
            FROM certificate_usage
            WHERE carrier_name = @carrierName
                AND billing_date = @billingDate
            "

        let parameters =
            [ "carrierName", Sql.text carrierName
              "billingDate", Sql.timestamp billingPeriodEndDate ]

        task {
            let! result = selector sql parameters mapper
            return Ok result
        }

    let mapRowToRateChangeDataDto (reader : IRowReader) : RateUpdateDataDao option =
        option {
            let! data =
                try
                    { Carrier = reader.text "carrier"
                      PolicyNumber = reader.text "policy_number"
                      Option = reader.text "option"
                      Coverage = reader.textOrNone "coverage"
                      ProductLine = reader.text "product_line"
                      Effective = reader.dateTime "effective"
                      RateUpdateData = reader.text "rate_data" |> deserialize
                      EventMetadata = reader.text "event_meta" |> deserialize }
                    |> Some
                with ex ->
                    Log.Logger.Error(ex.Message)
                    None

            return data
        }

    [<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
    let getRateChangesForCarrierOnOrAfter selector mapper =
        fun (usageForCarrierByDateParameters : UsageForCarrierByDateParameters) ->
            let query =
                "
				WITH rate AS (
					SELECT
						carrier,
						policy_number,
						option,
						coverage,
					    product_line,
						date_trunc('month',effective)::date as effective,
						rate_data,
						event_meta,
						ROW_NUMBER() OVER (PARTITION BY
												cr.carrier,
												cr.policy_number,
            									cr.product_line,
												cr.option,
												cr.coverage
						   					ORDER BY cr.effective DESC) as rank

					FROM carrier_rates cr
					WHERE date_trunc('month',effective)::date >= date_trunc('month',@effective)::date
			        AND carrier = @carrier
				)
				SELECT
					carrier,
					policy_number,
					option,
					coverage,
					product_line,
					date_trunc('month',effective)::date as effective,
					rate_data,
					event_meta
				FROM rate
				WHERE rank = 1
                "

            let parameters =
                [ "carrier", Sql.string usageForCarrierByDateParameters.Carrier
                  "effective", Sql.timestamp (usageForCarrierByDateParameters.Cutoff.AddMonths(-1)) ]

            selector query parameters mapper

    let previewMapper (reader : IRowReader) : UsagePreviewDao option =
        option {
            let! data =
                try
                    { CertificateNumber = reader.text "certificate_number"
                      CarrierName = reader.text "carrier_name"
                      ClientName = reader.text "client_name"
                      PolicyNumber = reader.text "policy_number"
                      ScbPolicyNumber = reader.text "scb_policy_number"
                      BenefitStartDate = reader.dateTime "benefit_start_date"
                      BenefitEndDate = reader.dateTimeOrNone "benefit_end_date"
                      Division = reader.text "division"
                      ProductLine = reader.text "product_line"
                      ProductLineGroup = reader.textOrNone "product_line_group"
                      Coverage = reader.textOrNone "coverage"
                      Option = reader.text "option"
                      RatePer = reader.decimalOrNone "rate_per"
                      VolumeAmount = reader.decimal "volume_amount"
                      VolumeUnit = reader.text "volume_unit"
                      CarrierRate = reader.decimal "carrier_rate"
                      TaxRate = reader.decimalOrNone "tax_rate"
                      TaxProvince = reader.text "tax_province" }
                    |> Some
                with ex ->
                    Log.Logger.Error(ex.Message)
                    None

            return data
        }

    let previewUsage selector mapper carrierName =
        let querySql =
            "
            select
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
                and status = 'active'
            "

        let queryParameters = [ "carrierName", Sql.string carrierName ]

        selector querySql queryParameters mapper

    let mapClosedBookDate (reader : IRowReader) : DateTime = reader.dateTime "billing_date"

    let getClosedBookDates selector mapper carrierName =
        let sql =
            "
            select
                billing_date
            from certificate_usage
            where carrier_name = @carrierName
            group by billing_date
            order by billing_date desc
            "

        let parameters = [ "carrierName", Sql.text carrierName ]
        selector sql parameters mapper
