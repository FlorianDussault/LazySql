using LazySql;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Bulk Insert")]
public class BulkInsertTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void BulkLazyObjects()
    {
        ClientTest.CleanTables();
        
        List<SimpleTable> simpleTables = new();
        for (int i = 0; i < 100; i++) simpleTables.Add(new SimpleTable() {Username = "U" + i});
        LazyClient.BulkInsert(simpleTables);

        int childId = 0;
        List<ChildTable> childTables = new();
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>())
        {
            for (int i = 0; i < 200; i++)
            {
                childTables.Add(new ChildTable{Id = childId++, ParentId = simpleTable.Id});
            }
        }
        LazyClient.BulkInsert(childTables);

        childTables.Clear();

        simpleTables = LazyClient.Select<SimpleTable>().ToList();

        Assert.That(simpleTables.Count, Is.EqualTo(100));
        foreach (SimpleTable simpleTable in simpleTables)
        {
            Assert.That(simpleTable.ChildTables.Count, Is.EqualTo(200));
        }

    }

    [Test]
    public void BulkLazyObjectsOnSchema()
    {
        ClientTest.CleanTables();
        List<PrimaryValue> primaryValues = new();
        for (int i = 0; i < 100; i++) primaryValues.Add(new PrimaryValue() { Value = "U" + i });
        LazyClient.BulkInsert(primaryValues);
        primaryValues = LazyClient.Select<PrimaryValue>().ToList();
        Assert.That(primaryValues.Count, Is.EqualTo(100));
        foreach (PrimaryValue primaryValue in primaryValues)
            Assert.That(primaryValue.Value, Is.EqualTo($"U{primaryValue.Key - 1}"));
    }

    [Test]
    public void BulkObjects()
    {
        ClientTest.CleanTables();

        List<Simple_Table> simpleTables = new();
        for (int i = 0; i < 100; i++) simpleTables.Add(new Simple_Table() { Username = "U" + i });
        LazyClient.BulkInsert(simpleTables);

        simpleTables = LazyClient.Select<Simple_Table>().ToList();
        Assert.That(simpleTables.Count, Is.EqualTo(100));
    }

    [Test]
    public void BulkObjectsOnSchema()
    {
        ClientTest.CleanTables();

        List<TablePrimary> tablePrimaries = new();
        for (int i = 0; i < 100; i++) tablePrimaries.Add(new TablePrimary() { Value = "U" + i });
        LazyClient.BulkInsert("lazys", "tablePrimary", tablePrimaries);

        tablePrimaries = LazyClient.Select<TablePrimary>("lazys", "tablePrimary").ToList();

        foreach (TablePrimary primaryValue in tablePrimaries)
            Assert.That(primaryValue.Value, Is.EqualTo($"U{primaryValue.Id - 1}"));
    }

    [Test]
    public void BulkDynamic()
    {
        ClientTest.CleanTables();

        List<dynamic> simpleTables = new();
        for (int i = 0; i < 600; i++) simpleTables.Add(new {aNotInSql = 1, Username = "U" + i, NotInSql = i });
        LazyClient.BulkInsert("simple_table", simpleTables);

        simpleTables = LazyClient.Select<dynamic>("simple_table").ToList();
        Assert.That(simpleTables.Count, Is.EqualTo(600));
    }

    [Test]
    public void BulkDynamicOnSchema()
    {
        ClientTest.CleanTables();

        List<dynamic> primaries = new();
        for (int i = 0; i < 600; i++) primaries.Add(new { Value = "U" + i });
        LazyClient.BulkInsert("lazys", "tablePrimary", primaries);

        primaries = LazyClient.Select<dynamic>("lazys", "tablePrimary").ToList();
        Assert.That(primaries.Count, Is.EqualTo(600));
        Assert.IsNotEmpty(primaries.Where(p=>p.value == "U59"));
        Assert.IsNotEmpty(primaries.Where(p=>p.value == "U101"));
        Assert.IsNotEmpty(primaries.Where(p=>p.value == "U3"));
    }

}