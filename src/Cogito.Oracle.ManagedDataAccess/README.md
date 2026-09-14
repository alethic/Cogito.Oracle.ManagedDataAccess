# Cogito.Oracle.ManagedDataAccess

Oracle Advanced Queuing for the fully managed ODP.NET driver.

## Why

Oracle's managed driver dropped the AQ support the unmanaged one had, so applications using Advanced
Queuing were stuck on the unmanaged driver and its native client install. This implements AQ against
the managed driver, so dequeuing a message no longer requires Oracle client binaries on the machine.

## Install

```shell
dotnet add package Cogito.Oracle.ManagedDataAccess
```

## Use

Dequeue against an `OracleConnection` from the managed driver, with no unmanaged client present.

Targets .NET Framework; see `Cogito.Oracle.ManagedDataAccess.Core` for `netstandard2.0`.

## License

MIT.
