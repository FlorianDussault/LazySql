namespace LazySql;

/// <summary>
/// SQL Functions
/// </summary>
public partial class LzFunctions : LambdaFunctionParser
{
    /// <summary>
    /// Entry Point
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">LambdaParser</param>
    /// <param name="queryBuilder">QueryBuilder</param>
    /// <exception cref="NotImplementedException"></exception>
    internal override void Parse(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        MethodInfo method =  GetType().GetTypeInfo().GetDeclaredMethod($"Parse{expression.Method.Name}");
        if (method == null)
            throw new NotImplementedException($"Function 'Parse{expression.Method.Name}' is missing");
        method.Invoke(this, new object[] {expression, lambdaParser, queryBuilder});
    }

    public static int Count(int i)
    {
        return - 1;
    }

    private void ParseCount(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" COUNT(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(")");
    }

    private void ParseMax(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" MAX(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(") AS ");
        if (expression.Arguments[1] is ConstantExpression constantExpression)
            queryBuilder.Append($"[{constantExpression.Value}] ");
        else
            throw new NotImplementedException();
    }


    /// <summary>
    /// Like function: <code>XXX LIKE '%value%'</code>
    /// </summary>
    /// <param name="obj">Object to compare</param>
    /// <param name="value">Value</param>
    /// <returns>true if find</returns>
    public static bool Like(object obj, string value)
    {
        if (obj == null) return false;
        string valueToCompare = obj.ToString()!.ToLower();
        string pattern = value.ToLower();
        if (pattern[0] == '%' && pattern[pattern.Length - 1] == '%')
            return valueToCompare.Contains(pattern.Substring(1, pattern.Length - 2));
        if (pattern[0] == '%')
            return valueToCompare.StartsWith(pattern.Substring(1));
        if (pattern[pattern.Length - 1] == '%')
            return valueToCompare.EndsWith(pattern.Substring(0, pattern.Length - 2));
        return string.Equals(valueToCompare, pattern, StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// Transform C# Like to SQL
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">LambdaParser</param>
    /// <param name="queryBuilder">QueryBuilder</param>
    private void ParseLike(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(" LIKE ");
        lambdaParser.ParseExpression(expression.Arguments[1]);
    }

    /// <summary>
    /// Not Like function: <code>XXX NOT LIKE '%value%'</code>
    /// </summary>
    /// <param name="obj">Object to compare</param>
    /// <param name="value">Value</param>
    /// <returns>true if not find</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool NotLike(object obj, string value)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Transform C# NotLike to SQL
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="lambdaParser"></param>
    /// <param name="queryBuilder"></param>
    private void ParseNotLike(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(" NOT LIKE ");
        lambdaParser.ParseExpression(expression.Arguments[1]);
    }
}