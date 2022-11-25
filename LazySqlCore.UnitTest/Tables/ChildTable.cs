
using LazySql;

namespace LazySqlCore.UnitTest.Tables;

[LazyTable("child_table")]
public class ChildTable : LazyBase
{
    [LazyColumn("id", SqlType.Int)]
    [PrimaryKey(false)]
    public int Id { get; set; }

    [LazyColumn("simple_table_id", SqlType.Int)]
    public int ParentId { get; set; }
        
    [LazyColumn("texte", SqlType.VarChar)]

    public string TypeChar { get; set; }

    public  List<SubChildTable> SubChildTables { get; set; }

    public override void InitializeTable()
    {
        AddOneToMany<ChildTable, SubChildTable>(nameof(SubChildTables), (c, s) => c.Id == s.ParentId);
    }
}