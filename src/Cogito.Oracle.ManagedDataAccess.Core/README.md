# Cogito.Oracle.ManagedDataAccess.Core

Oracle Advanced Queuing for the fully managed ODP.NET Core driver.

## Why

The same gap as on .NET Framework: `Oracle.ManagedDataAccess.Core` has no Advanced Queuing support,
which leaves AQ applications unable to move off the unmanaged driver. This implements it.

## Install

```shell
dotnet add package Cogito.Oracle.ManagedDataAccess.Core
```

## Use

Dequeue against an `OracleConnection` from `Oracle.ManagedDataAccess.Core`, with no native client
install.

This is the `netstandard2.0` build; `Cogito.Oracle.ManagedDataAccess` is the .NET Framework one.

## License

MIT.
