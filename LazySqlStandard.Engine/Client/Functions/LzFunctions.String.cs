namespace LazySql;

public partial class LzFunctions
{
    /// <summary>
    /// Sql: <code>ASCII(character_expression)</code>
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>ASCII Value</returns>
    public static int Ascii(string value) => value[0];

    /// <summary>
    /// Convert Ascii to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseAscii(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" ASCII(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>CHAR(character_expression)</code>
    /// </summary>
    /// <param name="b">Value</param>
    /// <returns>Char value</returns>
    public static string Char(byte b) => Encoding.ASCII.GetString(new[] {b});

    /// <summary>
    /// Convert Char to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseChar(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" CHAR(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>CHARINDEX(expressionToFind, expressionToSearch, start_location)</code>
    /// </summary>
    /// <param name="search">A character expression containing the sequence to find. expressionToFind has an 8000 character limit.</param>
    /// <param name="expression">A character expression to search.</param>
    /// <param name="startLocation">An integer or bigint expression at which the search starts. If start_location is not specified, has a negative value, or has a zero (0) value, the search starts at the beginning of expressionToSearch.</param>
    /// <returns>Returning the starting position of the first expression if found.</returns>
    public static int? CharIndex(string search, string expression, int? startLocation = 0)
    {
        if (search == null || expression == null || !startLocation.HasValue)
            return null;

        if (startLocation > 0)
            return expression.Substring(startLocation.Value).IndexOf(search, StringComparison.Ordinal);
        return expression.IndexOf(search, StringComparison.Ordinal);
    }

    /// <summary>
    /// Convert CharIndex to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseCharIndex(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" CHARINDEX(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(", ");
        lambdaParser.ParseExpression(expression.Arguments[1]);
        if (expression.Arguments.Count > 2)
        {
            queryBuilder.Append(", ");
            lambdaParser.ParseExpression(expression.Arguments[2]);
        }

        queryBuilder.Append(")");
    }

    /// <summary>
    /// Sql: <code>CONCAT(string_value1, string_value2, ...)</code>
    /// </summary>
    /// <param name="values">A string value to concatenate to the other values</param>
    /// <returns>A string value whose length and type depend on the input.</returns>
    public static string Concat(params string[] values) => string.Join(string.Empty, values);

    /// <summary>
    /// Convert Concat to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseConcat(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" CONCAT(");
        for (int i = 0; i < expression.Arguments.Count; i++)
        {
            lambdaParser.ParseExpression(expression.Arguments[i]);
            if (i + 1 < expression.Arguments.Count)
                queryBuilder.Append(", ");
        }

        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>CONCAT_WS(separator, argument1, argument2 [, argumentN]... )</code>
    /// </summary>
    /// <param name="separator">separator An expression of any character type (char, nchar, nvarchar, or varchar).</param>
    /// <param name="values">argument1, argument2, argumentN An expression of any type</param>
    /// <returns>A string value whose length and type depend on the input.</returns>
    public static string ConcatWs(string separator, params string[] values) => string.Join(separator, values);

    /// <summary>
    /// Convert ConcatWs to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    internal void ParseConcatWs(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" CONCAT_WS(");
        for (int i = 0; i < expression.Arguments.Count; i++)
        {
            lambdaParser.ParseExpression(expression.Arguments[i]);
            if (i + 1 < expression.Arguments.Count)
                queryBuilder.Append(", ");
        }

        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>DATALENGTH(expression)</code>
    /// </summary>
    /// <param name="obj">An expression of any data type</param>
    /// <returns>Length</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static int? DataLength(object obj) => throw new NotSupportedException();

    /// <summary>
    /// Convert DataLength to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseDataLength(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" DATALENGTH(");
        queryBuilder.Append(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }

    public static string Lower(string str) => str.ToLower();
    private void ParseLower(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" LOWER(");
        queryBuilder.Append(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }

    public static string Upper(string str) => str.ToUpper();
    private void ParseUpper(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" UPPER(");
        queryBuilder.Append(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }
}

