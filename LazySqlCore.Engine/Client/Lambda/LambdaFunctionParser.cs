using System.Linq.Expressions;
using LazySql.Engine.Client.Query;

namespace LazySql.Engine.Client.Lambda
{
    public abstract class LambdaFunctionParser
    {
        internal virtual void Parse(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder builder)
        {

        }
    }
}
