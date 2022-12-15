using LazySql;
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

        new ExtendedTable {Value = 2, Key = "B"}.Insert();
        Assert.IsNotEmpty(LazyClient.Select<ExtendedTable>());
        LazyClient.Truncate<ExtendedTable>();
        Assert.IsEmpty(LazyClient.Select<ExtendedTable>());

        new ExtendedTable {Value = 2, Key = "B"}.Insert();
        Assert.IsNotEmpty(LazyClient.Select<ExtendedTable>());
        LazyClient.Truncate<Extended_Table>();
        Assert.IsEmpty(LazyClient.Select<ExtendedTable>());

        new ExtendedTable {Value = 2, Key = "B"}.Insert();
        Assert.IsNotEmpty(LazyClient.Select<ExtendedTable>());
        LazyClient.Truncate("extended_table");
        Assert.IsEmpty(LazyClient.Select<ExtendedTable>());
    }

    [Test]
    public void TruncateSchema()
    {
        ClientTest.CleanTables();

        new PrimaryValue() {Value = "Hello"}.Insert();
        Assert.IsNotEmpty(LazyClient.Select<PrimaryValue>());
        LazyClient.Truncate<ExtendedTable>();
        Assert.IsEmpty(LazyClient.Select<ExtendedTable>());

        LazyClient.Insert("lazys", "tableprimary", new TablePrimary {Value = "Hello"}, nameof(TablePrimary.Id));
        Assert.IsNotEmpty(LazyClient.Select<dynamic>("lazys", "tableprimary"));
        LazyClient.Truncate("lazys", "tableprimary");
        Assert.IsEmpty(LazyClient.Select<dynamic>("lazys", "tableprimary"));

    }
}