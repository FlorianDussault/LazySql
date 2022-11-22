

// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    #region Insert

    /// <summary>
    /// Insert an item
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    /// <param name="obj">Item</param>
    /// <param name="autoIncrementColumn"></param>
    /// <param name="excludedColumns">Excluded columns</param>
    public static void Insert<T>(T obj, string autoIncrementColumn = null, params string[] excludedColumns) => Instance.InternalInsert(typeof(T), obj, autoIncrementColumn, excludedColumns);

    /// <summary>
    /// Insert an item
    /// </summary>
    /// <param name="type">Type of the item</param>
    /// <param name="obj">Item</param>
    /// <param name="autoIncrementColumn"></param>
    /// <param name="excludedColumns"></param>
    private void InternalInsert(Type type, object obj, string autoIncrementColumn, string[] excludedColumns)
    {
        if (obj is LazyBase)
        {
            InternalInsertLazy(type, obj);
        }
        else
        {
            InternalInsertObject(type, obj, autoIncrementColumn, excludedColumns);
        }

    }

    private void InternalInsertLazy(Type type, object obj)
    {
        CheckInitialization(type, out TableDefinition tableDefinition);
        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _,
            out IReadOnlyList<ColumnDefinition> primaryKeys);

        ColumnDefinition autoIncrementColumn = primaryKeys.FirstOrDefault(c => c.PrimaryKey.AutoIncrement);

        QueryBuilder queryBuilder = new QueryBuilder(tableDefinition);
        queryBuilder.Append($"INSERT INTO {tableDefinition.Table.TableName}");

        queryBuilder.Append(
            $" ({string.Join(", ", columns.Where(c => c.Column.SqlType != SqlType.Children).Select(c => c.Column.SqlColumnName))})");

        if (autoIncrementColumn != null)
            queryBuilder.Append($" output INSERTED.{autoIncrementColumn.Column.SqlColumnName}");


        List<string> values = columns.Where(c => c.Column.SqlType != SqlType.Children).Select(columnDefinition =>
            queryBuilder.RegisterArgument(columnDefinition.Column.SqlType, columnDefinition.GetValue(obj))).ToList();
        queryBuilder.Append($" VALUES ({string.Join(", ", values)})");

        if (autoIncrementColumn != null)
        {
            object output = ExecuteScalar(queryBuilder);
            autoIncrementColumn.PropertyInfo.SetValue(obj, output);
        }
        else
        {
            ExecuteNonQuery(queryBuilder);
        }
    }

    private void InternalInsertObject(Type type, object obj, string autoIncrementColumn, string[] excludedColumns)
    {
        CheckInitialization(type, out TableDefinition tableDefinition);
        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> allColumns, out _, out _);

        ColumnDefinition autoColumnDefinition = null;
        if (!string.IsNullOrWhiteSpace(autoIncrementColumn))
        {
            autoColumnDefinition = allColumns.First(c =>
                string.Equals(c.Column.ColumnName, autoIncrementColumn, StringComparison.InvariantCultureIgnoreCase));

            excludedColumns ??= Array.Empty<string>();
            excludedColumns = excludedColumns.Append(autoIncrementColumn).ToArray();
        }

        List<ColumnDefinition> columns = excludedColumns == null
            ? allColumns.ToList()
            : allColumns.Where(column => excludedColumns.All(c =>
                !string.Equals(c, column.Column.ColumnName, StringComparison.InvariantCultureIgnoreCase))).ToList();
        
        QueryBuilder queryBuilder = new QueryBuilder(tableDefinition);
        queryBuilder.Append($"INSERT INTO {tableDefinition.Table.TableName}");

        queryBuilder.Append(
            $" ({string.Join(", ", columns.Select(c => c.Column.SqlColumnName))})");

        if (autoIncrementColumn != null)
            queryBuilder.Append($" output INSERTED.{autoIncrementColumn}");


        List<string> values = columns.Select(columnDefinition =>
            queryBuilder.RegisterArgument(columnDefinition.Column.SqlType, columnDefinition.GetValue(obj))).ToList();
        queryBuilder.Append($" VALUES ({string.Join(", ", values)})");

        if (autoColumnDefinition != null)
        {
            object output = ExecuteScalar(queryBuilder);
            autoColumnDefinition.PropertyInfo.SetValue(obj, output);
        }
        else
        {
            ExecuteNonQuery(queryBuilder);
        }
    }

    #endregion
}