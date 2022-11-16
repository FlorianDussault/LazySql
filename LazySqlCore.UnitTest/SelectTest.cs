using LazySql.Engine;
using LazySql.Engine.Client;
using LazySqlCore.UnitTest.Tables;

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
            SimpleTable st = new SimpleTable()
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
        Assert.That(LazyClient.Select<SimpleTable>().ToList().Count(), Is.EqualTo(COUNT_SIMPLE_TABLE));
        Assert.That(LazyClient.Select<ChildTable>().ToList().Count(), Is.EqualTo(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE));
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

        Assert.That(LazyClient.Select<Simple_Table>().ToList().Count(), Is.EqualTo(20));
    }

    [Test]
    public void GetWithArgs()
    {
        AddSimpleTables();

        List<int> allowedIds = new List<int> { 5, 6, 7, 8, 9, 10, 20 };

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>().Where(i=>(i.Id > 4 && i.Id <= 10) || i.Username == "P20"))
        {
            Assert.IsTrue(allowedIds.Contains(simpleTable.Id));
        }
    }

    [Test]
    public void GetOrderBy()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable{Username = rand.Next(0,50).ToString("00")}.Insert();
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
    public void GetTop()
    {
        LazyClient.Truncate<SubChildTable>(true);
        LazyClient.Delete<ChildTable>();
        LazyClient.Truncate<SimpleTable>(true);

        Random rand = new(DateTime.Now.Millisecond);
        for (int i = 0; i < 1000; i++)
        {
            new SimpleTable { Username = rand.Next(0, 50).ToString("00") }.Insert();
        }

        List<SimpleTable> list = LazyClient.Select<SimpleTable>().OrderByDesc(s => s.Id).Top(2).ToList();
        Assert.That(2, Is.EqualTo(list.Count));
        Assert.That(1000, Is.EqualTo(list[0].Id));
        Assert.That(999, Is.EqualTo(list[1].Id));
    }
        
}