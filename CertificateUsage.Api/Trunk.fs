namespace CertificateUsage.Api.Dependencies

open Microsoft.Extensions.Configuration

module Trunk =
    type Trunk =
        { UsageDataReaderDependencies: UsageDataDependencies.UsageDataReaderIO }

    [<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
    let getConnectionString (configuration: IConfiguration) =
        (configuration.GetValue<string> "Db:ConnectionString")

    [<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "passthrough")>]
    let compose (config: IConfiguration) =
        let connectionString = getConnectionString config

        { UsageDataReaderDependencies = UsageDataDependencies.UsageDataReaderIO.compose connectionString }
