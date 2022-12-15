using System;
using System.Collections.Generic;
using System.Text;
using LazySql;

namespace LazySqlCore.UnitTest.Tables
{
    [LazyTable("lazys", "tablePrimary")]
    internal class PrimaryValue : LazyBase
    {
        [LazyColumn("id",SqlType.Int),PrimaryKey(true)]
        public int Key { get; set; }
        [LazyColumn(SqlType.VarChar)]
        public string Value { get; set; }
    }

    internal class TablePrimary
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
