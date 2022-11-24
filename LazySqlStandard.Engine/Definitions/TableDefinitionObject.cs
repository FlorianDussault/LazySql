namespace LazySql.Engine.Definitions;

internal sealed class TableDefinitionObject : TableDefinitionBase, ITableDefinition
{
    public ObjectType ObjectType => ObjectType.Object;
    public bool HasRelations => false;
    public string GetTableName(string tableName = null) => tableName ?? Table.TableName;

    public TableDefinitionObject(Type type, LazyTable table) : base(type, table)
    {
    }
}