using System.Diagnostics;
using LazySql.Engine;
using LazySql.Engine.Client;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Enums;
using LazySql.Engine.Exceptions;
using LazySqlCore.UnitTest.Tables;
using Microsoft.Data.SqlClient;

namespace LazySqlCore.UnitTest;

public class SelectTest
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
            SimpleTable st = new()
            {
                Username = $"U{i + 1}",
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
        Assert.That(LazyClient.Select<SimpleTable>().ToList().Count(), Is.EqualTo(COUNT_SIMPLE_TABLE));
        Assert.That(LazyClient.Select<ChildTable>().ToList().Count(),
            Is.EqualTo(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE));
    }

    [Test]
    public void SelectLazy()
    {
        AddSimpleTables();
    }

    [Test]
    public void SelectObject()
    {
        AddSimpleTables();

        List<Simple_Table> simpleTables = LazyClient.Select<Simple_Table>().ToList();
        Assert.That(simpleTables.Count, Is.EqualTo(20));

        for (int i = 0; i < simpleTables.Count; i++)
        {
            Assert.That(simpleTables[i].User_Id, Is.EqualTo(i));
            Assert.That(simpleTables[i].Username, Is.EqualTo($"U{i + 1}"));
            Assert.That(simpleTables[i].Password, Is.EqualTo($"P{i + 1}"));
            Assert.IsNull(simpleTables[i].NotInSqlFiled);
            Assert.IsNull(simpleTables[i].NotSqlType);
        }

    }

    [Test]
    public void SelectDynamic()
    {
        AddSimpleTables();
        Assert.Throws<LazySqlException>(() => { LazyClient.Select<dynamic>(); });
        List<dynamic> simpleTables = LazyClient.Select<dynamic>("simple_table").ToList();

        for (int i = 0; i < simpleTables.Count; i++)
        {
            Assert.That(simpleTables[i].user_id, Is.EqualTo(i));
            Assert.That(simpleTables[i].username, Is.EqualTo($"U{i + 1}"));
            Assert.That(simpleTables[i].password, Is.EqualTo($"P{i + 1}"));
            Assert.IsNull(simpleTables[i].extended_key);
        }
    }

    [Test]
    public void SelectLazyWithArgs()
    {
        AddSimpleTables();

        List<int> allowedIds = new() {5, 6, 7, 8, 9, 10, 20};

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>()
                     .Where(i => (i.Id > 4 && i.Id <= 10) || i.Username == "P20"))
        {
            Assert.IsTrue(allowedIds.Contains(simpleTable.Id));
        }
    }

    [Test]
    public void SelectObjectWithArgs()
    {
        AddSimpleTables();

        List<int> allowedIds = new() {5, 6, 7, 8, 9, 10, 20};

        foreach (Simple_Table simpleTable in LazyClient.Select<Simple_Table>()
                     .Where(i => (i.User_Id > 4 && i.User_Id <= 10) || i.Username == "P20"))
        {
            Assert.IsTrue(allowedIds.Contains(simpleTable.User_Id));
        }
    }

    [Test]
    public void SelectDynamicWithArgs()
    {
        AddSimpleTables();

        List<int> allowedIds = new() {5, 6, 7, 8, 9, 10, 20};
        // i => (i.User_Id > 4 && i.User_Id <= 10) || i.Username == "P20"
        string p20 = "P20";
        foreach (dynamic simpleTable in LazyClient.Select<dynamic>("simple_table").Where(
                     "(user_id > 4 AND user_id <= 10) OR username = @p20",
                     new SqlArguments().Add("@p20", SqlType.NVarChar, "P20")))
        {
            Assert.IsTrue(allowedIds.Contains(simpleTable.user_id));
        }
    }

    [Test]
    public void SelectLazyOrderBy()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable {Username = rand.Next(0, 50).ToString("00")}.Insert();
        }

        IEnumerable<SimpleTable>? list = LazyClient.Select<SimpleTable>().OrderByAsc(s => s.Username);
        int lastNumber = -1;
        foreach (SimpleTable simpleTable in list)
        {
            int number = int.Parse(simpleTable.Username);
            Assert.That(lastNumber, Is.LessThanOrEqualTo(number));
            lastNumber = number;
        }

        list = LazyClient.Select<SimpleTable>().OrderByDesc(s => s.Username);
        lastNumber = int.MaxValue;
        foreach (SimpleTable simpleTable in list)
        {
            int number = int.Parse(simpleTable.Username);
            Assert.That(lastNumber, Is.GreaterThanOrEqualTo(number));
            lastNumber = number;
        }
    }

    [Test]
    public void SelectObjectOrderBy()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable {Username = rand.Next(0, 50).ToString("00")}.Insert();
        }

        IEnumerable<Simple_Table>? list = LazyClient.Select<Simple_Table>().OrderByAsc(s => s.Username);
        int lastNumber = -1;
        foreach (Simple_Table simpleTable in list)
        {
            int number = int.Parse(simpleTable.Username);
            Assert.That(lastNumber, Is.LessThanOrEqualTo(number));
            lastNumber = number;
        }

        list = LazyClient.Select<Simple_Table>().OrderByDesc(s => s.Username);
        lastNumber = int.MaxValue;
        foreach (Simple_Table simpleTable in list)
        {
            int number = int.Parse(simpleTable.Username);
            Assert.That(lastNumber, Is.GreaterThanOrEqualTo(number));
            lastNumber = number;
        }

    }

    [Test]
    public void SelectDynamicOrderBy()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable {Username = rand.Next(0, 50).ToString("00")}.Insert();
        }

        IEnumerable<dynamic>? list = LazyClient.Select<dynamic>("simple_table").OrderByAsc("username");
        int lastNumber = -1;
        foreach (dynamic simpleTable in list)
        {
            int number = int.Parse(simpleTable.username);
            Assert.That(lastNumber, Is.LessThanOrEqualTo(number));
            lastNumber = number;
        }

        list = LazyClient.Select<dynamic>("simple_table").OrderByDesc("username");
        lastNumber = int.MaxValue;
        foreach (dynamic simpleTable in list)
        {
            int number = int.Parse(simpleTable.username);
            Assert.That(lastNumber, Is.GreaterThanOrEqualTo(number));
            lastNumber = number;
        }
    }

    [Test]
    public void SelectLazyTop()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable {Username = rand.Next(0, 50).ToString("00")}.Insert();
        }

        List<SimpleTable> list = LazyClient.Select<SimpleTable>().OrderByDesc(s => s.Id).Top(2).ToList();
        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list[0].Id, Is.EqualTo(999));
        Assert.That(list[1].Id, Is.EqualTo(998));
    }

    [Test]
    public void SelectPerformanceLazy()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        List<SimpleTable> simpleTables = new();
        const int maxRows = 1000000;
        for (int i = 0; i < maxRows; i++)
            simpleTables.Add(new SimpleTable() {Username = $"USERNAME {i}", Password = $"PASSWORD {i}"});
        LazyClient.BulkInsert(simpleTables);
        Assert.That(LazyClient.ExecuteScalar<int>("SELECT count(*) FROM simple_table"), Is.EqualTo(maxRows));

        Stopwatch stopwatch = Stopwatch.StartNew();
        simpleTables = LazyClient.Select<SimpleTable>().ToList();
        stopwatch.Stop();

    }

}