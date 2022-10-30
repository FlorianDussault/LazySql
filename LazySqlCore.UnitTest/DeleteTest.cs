using LazySql.Engine;
using LazySql.Engine.Client;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest
{
    public class DeleteTest
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
            LazyClient.Truncate<ChildTable>();
            LazyClient.Truncate<SimpleTable>(true);
            // Add values
            Assert.IsEmpty(LazyClient.Get<SimpleTable>());
            int bot_id = 0;
            for (int i = 0; i < COUNT_SIMPLE_TABLE; i++)
            {
                var st = new SimpleTable()
                {
                    Username = "",
                    Password = ""
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
            Assert.That(LazyClient.Get<SimpleTable>().ToList().Count, Is.EqualTo(COUNT_SIMPLE_TABLE));
            Assert.That(LazyClient.Get<ChildTable>().ToList().Count, Is.EqualTo(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE ));
        }

        [Test]
        public void Truncate()
        {
            AddSimpleTables();
            LazyClient.Truncate<ChildTable>();
            Assert.IsEmpty(LazyClient.Get<ChildTable>());
            LazyClient.Truncate<SimpleTable>(true);
            Assert.IsEmpty(LazyClient.Get<SimpleTable>());
        }

        [Test]
        public void Delete()
        {
            AddSimpleTables();
            
            // Clear data
            foreach (ChildTable childTable in LazyClient.Get<ChildTable>())
                childTable.Delete();
            foreach (SimpleTable simpleTable in LazyClient.Get<SimpleTable>())
                simpleTable.Delete();
            Assert.IsEmpty(LazyClient.Get<ChildTable>());
            Assert.IsEmpty(LazyClient.Get<SimpleTable>());
        }

        [Test]
        public void DeleteWithArgs()
        {
            AddSimpleTables();
            LazyClient.Delete<ChildTable>((i)=>i.Id <= 10 || i.Id == 20);
            List<int> allowedIds = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20};
            foreach (ChildTable childTable in LazyClient.Get<ChildTable>())
            {
                Assert.IsFalse((allowedIds.Any(id => childTable.Id == id)), $"{childTable.Id} not deleted");
            }
        }
    }
}