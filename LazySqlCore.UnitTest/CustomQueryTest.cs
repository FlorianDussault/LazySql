using LazySql;
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

    [Test]
    public void ExecuteNonQuery()
    {
        ClientTest.AddSimpleTables();

        Assert.That(LazyClient.ExecuteNonQuery("DELETE FROM child_table"), Is.EqualTo(400));
        Assert.That(LazyClient.ExecuteNonQuery("DELETE FROM simple_table"), Is.EqualTo(20));
        
        ClientTest.CleanSimpleTable();

        for (int i = 0; i < 100; i++)
        {
            new SimpleTable() {Username = "U"}.Insert();
        }

        Assert.That(LazyClient.ExecuteNonQuery("DELETE FROM simple_table WHERE user_id <= @IdMax".Bind("@IdMax", SqlType.Int, 50)), Is.EqualTo(50));
    }

    [Test]
    public void ExecuteScalar()
    {
        Assert.That(LazyClient.ExecuteScalar<string>("SELECT CONCAT('Hello', ' ', @WorldVariable)".Bind("@WorldVariable", SqlType.NVarChar, "WORLD")), Is.EqualTo("Hello WORLD"));
        Assert.That(LazyClient.ExecuteScalar<int>("SELECT 9876"), Is.EqualTo(9876));
    }
}