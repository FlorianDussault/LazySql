namespace LazySql;

/// <summary>
/// Table Definition
/// </summary>
internal sealed class TableDefinitionLazy : TableDefinitionBase, ITableDefinition
{
    public ObjectType ObjectType => ObjectType.LazyObject;
    public bool HasRelations => Relations != null && Relations.Any();
    public string GetTableName(string tableName = null) => tableName ?? Table.TableName;

    public TableDefinitionLazy(Type type, LazyTable table) : base(type, table)
    {
    }

    
}