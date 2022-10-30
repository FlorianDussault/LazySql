using System;

namespace LazySql.Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LazyTable : Attribute
    {
        public string TableName { get; }

        public LazyTable(string tableName)
        {
            TableName = tableName;
        }
    }
}