using System.Dynamic;
using System.Reflection;
using LazySql;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Insert")]
public class InsertTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void InsertLazy()
    {
        ClientTest.AddSimpleTables();
    }

    [Test]
    public void InsertLazyOnSchema()
    {
        LazyClient.Delete<PrimaryValue>();
        Assert.IsEmpty(LazyClient.Select<PrimaryValue>());
        for (int i = 0; i < 20; i++)
        {
            Assert.That(new PrimaryValue {Value = "U_" + i}.Insert(), Is.EqualTo(1));
            LazyClient.Insert(new PrimaryValue {Value = "F_" + i});
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(LazyClient.Select<PrimaryValue>(p=>p.Value == $"U_{i}").Count, Is.EqualTo(1));
            Assert.That(LazyClient.Select<PrimaryValue>(p=>p.Value == $"F_{i}").Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void InsertObject()
    {
        ClientTest.CleanTables();

        Simple_Table simpleTable = new()
        {
            Username = "Test1",
            Password = "Pass1"
        };
        LazyClient.Insert(simpleTable, nameof(Simple_Table.User_Id), nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled),
            nameof(Simple_Table.NotSqlType));

        Assert.That(simpleTable.User_Id, Is.EqualTo(0));

        simpleTable = new()
        {
            Username = "Test2",
            Password = "Pass2"
        };
        Assert.That(LazyClient.Insert(simpleTable, nameof(Simple_Table.User_Id), nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled),
            nameof(Simple_Table.NotSqlType)), Is.EqualTo(1));
        Assert.That(simpleTable.User_Id, Is.EqualTo(1));


        simpleTable = new()
        {
            User_Id = 999,
            Username = "Test3",
            Password = "Pass3"
        };
        LazyClient.Insert(simpleTable, nameof(Simple_Table.User_Id), nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled),
            nameof(Simple_Table.NotSqlType));
        Assert.That(simpleTable.User_Id, !Is.EqualTo(999));

        List<SimpleTable> values = LazyClient.Select<SimpleTable>().ToList();
        Assert.That(values[0].Id, Is.EqualTo(0));
        Assert.That(values[0].Username, Is.EqualTo("Test1"));
        Assert.That(values[0].Password, Is.EqualTo("Pass1"));

        Assert.That(values[1].Id, Is.EqualTo(1));
        Assert.That(values[1].Username, Is.EqualTo("Test2"));
        Assert.That(values[1].Password, Is.EqualTo("Pass2"));

        Assert.That(values[2].Id, Is.EqualTo(2));
        Assert.That(values[2].Username, Is.EqualTo("Test3"));
        Assert.That(values[2].Password, Is.EqualTo("Pass3"));
    }

    [Test]
    public void InsertObjectOnSchema()
    {
        LazyClient.Delete<dynamic>("lazys", "tablePrimary");
        Assert.IsEmpty(LazyClient.Select<TablePrimary>("lazys", "tableprimary"));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(LazyClient.Insert("lazys", "tableprimary", new TablePrimary { Value = "U_" + i }, nameof(TablePrimary.Id)), Is.EqualTo(1));
        }

        for (int i = 1; i < 21; i++)
        {
            List<TablePrimary> uValues = LazyClient.Select<TablePrimary>("lazys", "tableprimary", p => p.Id == i).ToList();
            Assert.That(uValues.Count, Is.EqualTo(1));
            Assert.That(uValues[0].Value, Is.EqualTo($"U_{i-1}"));
        }
    }

    [Test]
    public void InsertDynamic()
    {
        ClientTest.CleanTables();

        dynamic simpleTable = new
        {//User_Id = 1000,
            Username = "Test1",
            Password = "Pass1"
        };
        LazyClient.Insert("simple_table", simpleTable);
        
        dynamic simpleTable2 = new
        {
            Username = "Test2",
            Password = "Pass2"
        };
        LazyClient.Insert("simple_table", simpleTable2);

        dynamic simpleTable3 = new
        {
            Username = "Test3",
            Password = "Pass3",
            NotSqlType = 87
        };
        LazyClient.Insert("simple_table", simpleTable3, null, "NotSqlType");

        

        List<SimpleTable> values = LazyClient.Select<SimpleTable>().OrderBy(s=>s.Id).ToList();
        Assert.That(values[0].Username, Is.EqualTo("Test1"));
        Assert.That(values[0].Password, Is.EqualTo("Pass1"));

        Assert.That(values[1].Username, Is.EqualTo("Test2"));
        Assert.That(values[1].Password, Is.EqualTo("Pass2"));

        Assert.That(values[2].Username, Is.EqualTo("Test3"));
        Assert.That(values[2].Password, Is.EqualTo("Pass3"));


    }

    [Test]
    public void InsertDynamicOnSchema()
    {
        ClientTest.CleanTables();

        dynamic simpleTable = new
        {
            Age = 1,
            Name = "Pass1"
        };
        LazyClient.Insert("lazys", "WithoutKeys", simpleTable);

        dynamic simpleTable2 = new
        {
            Age = 2,
            Name = "Pass2"
        };
        LazyClient.Insert("lazys", "WithoutKeys", simpleTable2);

        dynamic simpleTable3 = new
        {
            Age = 3,
            Name = "Pass3",
            NotSqlType = 87
        };
        LazyClient.Insert("lazys", "WithoutKeys", simpleTable3, null, "NotSqlType");
        
        List<WithoutKeys> values = LazyClient.Select<WithoutKeys>().OrderBy(s => s.Age).ToList();
        Assert.That(values[0].Age, Is.EqualTo(1));
        Assert.That(values[0].Name, Is.EqualTo("Pass1"));

        Assert.That(values[1].Age, Is.EqualTo(2));
        Assert.That(values[1].Name, Is.EqualTo("Pass2"));

        Assert.That(values[2].Age, Is.EqualTo(3));
        Assert.That(values[2].Name, Is.EqualTo("Pass3"));
    }

    [Test]
    [Ignore("ExpandoObject to implement")]
    public void InsertExpandoObject()
    {
        ClientTest.CleanTables();

        dynamic simpleTable = new ExpandoObject();

        simpleTable.User_Id = 999;
        simpleTable.Username = "Test1";
        simpleTable.Password = "Pass1";
        
        LazyClient.Insert(simpleTable, "simple_table", null, nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType));
        Assert.That(simpleTable.User_Id, Is.EqualTo(1));

        dynamic simpleTable2 = new
        {
            Username = "Test2",
            Password = "Pass2"
        };
        LazyClient.Insert(simpleTable2, "simple_table", null, nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType));

        dynamic simpleTable3 = new
        {
            Username = "Test3",
            Password = "Pass3"
        };
        LazyClient.Insert(simpleTable3, "simple_table", null, nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType));

        List<SimpleTable> values = LazyClient.Select<SimpleTable>().OrderBy(s => s.Id).ToList();
        Assert.That(values[0].Username, Is.EqualTo("Test1"));
        Assert.That(values[0].Password, Is.EqualTo("Pass1"));

        Assert.That(values[1].Username, Is.EqualTo("Test2"));
        Assert.That(values[1].Password, Is.EqualTo("Pass2"));

        Assert.That(values[2].Username, Is.EqualTo("Test3"));
        Assert.That(values[2].Password, Is.EqualTo("Pass3"));


    }

    [Test]
    public void InsertNull()
    {
        ClientTest.CleanTables();

        new SimpleTable()
        {

        }.Insert();

        SimpleTable item = LazyClient.Select<SimpleTable>().First();
        Assert.That(item.Id, Is.EqualTo(0));
        Assert.IsNull(item.Username);
        Assert.IsNull(item.Password);
    }

    [Test]
    public void InsertNullOnSchema()
    {
        LazyClient.Truncate<PrimaryValue>();
        Assert.IsEmpty(LazyClient.Select<PrimaryValue>());
        new PrimaryValue()
        {

        }.Insert();

        PrimaryValue item = LazyClient.Select<PrimaryValue>().First();
        Assert.That(item.Key, Is.EqualTo(1));
        Assert.IsNull(item.Value);
    }

    [Test]
    public void InsertList()
    {
        ClientTest.CleanTables();

        List<SimpleTable> values = new()
        {
            new SimpleTable{Username = "U1"},
            new SimpleTable{Username = "U2"},
            new SimpleTable{Username = "U3"}
        };

        Assert.That(values.Insert(), Is.EqualTo(3));

        values = LazyClient.Select<SimpleTable>().OrderByAsc(s => s.Id).ToList();

        Assert.That(values[0].Username, Is.EqualTo("U1"));
        Assert.That(values[1].Username, Is.EqualTo("U2"));
        Assert.That(values[2].Username, Is.EqualTo("U3"));
    }

    [Test]
    public void InsertListOnSchema()
    {
        LazyClient.Truncate<PrimaryValue>();

        List<PrimaryValue> values = new()
        {
            new PrimaryValue{Value = "U1"},
            new PrimaryValue{Value = "U2"},
            new PrimaryValue{Value = "U3"}
        };

        Assert.That(values.Insert(), Is.EqualTo(3));

        values = LazyClient.Select<PrimaryValue>().OrderByAsc(s => s.Key).ToList();

        Assert.That(values[0].Value, Is.EqualTo("U1"));
        Assert.That(values[1].Value, Is.EqualTo("U2"));
        Assert.That(values[2].Value, Is.EqualTo("U3"));
    }

}