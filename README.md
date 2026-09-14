# Cogito.Oracle.ManagedDataAccess

[![Build](https://github.com/alethic/Cogito.Oracle.ManagedDataAccess/actions/workflows/Cogito.Oracle.ManagedDataAccess.yml/badge.svg)](https://github.com/alethic/Cogito.Oracle.ManagedDataAccess/actions/workflows/Cogito.Oracle.ManagedDataAccess.yml)

Oracle Advanced Queuing for the fully managed ODP.NET driver, so AQ no longer requires a native client install.

## Packages

**[Cogito.Oracle.ManagedDataAccess](https://www.nuget.org/packages/Cogito.Oracle.ManagedDataAccess)** — Oracle Advanced Queuing for the fully managed ODP.NET driver.

**[Cogito.Oracle.ManagedDataAccess.Core](https://www.nuget.org/packages/Cogito.Oracle.ManagedDataAccess.Core)** — Oracle Advanced Queuing for the fully managed ODP.NET Core driver.

Each package carries its own README with the detail; the links above go to nuget.org.

## Building

```shell
dotnet restore Cogito.Oracle.ManagedDataAccess.slnx
dotnet msbuild -p:Configuration=Release Cogito.Oracle.ManagedDataAccess.dist.msbuildproj
```

Packages are staged into `dist/nuget` and test suites into `dist/tests`; run a suite with
`dotnet test -f <tfm> <path to its assembly>`.

## License

MIT — see [LICENSE](LICENSE).
