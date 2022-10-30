using System.Linq.Expressions;
using LazySql.Engine.Client.Lambda;
using LazySql.Engine.Client.Query;

namespace LazySql.Engine.Client.Functions
{
    public class LzFunctions : LambdaFunctionParser
    {
        public static bool Like(object obj, string value)
        {
            return false;
        }

        internal override void Parse(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            switch (expression.Method.Name)
            {
                case nameof(Like):
                    ParseKike(expression, lambdaParser, builder);
                    break;
            }
        }


        private void ParseKike(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {
            lambdaParser.ParseExpression(expression.Arguments[0]);
            builder.Append(" LIKE ");
            lambdaParser.ParseExpression(expression.Arguments[1]);
        }
    }
}
