using LazySqlCore.UnitTest.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SqlClient.Truncate<ExtendedTable>();
            Assert.IsEmpty(SqlClient.Get<ExtendedTable>());
            SqlClient.Truncate<ChildTable>();
            Assert.IsEmpty(SqlClient.Get<ChildTable>());
            SqlClient.Truncate<SimpleTable>(true);
            Assert.IsEmpty(SqlClient.Get<SimpleTable>());

            new ExtendedTable() {Key = "AA", Value = 1}.Insert();
            new ExtendedTable() {Key = "BB", Value = 2}.Insert();
            new ExtendedTable() {Key = "CC", Value = 3}.Insert();
            new ExtendedTable() {Key = "DD", Value = 4}.Insert();

            new SimpleTable() {Username = "USR_1", Password = "PWD_1", ExtendedKey = "DD"}.Insert();
            new SimpleTable() {Username = "USR_2", Password = "PWD_2", ExtendedKey = "CC"}.Insert();
            new SimpleTable() {Username = "USR_3", Password = "PWD_3", ExtendedKey = "BB"}.Insert();
            new SimpleTable() {Username = "USR_4", Password = "PWD_4", ExtendedKey = "AA"}.Insert();

            var tables = SqlClient.Get<SimpleTable>().ToList();
            foreach (SimpleTable simpleTable in tables)
            {
                Assert.NotNull(simpleTable.Extended);
                Assert.IsTrue(simpleTable.ExtendedKey == simpleTable.Extended.Key);
            }
        }

        [Test]
        public void OneToOneFlatten()
        {

        }
    }
}
