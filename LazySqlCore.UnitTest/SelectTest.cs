using System.Diagnostics;
using LazySql.Engine;
using LazySql.Engine.Client;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Enums;
using LazySql.Engine.Exceptions;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Select")]
public class SelectTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void SelectLazy()
    {
        ClientTest.AddSimpleTables();
    }

    [Test]
    public void SelectObject()
    {
        ClientTest.AddSimpleTables();

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
        ClientTest.AddSimpleTables();
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
        ClientTest.AddSimpleTables();

        List<int> allowedIds = new() {5, 6, 7, 8, 9, 10, 20};

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(i => (i.Id > 4 && i.Id <= 10) || i.Username == "P20"))
        {
            Assert.IsTrue(allowedIds.Contains(simpleTable.Id));
        }
    }

    [Test]
    public void SelectObjectWithArgs()
    {
        ClientTest.AddSimpleTables();

        List<int> allowedIds = new() {5, 6, 7, 8, 9, 10, 20};

        foreach (Simple_Table simpleTable in LazyClient.Select<Simple_Table>(i => (i.User_Id > 4 && i.User_Id <= 10) || i.Username == "P20"))
        {
            Assert.IsTrue(allowedIds.Contains(simpleTable.User_Id));
        }
    }

    [Test]
    public void SelectDynamicWithArgs()
    {
        ClientTest.AddSimpleTables();

        List<int> allowedIds = new() {5, 6, 7, 8, 9, 10, 20};
        foreach (dynamic simpleTable in LazyClient.Select<dynamic>("simple_table", new SqlQuery("(user_id > 4 AND user_id <= 10) OR username = @p20").Add("@p20", "P20")))
        {
            Assert.IsTrue(allowedIds.Contains(simpleTable.user_id));
        }
    }

    [Test]
    public void SelectLazyOrderBy()
    {
        ClientTest.CleanTables();

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable {Username = rand.Next(0, 50).ToString("00")}.Insert();
        }

        IEnumerable<SimpleTable> list = LazyClient.Select<SimpleTable>().OrderByAsc(s => s.Username).OrderByDesc(s=>s.Password);
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
        ClientTest.CleanTables();

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable {Username = rand.Next(0, 50).ToString("00")}.Insert();
        }

        IEnumerable<Simple_Table> list = LazyClient.Select<Simple_Table>().OrderByAsc(s => s.Username);
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
        ClientTest.CleanTables();

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable {Username = rand.Next(0, 50).ToString("00")}.Insert();
        }

        IEnumerable<dynamic> list = LazyClient.Select<dynamic>("simple_table").OrderByAsc("username");
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
        ClientTest.CleanTables();

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
        ClientTest.CleanTables();

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