# LazySql

LazySql is a micro ORM to simplify the interfacing between an application and a database.

![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg) ![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)

:star: Star me on GitHub — it motivates me a lot!

## Table of content

- [Installation](#installation)
- [Compatibility](#compatibility)
   - [Sql Server](#sql-server)
      - [Sql queries](#sql-queries)
      - [Data Types](#data-types)
- [License](#license)
- [Links](#links)

## Compatibility

### .NET

|             | **.NET Core** | **.NET Framework** | **.NET Standard** |
|-------------|:-------------:|:------------------:|:-----------------:|
| **Version** |      6.0      |         4.8        |        2.0        |

### Databases

#### Sql Server
Minimum version: Sql Server 2012

##### Sql queries

| **Type of query**                             | **Supported** | **Comment**          |
|-----------------------------------------------|:-------------:|----------------------|
| **Standard queries (SELECT, UPDATE, DELETE)** |       🟢       | Generated by the ORM |
| **Bulk Insert**                               |       🟡       | In Progress          |
| **Stored Procedures**                         |       🟡       | In Progress          |
| **Views**                                     |       🟡       | In Progress          |
| **User-Defined SQL Functions**                |       🔴       | Nothing planned      |

##### Data Types

| **Data Type**                  | **Supported** | **Comment**     |
|--------------------------------|:-------------:|-----------------|
| bigint                         |       🟢      |                 |
| numeric                        |       🟢      |                 |
| bit                            |       🟢      |                 |
| smallint                       |       🟢      |                 |
| decimal                        |       🟢      |                 |
| smallmoney                     |       🟢      |                 |
| int                            |       🟢      |                 |
| tinyint                        |       🟢      |                 |
| money                          |       🟢      |                 |
| float                          |       🟢      |                 |
| real                           |       🟢      |                 |
| date                           |       🟢      |                 |
| datetimeoffset                 |       🟢      |                 |
| datetime2                      |       🟢      |                 |
| smalldatetime                  |       🟢      |                 |
| datetime                       |       🟢      |                 |
| time                           |       🟢      |                 |
| char                           |       🟢      |                 |
| varchar                        |       🟢      |                 |
| text                           |       🟢      |                 |
| nchar                          |       🟢      |                 |
| nvarchar                       |       🟢      |                 |
| ntext                          |       🟢      |                 |
| binary                         |       🟢      |                 |
| varbinary                      |       🟢      |                 |
| image                          |       🟢      |                 |
| hierarchyid                    |       🔴      | Nothing planned |
| uniqueidentifier               |       🟢      |                 |
| xml                            |       🟢      |                 |
| Spatial Geometry Types         |       🔴      | Nothing planned |
| Spatial Geography Types        |       🔴      | Nothing planned |

## Installation

Use NuGet ([LazySqlStandard.Engine](https://www.nuget.org/packages/LazySqlStandard.Engine/)) !

Packet manager:
```sh
PM> NuGet\Install-Package LazySqlStandard.Engine -Version 1.0.5-alpha
```

.NET CLI:
```sh
> dotnet add package LazySqlStandard.Engine --version 1.0.5-alpha
```

Paket CLI:
```sh
> paket add LazySqlStandard.Engine --version 1.0.5-alpha
```

Paket CLI:
```xml
<PackageReference Include="LazySqlStandard.Engine" Version="1.0.5-alpha" />
```

## License

LazySql is licensed under the terms of the [MIT](https://choosealicense.com/licenses/mit/) license and is available for free.

## Links

* [My Web site](https://floriandussault.dev/)
