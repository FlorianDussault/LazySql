namespace LazySql;

/// <summary>
/// C# Parser to SQL
/// </summary>
internal partial class LzCSharpFunctions
{
    /// <summary>
    /// String.Join
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">QueryBuilder</param>
    private void String_Join(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder) => new LzFunctions().ParseConcatWs(expression, lambdaParser, queryBuilder);

    /// <summary>
    /// String.Format or $
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">QueryBuilder</param>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void String_Format(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        Regex regex = new ("({[0-9]*})");
        if (!lambdaParser.GetValueFromExpression(expression.Arguments[0], out object objFormat))
            throw new NotSupportedException("Cannot extract value of " + expression.Arguments[0]);

        string[] values = regex.Split(objFormat.ToString()!).Where(x => !string.IsNullOrEmpty(x)).ToArray();
        queryBuilder.Append(" CONCAT(");
        for (int i = 0; i < values.Length; i++)
        {
            string value = values[i];

            if (regex.IsMatch(value))
            {
                int index = int.Parse(value.Substring(1, value.Length - 2)) + 1;
                if (index >= expression.Arguments.Count)
                    throw new ArgumentOutOfRangeException();
                if (lambdaParser.GetValueFromExpression(expression.Arguments[index], out object argObject))
                {
                    string argumentName = queryBuilder.RegisterArgument(argObject.GetType().ToSqlType(), argObject);
                    queryBuilder.Append(argumentName);
                }
                else
                {
                    lambdaParser.ParseExpression(expression.Arguments[index]);
                }
            }
            else
            {
                string argumentName = queryBuilder.RegisterArgument(SqlType.NVarChar, value);
                queryBuilder.Append(argumentName);
            }

            if (i + 1 < values.Length)
                queryBuilder.Append(", ");
        }

        queryBuilder.Append(")");
            
    }
}