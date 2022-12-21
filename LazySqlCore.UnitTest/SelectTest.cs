using System.ComponentModel;
using System.Diagnostics;
using LazySql;
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
    public void SelectLazyOnSchema()
    {
        LazyClient.Delete<PrimaryValue>();
        Assert.IsEmpty(LazyClient.Select<PrimaryValue>());
        for (int i = 0; i < 20; i++)
        {
            Assert.That(new PrimaryValue { Value = "U_" + i }.Insert(), Is.EqualTo(1));
            LazyClient.Insert(new PrimaryValue { Value = "F_" + i });
        }
        Assert.That(LazyClient.Select<PrimaryValue>().Count, Is.EqualTo(40));
        for (int i = 0; i < 20; i++)
        {
            Assert.That(LazyClient.Select<PrimaryValue>(p => p.Value == $"U_{i}").Count, Is.EqualTo(1));
            Assert.That(LazyClient.Select<PrimaryValue>(p => p.Value == $"F_{i}").Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void SelectObject()
    {
        ClientTest.AddSimpleTables();
        List <Simple_Table> simpleTables = LazyClient.Select<Simple_Table>().ToList();
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
    public void SelectObjectOnSchema()
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
            Assert.That(uValues[0].Value, Is.EqualTo($"U_{i - 1}"));
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
    public void SelectDynamicOnSchema()
    {
        ClientTest.AddSchemaRows();
        
        List<dynamic> values = LazyClient.Select<dynamic>("lazys", "WithoutKeys").ToList();

        for (int i = 0; i < values.Count; i++)
        {
            Assert.That(values[i].Age, Is.EqualTo(i));
            Assert.That(values[i].Name, Is.EqualTo($"N_{i}"));
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
        foreach (dynamic simpleTable in LazyClient.Select<dynamic>("simple_table", "(user_id > 4 AND user_id <= 10) OR username = @p20".Bind("@p20", "P20")))
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

    [Test]
    public void SelectHierarchy()
    {
        LazyClient.Delete<HierarchyTable>();

 
        for (int i = 0; i < 10; i++)
        {
            HierarchyTable root = new() {Name = "?"};
            root.Insert();
            root.Name = root.Id.ToString();
            root.Update();
            for (int j = 0; j < 5; j++)
            {
                HierarchyTable sub1 = new() { Name = "?", ParentId = root.Id };
                sub1.Insert();
                sub1.Name = $"{root.Id}/{sub1.Id}";
                sub1.Update();

                for (int k = 0; k < 7; k++)
                {
                    HierarchyTable sub2 = new() { Name = "?", ParentId = sub1.Id};
                    sub2.Insert();
                    sub2.Name = $"{root.Id}/{sub1.Id}/{sub2.Id}";
                    sub2.Update();

                    for (int l = 0; l < 5; l++)
                    {
                        HierarchyTable sub3 = new() { Name = "?", ParentId = sub2.Id};
                        sub3.Insert();
                        sub3.Name = $"{root.Id}/{sub1.Id}/{sub2.Id}/{sub3.Id}";
                        sub3.Update();
                    }

                }
            }
        }

        List<HierarchyTable> roots = LazyClient.Select<HierarchyTable>(h=>h.ParentId == null).ToList();
        Assert.That(roots.Count, Is.EqualTo(10));

        foreach (HierarchyTable t in roots)
        {
            Assert.That(t.ParentId, Is.Null);
            Assert.That(t.Children.Count, Is.EqualTo(5));
            Assert.That(t.Name, Is.EqualTo(t.Id.ToString()));

            foreach (HierarchyTable t1 in t.Children)
            {
                Assert.That(t1.ParentId, Is.EqualTo(t.Id));
                Assert.That(t1.Children.Count, Is.EqualTo(7));
                Assert.That(t1.Name, Is.EqualTo($"{t.Id}/{t1.Id}"));

                foreach (HierarchyTable t2 in t1.Children)
                {
                    Assert.That(t2.ParentId, Is.EqualTo(t1.Id));
                    Assert.That(t2.Children.Count, Is.EqualTo(5));
                    Assert.That(t2.Name, Is.EqualTo($"{t.Id}/{t1.Id}/{t2.Id}"));

                    foreach (HierarchyTable t3 in t2.Children)
                    {
                        Assert.That(t3.ParentId, Is.EqualTo(t2.Id));
                        Assert.That(t3.Children.Count, Is.EqualTo(0));
                        Assert.That(t3.Name, Is.EqualTo($"{t.Id}/{t1.Id}/{t2.Id}/{t3.Id}"));
                    }
                }
            }
        }
    }

    [Test]
    public void SelectHierarchyOnSchema()
    {
        LazyClient.Delete<HierarchySchema>();


        for (int i = 0; i < 10; i++)
        {
            HierarchySchema root = new() { Name = "?" };
            root.Insert();
            root.Name = root.Id.ToString();
            root.Update();
            for (int j = 0; j < 5; j++)
            {
                HierarchySchema sub1 = new() { Name = "?", ParentId = root.Id };
                sub1.Insert();
                sub1.Name = $"{root.Id}/{sub1.Id}";
                sub1.Update();

                for (int k = 0; k < 7; k++)
                {
                    HierarchySchema sub2 = new() { Name = "?", ParentId = sub1.Id };
                    sub2.Insert();
                    sub2.Name = $"{root.Id}/{sub1.Id}/{sub2.Id}";
                    sub2.Update();

                    for (int l = 0; l < 5; l++)
                    {
                        HierarchySchema sub3 = new() { Name = "?", ParentId = sub2.Id };
                        sub3.Insert();
                        sub3.Name = $"{root.Id}/{sub1.Id}/{sub2.Id}/{sub3.Id}";
                        sub3.Update();
                    }

                }
            }
        }

        List<HierarchySchema> roots = LazyClient.Select<HierarchySchema>(h => h.ParentId == null).ToList();
        Assert.That(roots.Count, Is.EqualTo(10));

        foreach (HierarchySchema t in roots)
        {
            Assert.That(t.ParentId, Is.Null);
            Assert.That(t.Children.Count, Is.EqualTo(5));
            Assert.That(t.Name, Is.EqualTo(t.Id.ToString()));

            foreach (HierarchySchema t1 in t.Children)
            {
                Assert.That(t1.ParentId, Is.EqualTo(t.Id));
                Assert.That(t1.Children.Count, Is.EqualTo(7));
                Assert.That(t1.Name, Is.EqualTo($"{t.Id}/{t1.Id}"));

                foreach (HierarchySchema t2 in t1.Children)
                {
                    Assert.That(t2.ParentId, Is.EqualTo(t1.Id));
                    Assert.That(t2.Children.Count, Is.EqualTo(5));
                    Assert.That(t2.Name, Is.EqualTo($"{t.Id}/{t1.Id}/{t2.Id}"));

                    foreach (HierarchySchema t3 in t2.Children)
                    {
                        Assert.That(t3.ParentId, Is.EqualTo(t2.Id));
                        Assert.That(t3.Children.Count, Is.EqualTo(0));
                        Assert.That(t3.Name, Is.EqualTo($"{t.Id}/{t1.Id}/{t2.Id}/{t3.Id}"));
                    }
                }
            }
        }
    }

    [Test]
    public void SelectGroupBy()
    {
        ClientTest.CleanTables();

        new SimpleTable {Username = "U1", Password = "P1"}.Insert();
        new SimpleTable {Username = "U1", Password = "P1"}.Insert();
        new SimpleTable {Username = "U1", Password = "P1"}.Insert();
        new SimpleTable {Username = "U1", Password = "P2"}.Insert();
        new SimpleTable {Username = "U1", Password = "P2"}.Insert();
        new SimpleTable {Username = "U1", Password = "P2"}.Insert();
        new SimpleTable {Username = "U1", Password = "P3" }.Insert();
        new SimpleTable {Username = "U1", Password = "P3" }.Insert();
        new SimpleTable {Username = "U1", Password = "P3" }.Insert();

        new SimpleTable { Username = "U2", Password = "P1" }.Insert();
        new SimpleTable { Username = "U2", Password = "P1" }.Insert();
        new SimpleTable { Username = "U2", Password = "P1" }.Insert();
        new SimpleTable { Username = "U2", Password = "P2" }.Insert();
        new SimpleTable { Username = "U2", Password = "P2" }.Insert();
        new SimpleTable { Username = "U2", Password = "P2" }.Insert();
        new SimpleTable { Username = "U2", Password = "P3" }.Insert();
        new SimpleTable { Username = "U2", Password = "P3" }.Insert();
        new SimpleTable { Username = "U2", Password = "P3" }.Insert();

        new SimpleTable { Username = "U3", Password = "P1" }.Insert();
        new SimpleTable { Username = "U3", Password = "P1" }.Insert();
        new SimpleTable { Username = "U3", Password = "P1" }.Insert();
        new SimpleTable { Username = "U3", Password = "P2" }.Insert();
        new SimpleTable { Username = "U3", Password = "P2" }.Insert();
        new SimpleTable { Username = "U3", Password = "P2" }.Insert();
        new SimpleTable { Username = "U3", Password = "P3" }.Insert();
        new SimpleTable { Username = "U3", Password = "P3" }.Insert();
        new SimpleTable { Username = "U3", Password = "P3" }.Insert();

        List<SimpleTable> values = LazyClient.Select<SimpleTable>().Columns(s=> s.Id.Max().As("user_id"), s=> s.Username, s=>s.Password).GroupBy(s => s.Username, s=>s.Password).OrderByDesc(s=>s.Password).ToList();

        Assert.That(values.Count, Is.EqualTo(9));

        Assert.That(values[0].Id, Is.EqualTo(8));
        Assert.That(values[0].Username, Is.EqualTo("U1"));
        Assert.That(values[0].Password, Is.EqualTo("P3"));
        Assert.That(values[1].Id, Is.EqualTo(17));
        Assert.That(values[1].Username, Is.EqualTo("U2"));
        Assert.That(values[1].Password, Is.EqualTo("P3"));
        Assert.That(values[2].Id, Is.EqualTo(26));
        Assert.That(values[2].Username, Is.EqualTo("U3"));
        Assert.That(values[2].Password, Is.EqualTo("P3"));

        Assert.That(values[3].Id, Is.EqualTo(5));
        Assert.That(values[3].Username, Is.EqualTo("U1"));
        Assert.That(values[3].Password, Is.EqualTo("P2"));
        Assert.That(values[4].Id, Is.EqualTo(14));
        Assert.That(values[4].Username, Is.EqualTo("U2"));
        Assert.That(values[4].Password, Is.EqualTo("P2"));
        Assert.That(values[5].Id, Is.EqualTo(23));
        Assert.That(values[5].Username, Is.EqualTo("U3"));
        Assert.That(values[5].Password, Is.EqualTo("P2"));


        Assert.That(values[6].Id, Is.EqualTo(2));
        Assert.That(values[6].Username, Is.EqualTo("U1"));
        Assert.That(values[6].Password, Is.EqualTo("P1"));
        Assert.That(values[7].Id, Is.EqualTo(11));
        Assert.That(values[7].Username, Is.EqualTo("U2"));
        Assert.That(values[7].Password, Is.EqualTo("P1"));
        Assert.That(values[8].Id, Is.EqualTo(20));

        Assert.That(values[8].Username, Is.EqualTo("U3"));
        Assert.That(values[8].Password, Is.EqualTo("P1"));
    }

    [Test]
    public void SelectGroupByStringColumn()
    {
        ClientTest.CleanTables();

        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();

        new SimpleTable { Username = "U2", Password = "P1" }.Insert();
        new SimpleTable { Username = "U2", Password = "P1" }.Insert();
        new SimpleTable { Username = "U2", Password = "P1" }.Insert();
        new SimpleTable { Username = "U2", Password = "P2" }.Insert();
        new SimpleTable { Username = "U2", Password = "P2" }.Insert();
        new SimpleTable { Username = "U2", Password = "P2" }.Insert();
        new SimpleTable { Username = "U2", Password = "P3" }.Insert();
        new SimpleTable { Username = "U2", Password = "P3" }.Insert();
        new SimpleTable { Username = "U2", Password = "P3" }.Insert();

        new SimpleTable { Username = "U3", Password = "P1" }.Insert();
        new SimpleTable { Username = "U3", Password = "P1" }.Insert();
        new SimpleTable { Username = "U3", Password = "P1" }.Insert();
        new SimpleTable { Username = "U3", Password = "P2" }.Insert();
        new SimpleTable { Username = "U3", Password = "P2" }.Insert();
        new SimpleTable { Username = "U3", Password = "P2" }.Insert();
        new SimpleTable { Username = "U3", Password = "P3" }.Insert();
        new SimpleTable { Username = "U3", Password = "P3" }.Insert();
        new SimpleTable { Username = "U3", Password = "P3" }.Insert();

        List<SimpleTable> values = LazyClient.Select<SimpleTable>().Columns(s => s.Id.Max().As("user_id"), s => s.Username, s => s.Password).GroupBy("Username", "Password").OrderByDesc(s => s.Password).ToList();

        Assert.That(values.Count, Is.EqualTo(9));

        Assert.That(values[0].Id, Is.EqualTo(8));
        Assert.That(values[0].Username, Is.EqualTo("U1"));
        Assert.That(values[0].Password, Is.EqualTo("P3"));
        Assert.That(values[1].Id, Is.EqualTo(17));
        Assert.That(values[1].Username, Is.EqualTo("U2"));
        Assert.That(values[1].Password, Is.EqualTo("P3"));
        Assert.That(values[2].Id, Is.EqualTo(26));
        Assert.That(values[2].Username, Is.EqualTo("U3"));
        Assert.That(values[2].Password, Is.EqualTo("P3"));

        Assert.That(values[3].Id, Is.EqualTo(5));
        Assert.That(values[3].Username, Is.EqualTo("U1"));
        Assert.That(values[3].Password, Is.EqualTo("P2"));
        Assert.That(values[4].Id, Is.EqualTo(14));
        Assert.That(values[4].Username, Is.EqualTo("U2"));
        Assert.That(values[4].Password, Is.EqualTo("P2"));
        Assert.That(values[5].Id, Is.EqualTo(23));
        Assert.That(values[5].Username, Is.EqualTo("U3"));
        Assert.That(values[5].Password, Is.EqualTo("P2"));


        Assert.That(values[6].Id, Is.EqualTo(2));
        Assert.That(values[6].Username, Is.EqualTo("U1"));
        Assert.That(values[6].Password, Is.EqualTo("P1"));
        Assert.That(values[7].Id, Is.EqualTo(11));
        Assert.That(values[7].Username, Is.EqualTo("U2"));
        Assert.That(values[7].Password, Is.EqualTo("P1"));
        Assert.That(values[8].Id, Is.EqualTo(20));

        Assert.That(values[8].Username, Is.EqualTo("U3"));
        Assert.That(values[8].Password, Is.EqualTo("P1"));
    }

    [Test]
    public void SelectCount()
    {
        ClientTest.CleanTables();

        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();

        Assert.That(LazyClient.Select<SimpleTable>(s=>s.Username == "U1" && s.Password == "P2").Count, Is.EqualTo(3));
    }

    [Test]
    public void SelectCountObject()
    {
        ClientTest.CleanTables();

        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();

        Assert.That(LazyClient.Select<Simple_Table>(s => s.Username == "U1" && s.Password == "P2").Count, Is.EqualTo(3));
    }

    [Test]
    public void SelectCountDynamic()
    {
        ClientTest.CleanTables();

        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P1" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P2" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();
        new SimpleTable { Username = "U1", Password = "P3" }.Insert();

        Assert.That(LazyClient.Select<dynamic>("simple_table", "Username = @Username AND password = @Password".Bind("@Username", "U1").Bind("@Password", "P2")).Count, Is.EqualTo(3));
    }
}