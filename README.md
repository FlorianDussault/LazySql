# LazySql

LazySql is a micro ORM to simplify the interfacing between an application and a Sql Server Database.

# Whats new in 2.1.0-preview?

* Support of ``object`` and ``dynamic`` types!
* Namespace fixed
* Support of SqlCredential

# Packages

[![Build status](https://ci.appveyor.com/api/projects/status/q5mj8574x62xi1o5/branch/master?svg=true)](https://ci.appveyor.com/project/FlorianDussault/lazysql/branch/master)


| Package | NuGet |  Downloads | **.NET** | **.NET Framework** | **.NET Standard** | **Sql Server** |
| ------- | ----- | ---------- |:--------:| ------------------:|:-----------------:|:----------:|
| [LazySqlStandard.Engine](https://www.nuget.org/packages/LazySqlStandard.Engine/) | [![LazySqlStandard.Engine](https://img.shields.io/nuget/v/LazySqlStandard.Engine.svg)](https://www.nuget.org/packages/LazySqlStandard.Engine/) | [![LazySqlStandard.Engine](https://img.shields.io/nuget/dt/LazySqlStandard.Engine.svg)](https://www.nuget.org/packages/LazySqlStandard.Engine/) | 6.0 | 4.8 | 2.0 | > Sql Server 2012 |


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

## Select some data in a table with Expression (WHERE)

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

This will generate the following SQL query:

```sql
UPDATE Cars SET Name = 'Second Car' WHERE id = 1
```

## Update multiple rows in database

```cs
Car updateCar = new Car() {Name = "New Name"};
LazyClient.Update<Car>(updateCar, c=>c.Name == null || Id = 0);
```
This will generate the following SQL query:

```sql
UPDATE Cars SET Name = 'New Name' WHERE Name IS NULL OR Id = 0
```
## Delete in database

If your object is derived from LazyBase, you can easily delete it:

```cs
Car car = new Car() {Id = 1};
car.Delete();
```

This will generate will generate the following SQL query:

```sql
DELETE Cars WHERE id = 1
```

## Delete multiple rows in database

```cs
LazyClient.Delete<Car>(c=>!c.Enabled);
```

This will generate will generate the following SQL query:

```sql
DELETE Cars WHERE Enabled = 0
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

# Documentation

[Click here](https://lazysql.floriandussault.dev/) for the full documentation, F.A.Q. and Release Note !

# License

LazySql is licensed under the terms of the [MIT](https://choosealicense.com/licenses/mit/) license and is available for free.

# Links

* My Web site:  [https://floriandussault.dev/](https://floriandussault.dev/)
* Documentation: [https://lazysql.floriandussault.dev/](https://lazysql.floriandussault.dev/)
