using LazySql.Engine.Client;
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

    private void ClearTables()
    {
        // Clear Table
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);
        // Add values
        Assert.IsEmpty(LazyClient.Select<SimpleTable>());
    }

    [Test]
    public void BulkLazyObjects()
    {
        ClearTables();

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
    public void BulkObjects()
    {
        ClearTables();

        List<Simple_Table> simpleTables = new();
        for (int i = 0; i < 100; i++) simpleTables.Add(new Simple_Table() { Username = "U" + i });
        LazyClient.BulkInsert(simpleTables);

        simpleTables = LazyClient.Select<Simple_Table>().ToList();
        Assert.That(simpleTables.Count, Is.EqualTo(100));
    }

    [Test]
    public void BulkDynamic()
    {
        ClearTables();

        List<dynamic> simpleTables = new();
        for (int i = 0; i < 600; i++) simpleTables.Add(new {aNotInSql = 1, Username = "U" + i, NotInSql = i });
        LazyClient.BulkInsert("simple_table", simpleTables);

        simpleTables = LazyClient.Select<dynamic>("simple_table").ToList();
        Assert.That(simpleTables.Count, Is.EqualTo(600));
    }

}