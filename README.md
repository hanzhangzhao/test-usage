# Certificate Usage

Listen for Membership events and track the usage for each certificate within SCBC.

## Runbook

If you having issues with the running system or want more information about the running system in the [runbook](./runbook/Readme.md)

## Installation

OS X Windows & Linux:

```sh
dotnet restore
dotnet build
```

## Usage example

### Run Service

```sh
ASPNETCORE_ENVIRONMENT=Local dotnet run -p CertificateUsage
```

### Format source code

```sh
dotnet fantomas -r .
```

## Development setup

### Unit Tests

```sh
dotnet test CertificateUsage.Tests/CertificateUsage.Tests.fsproj
```

### Coverage

```sh
dotnet test CertificateUsage.Tests/CertificateUsage.Tests.fsproj \
  /p:AltCover=true \
  /p:AltCoverCobertura=cobertura-coverage.xml \
  /p:AltCoverAssemblyExcludeFilter='^(xunit||FSharp||AltCover.*||Microsoft.*||System.*||Humanizer.*||ServiceStack.*||CertificateUsage.Tests||protobuf-net.*||Npgsql.FSharp||Spekt||Serilog.*).*$' \
  --no-restore

dotnet reportgenerator -reports:"./CertificateUsage.Tests/cobertura-coverage.xml" -targetdir:"coverage"
```

[Current Coverage Report](https://gitlab.com/sterlingcapitalbrokers/brokerage/ClaimsData-service/-/jobs/artifacts/main/file/coverage/index.html?job=test)

### Lint

```sh
dotnet fsharplint lint CertificateUsage.sln
```

## Tools

```sh
dotnet tool restore
```

