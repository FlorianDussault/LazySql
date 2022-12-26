using LazySql;
using LazySqlCore.UnitTest.Tables;
namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Lambda Expression")]
public class LambdaTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void GetSimple()
    {
        ClientTest.AddSimpleTables();

        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id < 10))
        {
            Assert.Less(simpleTable.Id, 10);
        }
    }

    [Test]
    public void GetSimple2()
    {
        ClientTest.AddSimpleTables();

        List<int> id = new() {10};
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id < id[0]))
        {
            Assert.Less(simpleTable.Id, 10);
        }
    }

    [Test]
    public void Linq()
    {
        ClientTest.AddSimpleTables();

        int valInt = 1;
        foreach (SimpleTable simpleTable in LazyClient.Select<SimpleTable>(s => s.Id < 9 + valInt))
        {
            Assert.Less(simpleTable.Id, 10);
        }

        {
            string valString = "U";

            List<SimpleTable> simpleTables =
                LazyClient.Select<SimpleTable>(s => s.Username == valString + "10").ToList();

            Assert.That(simpleTables.Count, Is.EqualTo(1));
            Assert.That(simpleTables[0].Username, Is.EqualTo("U10"));
        }
    }

    [Test]
    public void VariableName()
    {
        ClientTest.AddSimpleTables();

        // int Id = 5;
        // List<SimpleTable> values = LazyClient.Select<SimpleTable>(s => s.Id == Id).ToList();
        // Assert.That(values.Count, Is.EqualTo(1));
        // Assert.That(values[0].Id, Is.EqualTo(5));

        SimpleTable simpleTable = new() {Id = 6};
        List<SimpleTable> values = LazyClient.Select<SimpleTable>(s => s.Id == simpleTable.Id).ToList();
        Assert.That(values.Count, Is.EqualTo(1));
        Assert.That(values[0].Id, Is.EqualTo(6));
        
    }
}