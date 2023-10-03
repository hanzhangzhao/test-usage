module CertificateUsage.Repository

open System
open System.Threading.Tasks

open CommonExtensionsAndTypesForNpgsqlFSharp
open Npgsql

open CertificateUsage.IRowReader
open CertificateUsage.Dao
open CertificateUsage.Domain

type WriteModelInsert = string -> (string * SqlValue) list -> Task<int>

type SqlReader =
    string
        -> (string * SqlValue) list
        -> (NpgsqlFSharpRowReader -> CertificateRecordDao)
        -> Task<CertificateRecordDao option>

type CertificateDeserializer = string -> CertificateDao

let rowToCertificateRecordDao (deserialize : CertificateDeserializer) (reader : IRowReader) : CertificateRecordDao =
    { CertificateNumber = reader.text "certificate_number"
      Carrier = reader.text "carrier"
      PolicyNumber = reader.text "policy_number"
      ClientName = reader.text "client_name"
      Certificate = reader.text "certificate" |> deserialize
      Status = reader.text "status" |> CertificateStatus.fromString }

let queryCertificate reader mapper (certificateUsage : CertificateUsage) =
    let query : string =
        "
            SELECT
                certificate_number,
                carrier,
                policy_number,
                client_name,
                status,
                certificate
            FROM certificate
            WHERE certificate_number = @certificateNumber
                AND carrier = @carrierName
                AND policy_number = @policyNumber
                AND client_name = @clientName
        "

    let queryParameters : (string * SqlValue) list =
        [ "certificateNumber", Sql.string certificateUsage.CertificateNumber
          "carrierName", Sql.string certificateUsage.Carrier
          "policyNumber", Sql.string certificateUsage.PolicyNumber
          "clientName", Sql.string certificateUsage.ClientName ]

    reader query queryParameters mapper

type CertificateRecordDaoSerializer = CertificateDao -> string

let upsertCertificateRecordDao
    (serialize : CertificateRecordDaoSerializer)
    (insert : WriteModelInsert)
    (dao : CertificateRecordDao)
    : Task<int> =

    let command =
        "
            INSERT INTO certificate
            (
                certificate_number,
                carrier,
                policy_number,
                client_name,
                status,
                certificate
             )
             VALUES
             (
                @certificateNumber,
                @carrier,
                @policyNumber,
                @clientName,
                @status,
                @certificate
             )
             ON CONFLICT (certificate_number, carrier, policy_number, client_name)
             DO UPDATE
             SET (
                certificate_number,
                carrier,
                policy_number,
                client_name,
                status,
                certificate,
                updated
             ) =
             (
                @certificateNumber,
                @carrier,
                @policyNumber,
                @clientName,
                @status,
                @certificate,
                @updated
             )
        "

    let parameters : (string * SqlValue) list =
        [ "certificateNumber", Sql.string dao.CertificateNumber
          "carrier", Sql.string dao.Carrier
          "policyNumber", Sql.string dao.PolicyNumber
          "clientName", Sql.string dao.ClientName
          "status", Sql.parameter (NpgsqlParameter("certificate_status", dao.Status))
          "certificate", Sql.jsonb (dao.Certificate |> serialize)
          "updated", Sql.timestamp DateTime.Now ]

    insert command parameters

let insertCertificateUsage serialize (insert : WriteModelInsert) (certificateUsage : CertificateUsageChangeDao) =
    task {
        let command =
            """
                INSERT INTO certificate_usage_changes
                (
                    certificate_number,
                    carrier,
                    policy_number,
                    effective,
                    type,
                    coverage_data,
                    event_meta,
                    scb_policy_number,
                    created
                )
                VALUES
                (
                    @certificateNumber,
                    @carrier,
                    @policyNumber,
                    @effective,
                    @type,
                    @coverageData,
                    @eventMetadata,
                    @scbPolicyNumber,
                    @created
                )
            """

        return!
            insert
                command
                [ "certificateNumber", Sql.string certificateUsage.CertificateNumber
                  "carrier", Sql.string certificateUsage.Carrier
                  "policyNumber", Sql.string certificateUsage.PolicyNumber
                  "effective", Sql.timestamp certificateUsage.Effective
                  "type", Sql.parameter (NpgsqlParameter("type", certificateUsage.Type))
                  "coverageData", Sql.jsonb (serialize (unbox certificateUsage.CoverageData))
                  "eventMetadata", Sql.jsonb (serialize (unbox certificateUsage.EventMetadata))
                  "scbPolicyNumber", Sql.string certificateUsage.ScbPolicyNumber
                  "created", Sql.timestamp DateTime.Now ]
    }

let insertRate serialize (insert : WriteModelInsert) (rate : RateUpdateDataDao) =
    let command =
        """
            INSERT INTO carrier_rates
            (
                carrier,
                policy_number,
                option,
                coverage,
                effective,
                rate_data,
                event_meta,
                created,
                product_line
            )
            VALUES
            (
                @carrier,
                @policyNumber,
                @option,
                @coverage,
                @effective,
                @rateData,
                @eventMetadata,
                @created,
                @product_line
            )
        """

    insert
        command
        [ "carrier", Sql.string rate.Carrier
          "policyNumber", Sql.string rate.PolicyNumber
          "option", Sql.string rate.Option
          "coverage", Sql.stringOrNone rate.Coverage
          "effective", Sql.timestamp rate.Effective
          "rateData", Sql.jsonb (serialize (unbox rate.RateUpdateData))
          "eventMetadata", Sql.jsonb (serialize (unbox rate.EventMetadata))
          "created", Sql.timestamp DateTime.Now
          "product_line", Sql.string rate.ProductLine ]

let insertRetroactiveCertificateUpdate
    (insert : WriteModelInsert)
    (retroactiveUpdateDao : RetroactiveCertificateUpdateDao)
    =
    let command =
        """
            INSERT INTO retroactive_certificate_update
            (
                type,
                certificate_number,
                carrier_name,
                client_name,
                policy_number,
                product_line,
                coverage,
                option,
                backdate,
                update_date
            )
            VALUES
            (
                @type,
                @certificateNumber,
                @carrierName,
                @clientName,
                @policyNumber,
                @productLine,
                @coverage,
                @option,
                @backdate,
                @updateDate
            )
        """

    insert
        command
        [ "type", Sql.parameter (NpgsqlParameter("retroactive_certificate_update_type", retroactiveUpdateDao.Type))
          "certificateNumber", Sql.string retroactiveUpdateDao.CertificateNumber
          "carrierName", Sql.string retroactiveUpdateDao.CarrierName
          "clientName", Sql.string retroactiveUpdateDao.ClientName
          "policyNumber", Sql.string retroactiveUpdateDao.PolicyNumber
          "productLine", Sql.string retroactiveUpdateDao.ProductLine
          "coverage", Sql.stringOrNone retroactiveUpdateDao.Coverage
          "option", Sql.string retroactiveUpdateDao.Option
          "backdate", Sql.timestamp retroactiveUpdateDao.Backdate
          "updateDate", Sql.timestamp retroactiveUpdateDao.UpdateDate ]

let nameMapper (reader : IRowReader) = reader.text "carrier_name"

let mapCarrierName selector mapper name =
    let query =
        """
        select
            carrier_name
        from carrier_alias_mapping
        where alias = @name
        """

    let parameters = [ "name", Sql.text name ]
    selector query parameters mapper
