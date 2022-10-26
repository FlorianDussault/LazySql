using System;
using LazySql.Engine.Enums;

namespace LazySql.Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LazyColumn : Attribute
    {
        internal string ColumnName { get; private set; }
        internal string SqlColumnName => ColumnName.ToSqlColumn() ?? throw new InvalidOperationException();
        internal SqlType SqlType { get; }
        public LazyColumn(string columnName, SqlType sqlType)
        {
            ColumnName = columnName;
            SqlType = sqlType;
        }

        public LazyColumn(SqlType sqlType)
        {
            SqlType = sqlType;
        }

        internal void SetColumnName(string columnName)
        {
            ColumnName = columnName;
        }
    }
}