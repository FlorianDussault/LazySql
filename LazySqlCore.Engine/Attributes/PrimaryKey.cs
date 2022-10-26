using System;

namespace LazySql.Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrimaryKey : Attribute
    {
        internal bool AutoIncrement {get;}
        public PrimaryKey(bool autoIncrement)
        {
            AutoIncrement = autoIncrement;
        }
    }
}