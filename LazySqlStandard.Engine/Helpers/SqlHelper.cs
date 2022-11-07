using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using LazySql.Engine.Enums;

namespace LazySql.Engine.Helpers
{
    internal static class SqlHelper
    {
        private static Dictionary<Type, SqlType> _map;

        static SqlHelper()
        {
            _map = new Dictionary<Type, SqlType>
            {
                [typeof(long)] = SqlType.BigInt,
                [typeof(byte[])] = SqlType.Binary,
                [typeof(bool)] = SqlType.Bit,
                [typeof(string)] = SqlType.NVarChar,
                [typeof(char[])] = SqlType.NVarChar,
                [typeof(DateTime)] = SqlType.DateTime,
                [typeof(DateTimeOffset)] = SqlType.DateTimeOffset,
                [typeof(byte)] = SqlType.TinyInt,
                [typeof(short)] = SqlType.SmallInt,
                [typeof(int)] = SqlType.Int,
                [typeof(decimal)] = SqlType.Money,
                [typeof(float)] = SqlType.Real,
                [typeof(double)] = SqlType.Float,
                [typeof(TimeSpan)] = SqlType.Time,
                [typeof(Guid)] = SqlType.UniqueIdentifier
            };

        }


        public static SqlType ToSqlType(this Type type)
        {
            Type typeToCheck = Nullable.GetUnderlyingType(type) ?? type;
            if (_map.ContainsKey(typeToCheck))
                return _map[typeToCheck];
            return SqlType.Default;
        }
    }
}
