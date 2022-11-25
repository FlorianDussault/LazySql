namespace LazySql;

internal interface ITableDefinition : IList<ColumnDefinition>
{
    public Type TableType { get; }

    /// <summary>
    /// LazyTable attribute
    /// </summary>
    public LazyTable Table { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public ObjectType ObjectType { get; }

    public RelationsInformation Relations { get; set; }

    public bool HasRelations { get; }

    public void GetColumns(out IReadOnlyList<ColumnDefinition> allColumns,
        out IReadOnlyList<ColumnDefinition> columnsWithoutAutoIncrement,
        out IReadOnlyList<ColumnDefinition> columnsWithoutPrimaryKeys, out IReadOnlyList<ColumnDefinition> primaryKeys);

    public ColumnDefinition GetColumn(string propertyName);

    public string GetTableName(string tableName = null);

}