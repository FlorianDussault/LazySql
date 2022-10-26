using System.Formats.Asn1;
using LazySql.Engine;
using LazySql.Engine.Client;
using LazySqlCore.UnitTest.Tables;

namespace LazySqlCore.UnitTest
{
    public class TypesTest
    {
        [SetUp]
        public void Setup()
        {
            ClientTest.Initialize();
        }

        private void Reset()
        {
            SqlClient.Truncate<TypesTable>();
        }

        [Test]
        public void TypeBigint()
        {
            Reset();
            var val = long.MaxValue;
            new TypesTable() {TypeBigint = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeBigint == val);
        }

        [Test]
        public void TypeBinary()
        {
            Reset();
            byte[] val = new byte[50];
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < val.Length; i++)
            {
                val[i] = (byte)rand.Next(0, 255);
            }
            new TypesTable() { TypeBinary = val }.Insert();
            byte[] result = SqlClient.Get<TypesTable>().First().TypeBinary;

            for (int i = 0; i < val.Length; i++)
            {
                Assert.AreEqual(val[i], result[i]);
            }
        }
        [Test]
        public void TypeBit()
        {
            Reset();
            var val = true;
            new TypesTable() { TypeBit = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeBit == val);
        }

        [Test]
        public void TypeChar()
        {
            Reset();
            string val = "hello";
            new TypesTable() { TypeChar = val }.Insert();
            Assert.AreEqual(val, SqlClient.Get<TypesTable>().First().TypeChar.Trim());
        }

        [Test]
        public void TypeDate()
        {
            Reset();
            DateTime val = DateTime.Today;
            new TypesTable() { TypeDate = val }.Insert();
            Assert.AreEqual(SqlClient.Get<TypesTable>().First().TypeDate,val);
        }

        [Test]
        public void TypeDateTime2()
        {
            Reset();
            DateTime val = DateTime.Now;
            new TypesTable() { TypeDateTime2 = val }.Insert();
            Assert.AreEqual(SqlClient.Get<TypesTable>().First().TypeDateTime2, val);
        }

        [Test]
        public void TypeDatetimeoffset()
        {
            Reset();
            DateTimeOffset val = DateTimeOffset.Now;
            new TypesTable() { TypeDatetimeoffset = val }.Insert();
            Assert.AreEqual(SqlClient.Get<TypesTable>().First().TypeDatetimeoffset, val);
        }

        [Test]
        public void TypeDecimal()
        {
            Reset();
            decimal val = 1.5M;
            new TypesTable() { TypeDecimal = val }.Insert();
            Assert.AreEqual(SqlClient.Get<TypesTable>().First().TypeDecimal,val);
        }

        [Test]
        public void TypeFloat()
        {
            Reset();
            double val = 1.14F;
            new TypesTable() { TypeFloat = val }.Insert();
            Assert.AreEqual(SqlClient.Get<TypesTable>().First().TypeFloat, val);
        }

        [Test]
        [Ignore("Geography")]
        public void TypeGeography()
        {
        //    Reset();
        //    var val = long.MaxValue;
        //    new TypesTable() { TypeGeography = val }.Insert();
        //    Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeGeography == val);
        }

        [Test]
        [Ignore("Hierarchyid")]
        public void TypeHierarchyid()
        {
        //    Reset();
        //    var val = long.MaxValue;
        //    new TypesTable() { TypeHierarchyid = val }.Insert();
        //    Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeHierarchyid == val);
        }
        [Test]
        public void TypeImage()
        {
            Reset();
            byte[] val = new byte[50];
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < val.Length; i++)
            {
                val[i] = (byte)rand.Next(0, 255);
            }
            new TypesTable() { TypeImage = val }.Insert();
            byte[] result = SqlClient.Get<TypesTable>().First().TypeImage;

            for (int i = 0; i < val.Length; i++)
            {
                Assert.AreEqual(val[i], result[i]);
            }
        }
 
        [Test]
        public void TypeInt()
        {
            Reset();
            var val = int.MaxValue;
            new TypesTable() { TypeInt = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeInt == val);
        }
        [Test]
        public void TypeMoney()
        {
            Reset();
            decimal val = 1.14M;
            new TypesTable() { TypeMoney = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeMoney == val);
        }

        [Test]
        public void TypeNtext()
        {
            Reset();
            string val = "ntext value";
            new TypesTable() { TypeNtext = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeNtext == val);
        }

        [Test]
        public void TypeNumeric()
        {
            Reset();
            decimal val = 1.14M;
            new TypesTable() { TypeDecimal = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeDecimal == val);
        }


        [Test]
        public void TypeReal()
        {
            Reset();
            var val = Single.Epsilon;
            new TypesTable() { TypeReal = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeReal == val);
        }

        [Test]
        public void TypeSmalldatime()
        {
            Reset();
            var val = DateTime.Today;
            new TypesTable() { TypeSmalldatime = val }.Insert();
            Assert.AreEqual(SqlClient.Get<TypesTable>().First().TypeSmalldatime, val);
        }
        [Test]
        public void TypeSmallint()
        {
            Reset();
            var val = short.MaxValue;
            new TypesTable() { TypeSmallint = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeSmallint == val);
        }
        [Test]
        public void TypeSmallmoney()
        {
            Reset();
            var val = 1.14M;
            new TypesTable() { TypeSmallmoney = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeSmallmoney == val);
        }
        [Test]
        public void TypeSqlVariant()
        {
            Reset();
            var val ="VARIANT";
            new TypesTable() { TypeSqlVariant = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeSqlVariant == val);
        }
        [Test]
        public void TypeText()
        {
            Reset();
            var val = "TYPE TEXT";
            new TypesTable() { TypeText = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeText == val);
        }

        [Test]
        public void TypeTime()
        {
            Reset();
            var val = new TimeSpan(0, 9, 8, 7);
            new TypesTable() { TypeTime = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeTime == val);
        }

        [Test]
        [Ignore("Timestamp")]
        public void TypeTimestamp()
        {
            Reset();
            //byte[] val = new byte[5];
            //Random rand = new Random(DateTime.Now.Millisecond);
            //for (int i = 0; i < val.Length; i++)
            //{
            //    val[i] = (byte)rand.Next(0, 255);
            //}
            //new TypesTable() { TypeTimestamp = val }.Insert();
            //byte[] result = SqlClient.Get<TypesTable>().First().TypeTimestamp;

            //for (int i = 0; i < val.Length; i++)
            //{
            //    Assert.AreEqual(val[i], result[i]);
            //}
        }

        [Test]
        public void TypeTinyint()
        {
            Reset();
            var val = byte.MaxValue;
            new TypesTable() { TypeTinyint = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeTinyint == val);
        }

        [Test]
        public void TypeUniqueidentifier()
        {
            Reset();
            var val = Guid.NewGuid();
            new TypesTable() { TypeUniqueidentifier = val }.Insert();
            Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeUniqueidentifier == val);
        }

        [Test]
        [Ignore("XML")]
        public void TypeXml()
        {
            //Reset();
            //var val = long.MaxValue;
            //new TypesTable() { TypeXml = val }.Insert();
            //Assert.IsTrue(SqlClient.Get<TypesTable>().First().TypeXml == val);
        }



    }
}