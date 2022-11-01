using LazySql.Engine;
using LazySql.Engine.Attributes;
using LazySql.Engine.Enums;

namespace LazySqlCore.UnitTest.Tables
{
    [LazyTable("extended_table")]
    public class ExtendedTable : LazyBase
    {
        [LazyColumn(SqlType.Int)]
        [PrimaryKey(true)]
        public int Id { get; set; }

        [LazyColumn(SqlType.VarChar)]
        public string Key { get; set; }

        [LazyColumn(SqlType.Int)]
        public int Value { get; set; }
    }
}
