using System.Dynamic;
using LazySql.Engine;
using LazySql.Engine.Client;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

public class InsertTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    private void AddSimpleTables()
    {
        const int COUNT_SIMPLE_TABLE = 20;
        const int COUNT_CHILD_TABLE = 20;
        // Clear Table
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);
        // Add values
        Assert.IsEmpty(LazyClient.Select<SimpleTable>());
        int bot_id = 0;
        for (int i = 0; i < COUNT_SIMPLE_TABLE; i++)
        {
            SimpleTable? st = new()
            {
                Username = $"U{i+1}",
                Password = $"P{i + 1}"
            };
            st.Insert();

            for (int j = 0; j < COUNT_CHILD_TABLE; j++)
            {
                new ChildTable()
                {
                    Id = ++bot_id,
                    ParentId = st.Id,
                    TypeChar = "hello"
                }.Insert();
            }
        }
        // Check
        Assert.That(LazyClient.Select<SimpleTable>().ToList().Count, Is.EqualTo(COUNT_SIMPLE_TABLE));
        Assert.That(LazyClient.Select<ChildTable>().ToList().Count, Is.EqualTo(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE ));
    }

    [Test]
    public void InsertLazy()
    {
        AddSimpleTables();
    }

    [Test]
    public void InsertObject()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        
        Simple_Table simpleTable = new()
        {
            Username = "Test1",
            Password = "Pass1"
        };
        LazyClient.Insert(simpleTable,null, nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType));

        Assert.That(simpleTable.User_Id, Is.EqualTo(0));

        simpleTable = new()
        {
            Username = "Test2",
            Password = "Pass2"
        };
        LazyClient.Insert(simpleTable,null, nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType));
        Assert.That(simpleTable.User_Id, Is.EqualTo(1));

        simpleTable = new()
        {
            User_Id = 999,
            Username = "Test3",
            Password = "Pass3"
        };
        LazyClient.Insert(simpleTable, null, nameof(Simple_Table.User_Id), nameof(Simple_Table.NotInSqlFiled), nameof(Simple_Table.NotSqlType));
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
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);


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
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);


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
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        new SimpleTable()
        {

        }.Insert();

        SimpleTable item = LazyClient.Select<SimpleTable>().First();
        Assert.That(item.Id, Is.EqualTo(0));
        Assert.IsNull(item.Username);
        Assert.IsNull(item.Password);
    }
        
}