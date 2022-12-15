using LazySql;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "DataLive")]
public class DataLiveTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();


    }



    [Test]
    public void Initialize()
    {
        ClientTest.AddSimpleTables();
        int count = LazyClient.Select<SimpleTable>().Count();
        DataLive<SimpleTable> dataLive = new(true);
        Assert.That(dataLive.Count, Is.EqualTo(count));
        dataLive = new();
        Assert.That(dataLive.Count, Is.EqualTo(0));
    }

    [Test]
    public void InitializeWithoutPk()
    {
        ClientTest.AddSchemaRows();
        int count = LazyClient.Select<WithoutKeys>().Count();
        DataLive<WithoutKeys> dataLive = new(true);
        Assert.That(dataLive.Count, Is.EqualTo(count));
        dataLive = new();
        Assert.That(dataLive.Count, Is.EqualTo(0));
    }

    [Test]
    public void Load()
    {
        ClientTest.AddSimpleTables();
        DataLive<SimpleTable> dataLive = new();
        dataLive.Load();
        Assert.That(dataLive.Count, Is.EqualTo(LazyClient.Select<SimpleTable>().Count()));
    }

    [Test]
    public void LoadWithoutPk()
    {
        ClientTest.AddSchemaRows();
        DataLive<WithoutKeys> dataLive = new();
        dataLive.Load();
        Assert.That(dataLive.Count, Is.EqualTo(LazyClient.Select<WithoutKeys>().Count()));
    }

    

    [Test]
    public void LoadAdditional()
    {
        ClientTest.AddSimpleTables();
        DataLive<SimpleTable> dataLive = new();
        Assert.That(dataLive.Count, Is.EqualTo(0));
        dataLive.LoadAdditional();
    }

    [Test]
    public void Add()
    {
        ClientTest.CleanTables();
        DataLive<SimpleTable> dataLive = new();
        Assert.That(LazyClient.Select<SimpleTable>().Count(), Is.EqualTo(0));
        Assert.That(dataLive.Add(new SimpleTable {Username = "Hello"}), Is.EqualTo(1));
        Assert.That(LazyClient.Select<SimpleTable>().Count(), Is.EqualTo(1));
        Assert.That(dataLive.Count, Is.EqualTo(1));
        Assert.That(dataLive.Add(new SimpleTable { Username = "Hello2" }), Is.EqualTo(1));
        Assert.That(LazyClient.Select<SimpleTable>().Count(), Is.EqualTo(2));
        Assert.That(dataLive.Count, Is.EqualTo(2));
    }

    [Test]
    public void AddRange()
    {
        ClientTest.AddSimpleTables();

        DataLive<SimpleTable> dataLive = new(true);

        int count = LazyClient.Select<SimpleTable>().Count();
        List<SimpleTable> simpleTables = new();
        for (int i = 0; i < 10; i++)
        {
            simpleTables.Add(new SimpleTable {Id = -1, Username = $"N{i}"});
        }

        Assert.That(dataLive.AddRange(simpleTables), Is.EqualTo(10));

        Assert.That(LazyClient.Select<SimpleTable>().Count(), Is.EqualTo(count + 10));
        Assert.That(dataLive.Count, Is.EqualTo(count + 10));

        int j = 0;
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s=>LzFunctions.Like(s.Username, "N%")).OrderByAsc(s=>s.Id))
        {
            int val = int.Parse(simpleTable.Username.Substring(1));
            Assert.That(val, Is.EqualTo(j++));
        }

    }

    [Test]
    public void Insert()
    {
        ClientTest.AddSimpleTables();

        DataLive<SimpleTable> dataLive = new(true);

        int count = LazyClient.Select<SimpleTable>().Count();

        Assert.That(dataLive.Insert(2, new SimpleTable() {Id = -1, Username = "XXX"}), Is.EqualTo(1));
        Assert.That(dataLive[2].Username, Is.EqualTo("XXX"));
        Assert.That(LazyClient.Select<SimpleTable>(s=>s.Username == "XXX").Count(), Is.EqualTo(1));

    }

    [Test]
    public void InsertRange()
    {
        ClientTest.AddSimpleTables();

        DataLive<SimpleTable> dataLive = new(true);

        int count = LazyClient.Select<SimpleTable>().Count();
        List<SimpleTable> simpleTables = new();
        for (int i = 0; i < 10; i++)
        {
            simpleTables.Add(new SimpleTable { Id = -1, Username = $"N{i}" });
        }

        Assert.That(dataLive.InsertRange(3, simpleTables), Is.EqualTo(10));

        Assert.That(LazyClient.Select<SimpleTable>().Count(), Is.EqualTo(count + 10));
        Assert.That(dataLive.Count, Is.EqualTo(count + 10));

        int j = 0;
        List<SimpleTable> list = LazyClient.Select<SimpleTable>(s => LzFunctions.Like(s.Username, "N%"))
                     .OrderByAsc(s => s.Id).ToList();
        for (int index = 0; index < list.Count; index++)
        {
            SimpleTable simpleTable = list[index];
            int val = int.Parse(simpleTable.Username.Substring(1));
            Assert.That(val, Is.EqualTo(j++));
        }
    }

    [Test]
    public void Remove()
    {
        throw new NotImplementedException();

    }

    [Test]
    public void RemoveAll()
    {
        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>();

        DataLive<SimpleTable> dataLive = new(s=>LzFunctions.Like(s.Username, "%1%"));
        int count = LazyClient.Select<SimpleTable>(s => LzFunctions.Like(s.Username, "%1%")).Count();
        Assert.That(count, Is.GreaterThan(0));
        Assert.That(dataLive.RemoveAll(s => s.Username.Contains("1")), Is.EqualTo(count));
        Assert.That(dataLive.Count(s => s.Username.Contains("1")), Is.EqualTo(0));
        Assert.That(LazyClient.Select<SimpleTable>(s => LzFunctions.Like(s.Username, "%1%")).Count(), Is.EqualTo(0));



    }

    [Test]
    public void RemoveAt()
    {
        throw new NotImplementedException();

    }

    [Test]
    public void RemoveRange()
    {
        throw new NotImplementedException();

    }
}