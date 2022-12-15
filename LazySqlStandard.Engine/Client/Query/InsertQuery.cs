namespace LazySql;

internal class InsertQuery : QueryBase
{
    private readonly object _obj;
    private string _autoIncrementColumnName;
    private string[] _excludedColumn;

    public InsertQuery(object obj, ITableDefinition tableDefinition, string schema, string tableName) : base(tableDefinition, schema, tableName)
    {
        _obj = obj;
    }


    public virtual QueryBuilder BuildQuery(out PropertyInfo autoIncrementProperty)
    {
        TableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _, out IReadOnlyList<ColumnDefinition> columnsPrimaryKeys);

        List<ColumnDefinition> columnsToInsert = columns.ToList();

        if (_autoIncrementColumnName != null)
        {
            _excludedColumn ??= new string[] {};
            _excludedColumn = _excludedColumn.Append(_autoIncrementColumnName).ToArray();
        }
        if (_excludedColumn != null)
        {
            columnsToInsert.RemoveAll(c => _excludedColumn.Any(ec => string.Equals(ec, c.Column.ColumnName,
                StringComparison.Ordinal)));
        }


        QueryBuilder.Append($"INSERT INTO {SqlHelper.TableName(Schema, TableName)}");

        QueryBuilder.Append(
            $" ({string.Join(", ", columnsToInsert.Where(c => c.Column.SqlType != SqlType.Children).Select(c => c.Column.SqlColumnName))})");

        autoIncrementProperty = null;
        string autoIncrementColumnName = null;
        if (!string.IsNullOrEmpty(_autoIncrementColumnName))
        {
            
            autoIncrementProperty = _obj.GetType().GetProperties().FirstOrDefault(p => p.CanWrite &&
                                                                           string.Equals(p.Name, _autoIncrementColumnName,
                                                                               StringComparison.Ordinal));
            if (autoIncrementProperty == null)
                throw new LazySqlException($"Property '{_autoIncrementColumnName}' not found");
            autoIncrementColumnName = autoIncrementProperty.Name;
        }
        else
        {
            ColumnDefinition autoIncrementColumn = columnsPrimaryKeys.FirstOrDefault(c => c.PrimaryKey.AutoIncrement);
            if (autoIncrementColumn != null)
            {
                autoIncrementColumnName = autoIncrementColumn.Column.SqlColumnName;
                autoIncrementProperty = autoIncrementColumn.PropertyInfo;
            }
        }

        if (autoIncrementColumnName != null)
            QueryBuilder.Append($" output INSERTED.{autoIncrementColumnName}");

        List<string> values = columnsToInsert.Where(c => c.Column.SqlType != SqlType.Children).Select(columnDefinition =>
            QueryBuilder.RegisterArgument(columnDefinition.Column.SqlType, columnDefinition.GetValue(_obj))).ToList();
        QueryBuilder.Append($" VALUES ({string.Join(", ", values)})");
        
        return QueryBuilder;
    }

    public void SetAutoIncrementColumn(string autoIncrementColumnName)
    {
        _autoIncrementColumnName = autoIncrementColumnName;
    }

    public void SetExcludedColumns(string[] excludedColumn)
    {
        _excludedColumn = excludedColumn;
    }
}