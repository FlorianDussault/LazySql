# LazySql

[![Build status](https://ci.appveyor.com/api/projects/status/q5mj8574x62xi1o5/branch/master?svg=true)](https://ci.appveyor.com/project/FlorianDussault/lazysql/branch/master) ![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)

![Nuget](https://img.shields.io/nuget/v/LazySqlStandard.Engine)

LazySql is a micro ORM to simplify the interfacing between an application and a database.

:star: Star me on GitHub — it motivates me a lot!

# Compatibility

## .NET

|             | **.NET** | **.NET Framework** | **.NET Standard** |
|-------------|:-------------:|:------------------:|:-----------------:|
| **Version** |      6.0      |         4.8        |        2.0        |

## Sql Server

Minimum version: Sql Server 2012

# How to use it?

## First of all, the initialization

LazySql uses a singleton, so you only have to initialise the client once.

```cs
LazyClient.Initialize("Server=my-server\SQL2019;Database=MyDataBase;");
```

## Select all the data in a table

```cs
foreach (Car car in LazyClient.Select<Car>()) {
   Console.WriteLine(car.Name);
}
```
This automatically generates the following SQL query in the background:
```sql
SELECT Id, Name, ... FROM Cars
```

## Select some data in a table (with Expression)

```cs
foreach (Car car in LazyClient.Select<Car>().Where(c=>c.Country == "FR" && c.Enabled).OrderBy(c=>c.Name)) {
   Console.WriteLine(car.Name);
}
```

This automatically generates the following SQL query in the background:

```sql
SELECT Id, Name, ... FROM Cars WHERE Country = 'FR' AND Enabled = 1 ORDER BY Name
```

## Update in database

If your object is derived from LazyBase, you can easily update it:

```cs
Car car = new Car() {Id = 1, Name = "Second Car"};
car.Update();
```

Or (if your object doesn't derive from LazyBase):
```cs
Car car = new Car() {Id = 1, Name = "Second Car"};
car.Update(c=>c.Id == car.Id);
```

These two methods will generate the following SQL query:

```sql
UPDATE Cars SET Name = 'Second Car' WHERE id = 1
```

## Delete in database

If your object is derived from LazyBase, you can easily delete it:

```cs
Car car = new Car() {Id = 1};
car.Delete();
```

Or (if your object doesn't derive from LazyBase):
```cs
Car car = new Car() {Id = 1};
car.Delete(c=>c.Id == car.Id);
```

These two methods will generate the following SQL query:

```sql
DELETE Cars WHERE id = 1
```

## And more...

### Stored Procedure

```cs
StoredProcedureResult result = LazyClient.StoredProcedure("my_procedure",
            new SqlArguments()
               .Add("@FirstName", SqlType.NVarChar, "Florian")
               .Add("@Age", SqlType.Int, 40)
               .AddOut("@Id", SqlType.Int) // OUTPUT PARAMETER
        );

// Get return value (RETURN)
Console.WriteLine($"Result: {result.ReturnValue}");
// Get Output value
Console.WriteLine($"Result: {result.Output.Id}");

// Get first table with dynamic types
IEnumerable<dynamic> dynamicValues = result.Tables[0].Parse();
// Get second table with an object type
IEnumerable<Profil> result.Tables[1].Parse<Profil>();
```
### Bulk Insert

```cs
List<Car> cars = GetCars();
LazyClient.BulkInsert(cars);
```

### ExecuteNonQuery method

```cs
int affectedRows = LazyClient.ExecuteNonQuery("DELETE FROM web_users");
```

### ExecuteScalar method

```cs
string firstName = LazyClient.ExecuteNonQuery<string>("SELECT firstname FROM web_users WHERE Id = @Id", new SqlArgument("@Id", SqlType.Int, 50));
```

![Et voilà](https://media1.giphy.com/media/uLYTKQE2cftpLFMpEG/giphy.gif?cid=ecf05e47a8j3vm5qxa15yj2us0en0ne34qunwsjp7d4vnrgn&rid=giphy.gif&ct=g)
()


# Documentation

[Click here](https://lazysql.floriandussault.dev/) for the full documentation, F.A.Q. and Release Note !

# License

LazySql is licensed under the terms of the [MIT](https://choosealicense.com/licenses/mit/) license and is available for free.

# Links

* My Web site:  [https://floriandussault.dev/](https://floriandussault.dev/)
* Documentation: [https://lazysql.floriandussault.dev/](https://lazysql.floriandussault.dev/)
