namespace LazySql;

internal sealed class TableDefinitionDynamic : TableDefinitionBase, ITableDefinition
{
    public ObjectType ObjectType => ObjectType.Dynamic;
    public bool HasRelations => false;
    public string GetTableName(string tableName) => tableName;
    public string GetSchema(string schema = null) => schema;

    public string GetSchemaAndTableName(string schema, string tableName) => SqlHelper.TableName(schema, tableName);

    public TableDefinitionDynamic(Type type, LazyTable table) : base(type, table)
    {
    }
}