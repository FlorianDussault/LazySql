using LazySql;

namespace LazySqlCore.UnitTest.Tables;

[LazyTable("lazys", "hierarchy_schema")]
public class HierarchySchema : LazyBase
{
    [LazyColumn(SqlType.Int)]
    [PrimaryKey(true)]
    public int Id { get; set; }

    [LazyColumn("parent_id", SqlType.Int)]
    public int? ParentId { get; set; }

    [LazyColumn(SqlType.NVarChar)]
    public string Name { get; set; }

    public List<HierarchySchema> Children { get; set; }

    public override void OnLoaded()
    {
        int id = Id;
        Children = LazyClient.Select<HierarchySchema>(child => child.ParentId == id).ToList();
    }

    public override string ToString()
    {
        return $"{Id} - {Name}";
    }
}