using System;
using LazySql.Engine.Enums;

namespace LazySql.Engine.Attributes
{
    /// <summary>
    /// Attribute for a column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LazyColumn : Attribute
    {
        /// <summary>
        /// Column name in SQL
        /// </summary>
        internal string ColumnName { get; private set; }

        /// <summary>
        /// SQL Name
        /// </summary>
        internal string SqlColumnName => ColumnName.ToSqlColumn() ?? throw new InvalidOperationException();

        /// <summary>
        /// Type of column
        /// </summary>
        internal SqlType SqlType { get; }

        /// <summary>
        /// LazyColumn
        /// </summary>
        /// <param name="columnName">Column name in SQL</param>
        /// <param name="sqlType">SQL Type</param>
        public LazyColumn(string columnName, SqlType sqlType)
        {
            ColumnName = columnName;
            SqlType = sqlType;
        }

        /// <summary>
        /// LazyColumn (property name exactly the same in SQL)
        /// </summary>
        /// <param name="sqlType">SQL Type</param>
        public LazyColumn(SqlType sqlType) => SqlType = sqlType;

        /// <summary>
        /// Set column name
        /// </summary>
        /// <param name="columnName"></param>
        internal void SetColumnName(string columnName) => ColumnName = columnName;
    }
}