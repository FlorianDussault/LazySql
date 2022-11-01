using LazySql.Engine;
using LazySql.Engine.Attributes;
using LazySql.Engine.Enums;

namespace LazySqlCore.UnitTest.Tables
{
    [LazyTable("subchild_table")]
    public class SubChildTable : LazyBase
    {
        [LazyColumn(SqlType.Int)]
        [PrimaryKey(true)]
        public int Id { get; set; }

        [LazyColumn("parent_id", SqlType.Int)]
        public int? ParentId { get; set; }

        [LazyColumn(SqlType.VarChar)]
        public string Value { get; set; }
    }
}
