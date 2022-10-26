using System.Collections.Generic;
using LazySql.Engine.Enums;

namespace LazySql.Engine.Client.Query
{
    internal sealed class SqlArguments : List<SqlArgument>
    {
        internal string Register(SqlType type,  object obj)
        {
            string argumentName = $"@_LZ_{Count}_LZ_@";
            Add(new SqlArgument(argumentName, type, obj));
            return argumentName;
        }
    }
}