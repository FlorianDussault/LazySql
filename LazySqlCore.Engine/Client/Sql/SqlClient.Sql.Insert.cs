using System.Collections.Generic;
using System.Linq;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Definitions;
using LazySql.Engine.Enums;

namespace LazySql.Engine.Client
{
    // ReSharper disable once ClassCannotBeInstantiated
    public sealed partial class SqlClient
    {
        #region Insert

        public static void Insert<T>(T obj) where T : LazyBase => Instance.InternalInsert<T>(obj);

        private void InternalInsert<T>(T obj) where T : LazyBase
        {
            CheckInitialization<T>(out TableDefinition tableDefinition);
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
                object output = ExecuteScalar<T>(queryBuilder);
                autoIncrementColumn.PropertyInfo.SetValue(obj, output);
            }
            else
            {
                ExecuteNonQuery(queryBuilder);
            }
        }

        #endregion
    }
}