
using LazySql.Engine;
using LazySql.Engine.Attributes;
using LazySql.Engine.Enums;

namespace LazySqlCore.UnitTest.Tables
{
    [LazyTable("child_table")]
    public class ChildTable : LazyBase
    {
        [LazyColumn("id", SqlType.Int32)]
        [PrimaryKey(false)]
        public int Id { get; set; }

        [LazyColumn("simple_table_id", SqlType.Int32)]
        public int ParentId { get; set; }
        
        [LazyColumn("texte", SqlType.String)]

        public string TypeChar { get; set; }
    }
}
