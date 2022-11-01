using LazySql.Engine;
using LazySql.Engine.Client;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest
{
    public class InsertTest
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
            Assert.IsEmpty(LazyClient.Get<SimpleTable>());
            int bot_id = 0;
            for (int i = 0; i < COUNT_SIMPLE_TABLE; i++)
            {
                var st = new SimpleTable()
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
            Assert.That(LazyClient.Get<SimpleTable>().ToList().Count, Is.EqualTo(COUNT_SIMPLE_TABLE));
            Assert.That(LazyClient.Get<ChildTable>().ToList().Count, Is.EqualTo(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE ));
        }

        [Test]
        public void Insert()
        {
            AddSimpleTables();
        }

        [Test]
        public void InsertNull()
        {
            LazyClient.Truncate<SubChildTable>(true);
            LazyClient.Delete<ChildTable>();
            LazyClient.Truncate<SimpleTable>(true);

            new SimpleTable()
            {

            }.Insert();

            SimpleTable item = LazyClient.Get<SimpleTable>().First();
            Assert.That(item.Id, Is.EqualTo(1));
            Assert.IsNull(item.Username);
            Assert.IsNull(item.Password);
        }
        
    }
}