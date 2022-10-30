using LazySql.Engine;
using LazySql.Engine.Attributes;
using LazySql.Engine.Enums;

namespace LazySqlCore.UnitTest.Tables
{
    [LazyTable("extended_table")]
    public class ExtendedTable : LazyBase
    {
        [LazyColumn(SqlType.String)]
        [PrimaryKey(true)]
        public int Id { get; set; }

        [LazyColumn(SqlType.String)]
        public string Key { get; set; }

        [LazyColumn(SqlType.Int32)]
        public int Value { get; set; }
    }
}
