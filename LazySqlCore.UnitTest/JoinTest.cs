using LazySqlCore.UnitTest.Tables;
using LazySql.Engine;
using LazySql.Engine.Client;

namespace LazySqlCore.UnitTest
{
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
            LazyClient.Truncate<SubChildTable>(true);
            LazyClient.Truncate<ExtendedTable>();
            Assert.IsEmpty(LazyClient.Get<ExtendedTable>());
            LazyClient.Delete<ChildTable>();
            Assert.IsEmpty(LazyClient.Get<ChildTable>());
            LazyClient.Truncate<SimpleTable>(true);
            Assert.IsEmpty(LazyClient.Get<SimpleTable>());

            new ExtendedTable() {Key = "AA", Value = 1}.Insert();
            new ExtendedTable() {Key = "BB", Value = 2}.Insert();
            new ExtendedTable() {Key = "CC", Value = 3}.Insert();
            new ExtendedTable() {Key = "DD", Value = 4}.Insert();

            new SimpleTable() {Username = "USR_1", Password = "PWD_1", ExtendedKey = "DD"}.Insert();
            new SimpleTable() {Username = "USR_2", Password = "PWD_2", ExtendedKey = "CC"}.Insert();
            new SimpleTable() {Username = "USR_3", Password = "PWD_3", ExtendedKey = "BB"}.Insert();
            new SimpleTable() {Username = "USR_4", Password = "PWD_4", ExtendedKey = "AA"}.Insert();

            List<SimpleTable>? tables = LazyClient.Get<SimpleTable>().ToList();
            foreach (SimpleTable simpleTable in tables)
            {
                Assert.NotNull(simpleTable.Extended);
                Assert.IsTrue(simpleTable.ExtendedKey == simpleTable.Extended.Key);
            }
        }

        

        [Test]
        public void Hierarchy()
        {
            LazyClient.Truncate<SubChildTable>(true);
            Assert.IsEmpty(LazyClient.Get<SubChildTable>());
            LazyClient.Delete<ChildTable>();
            Assert.IsEmpty(LazyClient.Get<ChildTable>());
            LazyClient.Truncate<SimpleTable>(true);
            Assert.IsEmpty(LazyClient.Get<SimpleTable>());

            int childId = 0;
            for (int i = 0; i < 5; i++)
            {
                SimpleTable simpleTable = new SimpleTable() { };
                simpleTable.Insert();

                for (int j = 0; j < 9; j++)
                {
                    ChildTable childTable = new ChildTable() {Id = ++childId, ParentId = simpleTable.Id};
                    childTable.Insert();

                    for (int k = 0; k < 10; k++)
                    {
                        SubChildTable subChildTable = new SubChildTable() {ParentId = childTable.Id, Value = $"{simpleTable.Id} - {childTable.Id}"};
                        subChildTable.Insert();
                        subChildTable.Value = $"{simpleTable.Id} - {childTable.Id} - {subChildTable.Id}";
                        subChildTable.Update();
                    }
                }
            }

            List<SimpleTable>? datas = LazyClient.Get<SimpleTable>().ToList();

            Assert.That(5, Is.EqualTo(datas.Count));
            foreach (SimpleTable simpleTable in datas)
            {
                Assert.That(9, Is.EqualTo(simpleTable.ChildTables.Count));
                foreach (ChildTable childTable in simpleTable.ChildTables)
                {
                    Assert.That(10, Is.EqualTo(childTable.SubChildTables.Count));
                    foreach (SubChildTable subChildTable in childTable.SubChildTables)
                    {
                        Assert.That($"{simpleTable.Id} - {childTable.Id} - {subChildTable.Id}", Is.EqualTo(subChildTable.Value));
                    }
                }
            }
        }
    }
}
