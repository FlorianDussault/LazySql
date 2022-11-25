using LazySql.Engine;
using LazySql.Engine.Client;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Truncate")]
public class TruncateTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

   

    [Test]
    public void Truncate()
    {
        ClientTest.CleanTables();

        new ExtendedTable{Value = 2, Key = "B"}.Insert();
        Assert.IsNotEmpty(LazyClient.Select<ExtendedTable>());
        LazyClient.Truncate<ExtendedTable>();
        Assert.IsEmpty(LazyClient.Select<ExtendedTable>());

        new ExtendedTable { Value = 2, Key = "B" }.Insert();
        Assert.IsNotEmpty(LazyClient.Select<ExtendedTable>());
        LazyClient.Truncate<Extended_Table>();
        Assert.IsEmpty(LazyClient.Select<ExtendedTable>());

        new ExtendedTable { Value = 2, Key = "B" }.Insert();
        Assert.IsNotEmpty(LazyClient.Select<ExtendedTable>());
        LazyClient.Truncate("extended_table");
        Assert.IsEmpty(LazyClient.Select<ExtendedTable>());
    }
}