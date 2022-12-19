using LazySql;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Delete")]
public class DeleteTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void Delete()
    {
        ClientTest.AddSimpleTables();

        LazyClient.Delete<ChildTable>();
        LazyClient.Delete<SimpleTable>();

        Assert.IsEmpty(LazyClient.Select<ChildTable>());
        Assert.IsEmpty(LazyClient.Select<SimpleTable>());

        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>();
        LazyClient.Delete<Simple_Table>();
        Assert.IsEmpty(LazyClient.Select<SimpleTable>());

        ClientTest.AddSimpleTables();
        LazyClient.Delete<object>("child_table");
        LazyClient.Delete<object>("simple_table");
        Assert.IsEmpty(LazyClient.Select<ChildTable>());
        Assert.IsEmpty(LazyClient.Select<SimpleTable>());

        ClientTest.AddSimpleTables();
        Assert.That(LazyClient.Delete<object>("child_table", new SqlQuery("Id = @Id2 OR Id = @Id3").Bind("@Id2", 2).Bind("@Id3", 3)), Is.EqualTo(2));
        Assert.IsEmpty(LazyClient.Select<ChildTable>(c=>c.Id == 2  || c.Id == 3));
        Assert.IsNotEmpty(LazyClient.Select<ChildTable>());

        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>(null, SqlQuery.Empty);
        Assert.That(LazyClient.Delete<Simple_Table>(null, c => c.User_Id == 2 || c.User_Id == 3), Is.EqualTo(2));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(c => c.Id == 2 || c.Id == 3));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>());
    }

    [Test]
    public void DeleteLazy()
    {
        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>();

        SimpleTable simpleTable = LazyClient.Select<SimpleTable>(s => s.Id == 2).First();
        Assert.That(simpleTable.Delete(), Is.EqualTo(1));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(c => c.Id == 2));
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>());
    }

    [Test]
    public void DeleteLazyOnSchema()
    {
        ClientTest.AddSimpleTables();
        LazyClient.Delete<PrimaryValue>();
        Assert.IsEmpty(LazyClient.Select<PrimaryValue>());

        // Add values

        for (int i = 0; i < 100; i++)
            new PrimaryValue() {Value = $"U_{i}"}.Insert();

        Assert.That(LazyClient.Select<PrimaryValue>().Count, Is.EqualTo(100));
        Assert.IsNotEmpty(LazyClient.Select<PrimaryValue>(p=>p.Value == "U_59" ));

        LazyClient.Delete<PrimaryValue>(p => p.Value == "U_59");
        Assert.IsEmpty(LazyClient.Select<PrimaryValue>(p => p.Value == "U_59"));
        Assert.IsNotEmpty(LazyClient.Select<PrimaryValue>());

        LazyClient.Delete<PrimaryValue>("lazys", "tablePrimary", new SqlQuery("value = 'U_58'"));
        Assert.IsEmpty(LazyClient.Select<PrimaryValue>(p => p.Value == "U_58"));

        Assert.IsNotEmpty(LazyClient.Select<PrimaryValue>());
        LazyClient.Delete<PrimaryValue>();
        Assert.IsEmpty(LazyClient.Select<PrimaryValue>());
    }

    [Test]
    public void DeleteList()
    {
        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>();

        List<SimpleTable> values = LazyClient.Select<SimpleTable>(s=>s.Id >= 2 && s.Id <= 10).ToList();
        Assert.That(values.Delete(), Is.EqualTo(9));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => s.Id >= 2 && s.Id <= 10));
    }

    [Test]
    [Ignore("TODO")]
    public void DeleteListOnSchema()
    {
        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>();

        List<SimpleTable> values = LazyClient.Select<SimpleTable>(s => s.Id >= 2 && s.Id <= 10).ToList();
        Assert.That(values.Delete(), Is.EqualTo(9));
        Assert.IsEmpty(LazyClient.Select<SimpleTable>(s => s.Id >= 2 && s.Id <= 10));
    }
}