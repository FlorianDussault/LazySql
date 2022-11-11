namespace LazySql.Engine.Client.Lambda;

public abstract class LambdaFunctionParser
{
    internal virtual void Parse(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {

    }
}