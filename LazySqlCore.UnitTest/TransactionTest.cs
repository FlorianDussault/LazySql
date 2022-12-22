using LazySql;
using LazySql.Transaction;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;

[TestFixture(TestName = "Transaction")]
public class TransactionTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void Commit()
    {
        ClientTest.CleanTables();
        
        using LazyTransaction lazyTransaction = LazyClient.BeginTransaction("UT_TEST_COMMIT");

        List<int> ids = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            SimpleTable simpleTable = new() {Username = $"UT_TANSAC_{i}"};
            lazyTransaction.Insert(simpleTable);
            Assert.IsFalse(ids.Any(id=>id == simpleTable.Id));
            ids.Add(simpleTable.Id);
        }
        
        Assert.IsNotEmpty(lazyTransaction.Select<SimpleTable>());

        foreach (SimpleTable simpleTable in lazyTransaction.Select<SimpleTable>())
            Assert.IsTrue(ids.Any(id => id == simpleTable.Id));

        lazyTransaction.Commit();
        Assert.IsNotEmpty(LazyClient.Select<SimpleTable>());
    }
    
    [Test]
    public void Rollback()
    {
        ClientTest.CleanTables();
        
        using LazyTransaction lazyTransaction = LazyClient.BeginTransaction("UT_TEST_COMMIT");

        List<int> ids = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            SimpleTable simpleTable = new() {Username = $"UT_TANSAC_{i}"};
            lazyTransaction.Insert(simpleTable);
            Assert.IsFalse(ids.Any(id=>id == simpleTable.Id));
            ids.Add(simpleTable.Id);
        }
        
        Assert.IsNotEmpty(lazyTransaction.Select<SimpleTable>());

        foreach (SimpleTable simpleTable in lazyTransaction.Select<SimpleTable>())
            Assert.IsTrue(ids.Any(id => id == simpleTable.Id));

        lazyTransaction.Rollback();
        Assert.IsEmpty(LazyClient.Select<SimpleTable>());
    }
}