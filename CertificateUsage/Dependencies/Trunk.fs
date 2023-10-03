module CertificateUsage.Dependencies.Trunk

open Microsoft.Extensions.Configuration
open CertificateUsage.Dependencies.Leaves

open System.Diagnostics.CodeAnalysis

type Trunk =
    { CommandModelIO: WriteModel.IO
      EventStoreIO: EventStore.IO }
    
[<ExcludeFromCodeCoverage(Justification = "passthrough")>]        
let getConnectionString (configuration: IConfiguration) =
    configuration.GetValue<string> "Db:ConnectionString"

[<ExcludeFromCodeCoverage(Justification = "passthrough")>]
let compose (configuration: IConfiguration) =
    { CommandModelIO = WriteModel.compose (getConnectionString configuration)
      EventStoreIO = EventStore.compose configuration }
