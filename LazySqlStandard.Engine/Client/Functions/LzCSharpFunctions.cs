namespace LazySql;

/// <summary>
/// C# Parser to SQL
/// </summary>
internal partial class LzCSharpFunctions : LambdaFunctionParser
{
    /// <summary>
    /// Entry Point
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">LambdaParser</param>
    /// <param name="queryBuilder">QueryBuilder</param>
    /// <exception cref="NotSupportedException"></exception>
    internal override void Parse(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        MethodInfo method = GetType().GetTypeInfo()
            .GetDeclaredMethod($"{expression.Method.DeclaringType.Name}_{expression.Method.Name}");

        if (method != null)
            method.Invoke(this, new object[] { expression, lambdaParser, queryBuilder });
        else
        {
            Other(expression, lambdaParser, queryBuilder);
        }
       
    }
}