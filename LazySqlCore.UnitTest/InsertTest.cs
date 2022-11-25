using System.Dynamic;
using LazySql.Engine;
using LazySql.Engine.Client;
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
    public void InsertObject()
    {
        ClientTest.CleanTables();

        Simple_Table simpleTable = new()
        {
            Username = "Test1",
            Password = "Pass1"
        };
        LazyClient.Insert(simpleTable, null, nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled),
            nameof(Simple_Table.NotSqlType));

        Assert.That(simpleTable.User_Id, Is.EqualTo(0));

        simpleTable = new()
        {
            Username = "Test2",
            Password = "Pass2"
        };
        LazyClient.Insert(simpleTable, null, nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled),
            nameof(Simple_Table.NotSqlType));
        Assert.That(simpleTable.User_Id, Is.EqualTo(1));


        simpleTable = new()
        {
            User_Id = 999,
            Username = "Test3",
            Password = "Pass3"
        };
        LazyClient.Insert(simpleTable, null, nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled),
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
    public void InsertDynamic()
    {
        ClientTest.CleanTables();

        dynamic simpleTable = new
        {//User_Id = 1000,
            Username = "Test1",
            Password = "Pass1"
        };
        LazyClient.Insert(simpleTable, "simple_table", null, nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType));


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

        List<SimpleTable> values = LazyClient.Select<SimpleTable>().OrderBy(s=>s.Id).ToList();
        Assert.That(values[0].Username, Is.EqualTo("Test1"));
        Assert.That(values[0].Password, Is.EqualTo("Pass1"));

        Assert.That(values[1].Username, Is.EqualTo("Test2"));
        Assert.That(values[1].Password, Is.EqualTo("Pass2"));

        Assert.That(values[2].Username, Is.EqualTo("Test3"));
        Assert.That(values[2].Password, Is.EqualTo("Pass3"));


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

}