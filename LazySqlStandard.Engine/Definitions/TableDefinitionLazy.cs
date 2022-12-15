namespace LazySql;

/// <summary>
/// Table Definition
/// </summary>
internal sealed class TableDefinitionLazy : TableDefinitionBase, ITableDefinition
{
    public ObjectType ObjectType => ObjectType.LazyObject;
    public bool HasRelations => Relations != null && Relations.Any();
    public string GetTableName(string tableName = null) => tableName ?? Table.TableName;
    public string GetSchema(string schema = null) => schema ?? Table.Schema;
    public string GetSchemaAndTableName(string schema = null, string tableName = null) => SqlHelper.TableName(GetSchema(schema), GetTableName(tableName));

    public TableDefinitionLazy(Type type, LazyTable table) : base(type, table)
    {
    }

    
}