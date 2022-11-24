namespace LazySql.Engine.Definitions;

internal sealed class TableDefinitionDynamic : TableDefinitionBase, ITableDefinition
{
    public ObjectType ObjectType => ObjectType.Dynamic;
    public bool HasRelations => false;
    public string GetTableName(string tableName) => tableName;

    public TableDefinitionDynamic(Type type, LazyTable table) : base(type, table)
    {
    }
}