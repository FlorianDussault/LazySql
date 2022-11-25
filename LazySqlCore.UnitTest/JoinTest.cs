using LazySql;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest;
[TestFixture(TestName = "Table Join")]
public class JoinTest
{
    [SetUp]
    public void Setup()
    {
        ClientTest.Initialize();
    }

    [Test]
    public void OneToOne()
    {
        ClientTest.CleanTables();

        new ExtendedTable() {Key = "AA", Value = 1}.Insert();
        new ExtendedTable() {Key = "BB", Value = 2}.Insert();
        new ExtendedTable() {Key = "CC", Value = 3}.Insert();
        new ExtendedTable() {Key = "DD", Value = 4}.Insert();

        new SimpleTable() {Username = "USR_1", Password = "PWD_1", ExtendedKey = "DD"}.Insert();
        new SimpleTable() {Username = "USR_2", Password = "PWD_2", ExtendedKey = "CC"}.Insert();
        new SimpleTable() {Username = "USR_3", Password = "PWD_3", ExtendedKey = "BB"}.Insert();
        new SimpleTable() {Username = "USR_4", Password = "PWD_4", ExtendedKey = "AA"}.Insert();

        List<SimpleTable> tables = LazyClient.Select<SimpleTable>().ToList();
        foreach (SimpleTable simpleTable in tables)
        {
            Assert.NotNull(simpleTable.Extended);
            Assert.IsTrue(simpleTable.ExtendedKey == simpleTable.Extended.Key);
        }
    }

  
    [Test]
    public void Hierarchy()
    {
        ClientTest.CleanTables();

        int childId = 0;
        for (int i = 0; i < 5; i++)
        {
            SimpleTable simpleTable = new() { };
            simpleTable.Insert();

            for (int j = 0; j < 9; j++)
            {
                ChildTable childTable = new() {Id = ++childId, ParentId = simpleTable.Id};
                childTable.Insert();

                for (int k = 0; k < 10; k++)
                {
                    SubChildTable subChildTable = new() {ParentId = childTable.Id, Value = $"{simpleTable.Id} - {childTable.Id}"};
                    subChildTable.Insert();
                    subChildTable.Value = $"{simpleTable.Id} - {childTable.Id} - {subChildTable.Id}";
                    subChildTable.Update();
                }
            }
        }

        List<SimpleTable> datas = LazyClient.Select<SimpleTable>().ToList();

        Assert.That(datas.Count, Is.EqualTo(5));
        foreach (SimpleTable simpleTable in datas)
        {
            Assert.That(simpleTable.ChildTables.Count, Is.EqualTo(9));
            foreach (ChildTable childTable in simpleTable.ChildTables)
            {
                Assert.That(childTable.SubChildTables.Count, Is.EqualTo(10));
                foreach (SubChildTable subChildTable in childTable.SubChildTables)
                {
                    Assert.That($"{simpleTable.Id} - {childTable.Id} - {subChildTable.Id}", Is.EqualTo(subChildTable.Value));
                }
            }
        }
    }
}