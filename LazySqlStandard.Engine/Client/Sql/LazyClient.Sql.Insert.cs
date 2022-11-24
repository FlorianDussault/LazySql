

// ReSharper disable once CheckNamespace
using System.Dynamic;
using static System.Net.Mime.MediaTypeNames;

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
    public static void Insert(object obj, string tableName = null, string autoIncrementColumn = null, params string[] excludedColumns) => Instance.InternalInsert(obj, tableName, autoIncrementColumn, excludedColumns);

    /// <summary>
    /// Insert an item
    /// </summary>
    /// <param name="type">Type of the item</param>
    /// <param name="obj">Item</param>
    /// <param name="autoIncrementColumn"></param>
    /// <param name="excludedColumns"></param>
    private void InternalInsert(object obj, string tableName, string autoIncrementColumn, string[] excludedColumns)
    {
        CheckInitialization(obj.GetType(), out ITableDefinition tableDefinition);

        switch (tableDefinition.ObjectType)
        {
            case ObjectType.LazyObject:
                InternalInsertLazy(tableDefinition, obj);
                break;
            case ObjectType.Object:
                InternalInsertObject(tableDefinition, obj, tableName, autoIncrementColumn, excludedColumns);
                break;
            default:
                InternalInsertDynamic(tableDefinition, obj, tableName, autoIncrementColumn, excludedColumns);
                break;
        }
    }


    private void InternalInsertLazy(ITableDefinition tableDefinition, object obj)
    {
        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _,
            out IReadOnlyList<ColumnDefinition> primaryKeys);

        ColumnDefinition autoIncrementColumn = primaryKeys.FirstOrDefault(c => c.PrimaryKey.AutoIncrement);

        QueryBuilder queryBuilder = new(tableDefinition);
        queryBuilder.Append($"INSERT INTO {tableDefinition.GetTableName()}");

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

    private void InternalInsertObject(ITableDefinition tableDefinition, object obj, string tableName, string autoIncrementColumn, string[] excludedColumns)
    {
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
        
        QueryBuilder queryBuilder = new(tableDefinition);
        queryBuilder.Append($"INSERT INTO {tableDefinition.GetTableName(tableName)}");

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

    private void InternalInsertDynamic(ITableDefinition tableDefinition, object obj, string tableName, string autoIncrementColumn, string[] excludedColumns)
    {
        if (tableName == null)
            throw new LazySqlException($"{nameof(tableName)} cannot be null for dynamic object");

        List<PropertyInfo> allProperties = obj.GetType().GetProperties().ToList();

        PropertyInfo autoColumnDefinition = null;
        if (!string.IsNullOrWhiteSpace(autoIncrementColumn))
        {
            autoColumnDefinition = allProperties.First(c =>
                string.Equals(c.Name, autoIncrementColumn, StringComparison.InvariantCultureIgnoreCase));

            excludedColumns ??= Array.Empty<string>();
            excludedColumns = excludedColumns.Append(autoIncrementColumn).ToArray();
        }

        List<PropertyInfo> properties = excludedColumns == null
            ? allProperties.ToList()
            : allProperties.Where(column => excludedColumns.All(c =>
                !string.Equals(c, column.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();


        QueryBuilder queryBuilder = new(tableDefinition);
        queryBuilder.Append($"INSERT INTO {tableName}");

        queryBuilder.Append(
            $" ({string.Join(", ", properties.Select(c => c.Name))})");

        if (autoIncrementColumn != null)
            queryBuilder.Append($" output INSERTED.{autoIncrementColumn}");

        List<string> values = properties.Select(property =>
            queryBuilder.RegisterArgument(property.Name.GetType().ToSqlType(), property.GetValue(obj))).ToList();
        queryBuilder.Append($" VALUES ({string.Join(", ", values)})");

        if (autoColumnDefinition != null && autoColumnDefinition.CanWrite)
        {
            object output = ExecuteScalar(queryBuilder);
            object value = Convert.ChangeType(output, autoColumnDefinition.PropertyType);
            autoColumnDefinition.SetValue(this, value, null);
        }
        else
        {
            ExecuteNonQuery(queryBuilder);
        }
        
    }

    #endregion
}