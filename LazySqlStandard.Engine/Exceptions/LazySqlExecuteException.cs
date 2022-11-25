#if NETCORE
using Microsoft.Data.SqlClient;
#elif NETSTANDARD
#else
#endif

namespace LazySql
{
    public class LazySqlExecuteException : LazySqlException
    {
        public string DebugQuery { get; private set; }
        
        public LazySqlExecuteException(Exception ex, string message) : base(message, ex)
        {
        }

        public LazySqlExecuteException(string message) : base(message)
        {
        }

        internal static LazySqlExecuteException Generate(Exception ex, string query, SqlParameterCollection sqlArguments)
        {
            string debugQuery = query;
            foreach (SqlParameter sqlArgument in sqlArguments){
                if (sqlArgument.Value == DBNull.Value)
                {
                    debugQuery = debugQuery.Replace(sqlArgument.ParameterName, "NULL");
                }
                else
                {
                    debugQuery = debugQuery.Replace(sqlArgument.ParameterName, $"'{sqlArgument.Value?.ToString()}'");
                }
            }

            LazySqlExecuteException exception = new($"'{ex.Message} QUERY INFORMATION: {debugQuery}");
            exception.DebugQuery = debugQuery;
            return exception;
        }

        public static LazySqlExecuteException Generate(Exception exception, string message)
        {
            throw new LazySqlExecuteException(exception, message);
        }
    }
}
