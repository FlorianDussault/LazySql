using LazySql;

namespace LazySqlCore.UnitTest.Tables
{
    [LazyTable("hierarchy_table")]
    public class HierarchyTable : LazyBase
    {
        [LazyColumn(SqlType.Int)]
        [PrimaryKey(true)]
        public int Id { get; set; }

        [LazyColumn("parent_id", SqlType.Int)]
        public int? ParentId { get; set; }

        [LazyColumn(SqlType.NVarChar)]
        public string Name { get; set; }

        public List<HierarchyTable> Children {get; set; }

        public override void OnLoaded()
        {
            int id = Id;
            Children = LazyClient.Select<HierarchyTable>(child => child.ParentId == id).ToList();
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
