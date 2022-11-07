using System;
using System.Linq.Expressions;
using System.Reflection;
using LazySql.Engine.Client.Lambda;
using LazySql.Engine.Client.Query;

namespace LazySql.Engine.Client.Functions
{
    public partial class LzFunctions : LambdaFunctionParser
    {
        internal override void Parse(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            MethodInfo method =  GetType().GetTypeInfo().GetDeclaredMethod($"Parse{expression.Method.Name}");
            if (method == null)
                throw new NotImplementedException($"Function 'Parse{expression.Method.Name}' is missing");
            method.Invoke(this, new object[] {expression, lambdaParser, builder});
        }
        public static bool Like(object obj, string value)
        {
            if (obj == null) return false;
            string valueToCompare = obj.ToString().ToLower();
            string pattern = value.ToLower();
            if (pattern[0] == '%' && pattern[pattern.Length - 1] == '%')
                return valueToCompare.Contains(pattern.Substring(1, pattern.Length - 2));
            if (pattern[0] == '%')
                return valueToCompare.StartsWith(pattern.Substring(1));
            if (pattern[pattern.Length - 1] == '%')
                return valueToCompare.EndsWith(pattern.Substring(0, pattern.Length - 2));
            return string.Equals(valueToCompare, pattern, StringComparison.CurrentCultureIgnoreCase);
        }
        private void ParseLike(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(" LIKE ");
            lambdaParser.ParseExpression(expression.Arguments[1]);
        }

        public static bool NotLike(object obj, string value)
        {
            throw new NotImplementedException();
        }

        private void ParseNotLike(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(" NOT LIKE ");
            lambdaParser.ParseExpression(expression.Arguments[1]);
        }
    }
}
