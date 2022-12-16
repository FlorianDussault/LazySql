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

    [Test]
    public void ExecuteSelectLazy()
    {
        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>();

        List<SimpleTable> values = LazyClient.ExecuteSelect<SimpleTable>("SELECT user_id, Username  FROM simple_table WHERE user_id >= 5 AND user_id < 10 ORDER BY user_id ASC").ToList();
        Assert.That(values.Count, Is.EqualTo(5));
        Assert.That(values[0].Id, Is.EqualTo(5));
        Assert.That(values[1].Id, Is.EqualTo(6));
        Assert.That(values[2].Id, Is.EqualTo(7));
        Assert.That(values[3].Id, Is.EqualTo(8));
        Assert.That(values[4].Id, Is.EqualTo(9));
    }

    [Test]
    public void ExecuteSelectObject()
    {
        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>();

        List<Simple_Table> values = LazyClient.ExecuteSelect<Simple_Table>("SELECT user_id, Username  FROM simple_table WHERE user_id >= 5 AND user_id < 10 ORDER BY user_id ASC").ToList();
        Assert.That(values.Count, Is.EqualTo(5));
        Assert.That(values[0].User_Id, Is.EqualTo(5));
        Assert.That(values[1].User_Id, Is.EqualTo(6));
        Assert.That(values[2].User_Id, Is.EqualTo(7));
        Assert.That(values[3].User_Id, Is.EqualTo(8));
        Assert.That(values[4].User_Id, Is.EqualTo(9));
    }

    [Test]
    public void ExecuteSelectDynamic()
    {
        ClientTest.AddSimpleTables();
        LazyClient.Delete<ChildTable>();

        List<dynamic> values = LazyClient.ExecuteSelect<dynamic>("SELECT user_id as UID, Username  FROM simple_table WHERE user_id >= 5 AND user_id < 10 ORDER BY user_id ASC").ToList();
        Assert.That(values.Count, Is.EqualTo(5));
        Assert.That(values[0].UID, Is.EqualTo(5));
        Assert.That(values[1].UID, Is.EqualTo(6));
        Assert.That(values[2].UID, Is.EqualTo(7));
        Assert.That(values[3].UID, Is.EqualTo(8));
        Assert.That(values[4].UID, Is.EqualTo(9));
    }

}