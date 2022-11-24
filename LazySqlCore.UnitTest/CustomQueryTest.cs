using LazySql.Engine;
using LazySql.Engine.Client;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Enums;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;
[TestFixture(TestName = "Custom Queries")]
public class CustomQueryTest
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
        Assert.That(COUNT_SIMPLE_TABLE == LazyClient.Select<SimpleTable>().ToList().Count());
        Assert.That(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE == LazyClient.Select<ChildTable>().ToList().Count());
    }

    [Test]
    public void ExecuteNonQuery()
    {
        AddSimpleTables();

        Assert.That(LazyClient.ExecuteNonQuery("DELETE FROM child_table"), Is.EqualTo(400));
        Assert.That(LazyClient.ExecuteNonQuery("DELETE FROM simple_table"), Is.EqualTo(20));

        LazyClient.Truncate<SimpleTable>(true);

        for (int i = 0; i < 100; i++)
        {
            new SimpleTable() {Username = "U"}.Insert();
        }

        Assert.That(LazyClient.ExecuteNonQuery("DELETE FROM simple_table WHERE user_id <= @IdMax", new SqlArguments().Add("@IdMax", SqlType.Int, 50)), Is.EqualTo(50));
    }

    [Test]
    public void ExecuteScalar()
    {
        Assert.That(LazyClient.ExecuteScalar<string>("SELECT CONCAT('Hello', ' ', @WorldVariable)", new SqlArguments().Add("@WorldVariable", SqlType.NVarChar, "WORLD")), Is.EqualTo("Hello WORLD"));
        Assert.That(LazyClient.ExecuteScalar<int>("SELECT 9876"), Is.EqualTo(9876));
    }
}