

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
    public static void Insert<T>(T obj) where T : LazyBase => Instance.InternalInsert(typeof(T), obj);

    /// <summary>
    /// Insert an item
    /// </summary>
    /// <param name="type">Type of the item</param>
    /// <param name="obj">Item</param>
    private void InternalInsert(Type type, object obj)
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


        List<string> values = columns.Where(c => c.Column.SqlType != SqlType.Children).Select(columnDefinition => queryBuilder.RegisterArgument(columnDefinition.Column.SqlType ,columnDefinition.GetValue(obj))).ToList();
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

    #endregion
}