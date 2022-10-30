using System.Formats.Asn1;
using LazySql.Engine;
using LazySql.Engine.Client;
using LazySql.Engine.Client.Functions;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest
{
    public class LambdaTest
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
            SqlClient.Truncate<ChildTable>();
            SqlClient.Truncate<SimpleTable>(true);
            // Add values
            Assert.IsEmpty(SqlClient.Get<SimpleTable>());
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
            Assert.AreEqual(COUNT_SIMPLE_TABLE, SqlClient.Get<SimpleTable>().ToList().Count());
            Assert.AreEqual(COUNT_SIMPLE_TABLE * COUNT_CHILD_TABLE ,SqlClient.Get<ChildTable>().ToList().Count());
        }

        [Test]
        public void GetSimple()
        {
            AddSimpleTables();

            foreach (SimpleTable simpleTable in SqlClient.Get<SimpleTable>(s => s.Id < 10))
            {
                Assert.Less(simpleTable.Id,10);
            }

        }

        [Test]
        public void LzLike()
        {
            AddSimpleTables();

            foreach (SimpleTable simpleTable in SqlClient.Get<SimpleTable>(s => LzFunctions.Like(s.Id, "%1%") && LzFunctions.Like(s.Id,"%5") ))
            {
                Assert.IsTrue(simpleTable.Id.ToString().Contains("1"));
                Assert.IsTrue(simpleTable.Id.ToString().Contains("5"));
            }
        }
        
    }
}