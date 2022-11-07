using LazySql.Engine.Client.Lambda;
using LazySql.Engine.Client.Query;
using System;
using System.Linq.Expressions;
using System.Text;

namespace LazySql.Engine.Client.Functions
{
    public partial class LzFunctions : LambdaFunctionParser
    {
        public static int Ascii(string value)
        {
            return value[0];
        }

        private void ParseAscii(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" ASCII(");
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(") ");
        }

        public static string Char(byte b)
        {
            return Encoding.ASCII.GetString(new[] {b});
        }

        private void ParseChar(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" CHAR(");
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(") ");
        }

        public static int? CharIndex(string search, string expression, int? startLocation = 0)
        {
            if (search == null || expression == null || !startLocation.HasValue)
                return null;

            if (startLocation > 0)
                return expression.Substring(startLocation.Value).IndexOf(search, StringComparison.Ordinal);
            return expression.IndexOf(search, StringComparison.Ordinal);
        }

        private void ParseCharIndex(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" CHARINDEX(");
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(", ");
            lambdaParser.ParseExpression(expression.Arguments[1]);
            if (expression.Arguments.Count > 2)
            {
                builder.Append(", ");
                lambdaParser.ParseExpression(expression.Arguments[2]);
            }

            builder.Append(")");
        }

        public static string Concat(params string[] values)
        {
            return string.Join(string.Empty, values);
        }

        private void ParseConcat(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" CONCAT(");
            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                lambdaParser.ParseExpression(expression.Arguments[i]);
                if (i + 1 < expression.Arguments.Count)
                    builder.Append(", ");
            }

            builder.Append(") ");
        }

        public static string ConcatWs(string separator, params string[] values)
        {
            return string.Join(separator, values);
        }

        internal void ParseConcatWs(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" CONCAT_WS(");
            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                lambdaParser.ParseExpression(expression.Arguments[i]);
                if (i + 1 < expression.Arguments.Count)
                    builder.Append(", ");
            }

            builder.Append(") ");
        }

        public static int DataLength(object? obj)
        {
            throw new NotSupportedException();
        }

        private void ParseDataLength(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            builder.Append(" DATALENGTH(");
            builder.Append(expression.Arguments[0]);
            builder.Append(") ");
        }
    }
}
