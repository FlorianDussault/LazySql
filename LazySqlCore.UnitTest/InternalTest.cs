using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazySql;

namespace LazySqlCore.UnitTest
{
    [TestFixture(TestName ="Internal")]
    internal class InternalTest
    {
        [Test]
        public void ListHelper_ForeachWithLast()
        {
            new List<string>().ForeachWithLast().ToList();
        }
    }
}
