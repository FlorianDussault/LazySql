namespace LazySql;

internal sealed class TableDefinitionObject : TableDefinitionBase, ITableDefinition
{
    public ObjectType ObjectType => ObjectType.Object;
    public bool HasRelations => false;
    public string GetTableName(string tableName = null) => tableName ?? Table.TableName;
    public string GetSchema(string schema = null) => schema ?? Table.Schema;

    public string GetSchemaAndTableName(string schema = null, string tableName = null) => SqlHelper.TableName(GetSchema(schema), GetTableName(tableName));

    public TableDefinitionObject(Type type, LazyTable table) : base(type, table)
    {
    }
}