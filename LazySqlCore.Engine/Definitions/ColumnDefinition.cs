using System;
using System.Reflection;
using LazySql.Engine.Attributes;

namespace LazySql.Engine.Definitions
{
    internal sealed class ColumnDefinition
    {
        public PropertyInfo PropertyInfo { get; }
        public LazyColumn Column { get; }

        public PrimaryKey PrimaryKey { get; }

        public int Index { get; }

        public ColumnDefinition(PropertyInfo propertyInfo, LazyColumn column, int index, PrimaryKey primaryKey)
        {
            PropertyInfo = propertyInfo;
            Column = column;
            Index = index;
            PrimaryKey = primaryKey;
        }

        public object GetValue(object obj) => PropertyInfo.GetValue(obj, null );
    }
}