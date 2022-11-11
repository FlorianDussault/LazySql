using System.Globalization;

namespace LazySql.Engine.Client.Functions;

public partial class LzFunctions
{
    /// <summary>
    /// Sql: <code>ISDATE(expression)</code>
    /// </summary>
    /// <param name="value">Date value</param>
    /// <returns>True if the value is a date</returns>
    public static int IsDate(string value) => DateTime.TryParse(value, out _) ? 1 : 0;

    /// <summary>
    /// Convert IsDate to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseIsDate(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" ISDATE(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>GETDATE()</code>
    /// </summary>
    /// <returns>Current Date</returns>
    public static string GetDate() => DateTime.Now.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Convert GetDate to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseGetDate(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder) => queryBuilder.Append(" GETDATE() ");

    /// <summary>
    /// Sql: <code>DAY(date)</code>
    /// </summary>
    /// <param name="obj">value</param>
    /// <returns>Day of the date</returns>
    public static int Day(object obj) => ((DateTime)obj).Day;

    /// <summary>
    /// Convert Day to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseDay(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" DAY(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>MONTH(date)</code>
    /// </summary>
    /// <param name="obj">Value</param>
    /// <returns>Month of the date</returns>
    public static int Month(object obj) => ((DateTime)obj).Month;

    /// <summary>
    /// Convert Month to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseMonth(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" MONTH(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>YEAR(date)</code>
    /// </summary>
    /// <param name="obj">Value</param>
    /// <returns>Month of the date</returns>
    public static int Year(object obj) => ((DateTime)obj).Year;

    /// <summary>
    /// Convert Year to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseYear(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" YEAR(");
        lambdaParser.ParseExpression(expression.Arguments[0]);
        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>ATEADD(datepart, number, date)</code>D
    /// </summary>
    /// <param name="lzDatePart">DatePart</param>
    /// <param name="increment">Increment</param>
    /// <param name="obj">Date</param>
    /// <returns>Date</returns>
    public static string DateAdd(LzDatePart lzDatePart, int increment, object obj) => DateTime.Now.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Convert DateAdd to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    private void ParseDateAdd(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" DATEADD(");
        if (!lambdaParser.GetValueFromExpression(expression.Arguments[0], out object arg1) ||
            arg1 is not LzDatePart datePart)
            throw new NotImplementedException();
        queryBuilder.Append($"{Enum.GetName(typeof(LzDatePart), datePart)}, ");
        queryBuilder.Append(expression.Arguments[1]);
        queryBuilder.Append(", ");
        queryBuilder.Append(expression.Arguments[2]);
        queryBuilder.Append(") ");
    }

    /// <summary>
    /// Sql: <code>DATEDIFF(datepart, startdate, enddate)</code>
    /// </summary>
    /// <param name="lzDatePart">DatePart</param>
    /// <param name="start">Date Start</param>
    /// <param name="end">Date End</param>
    /// <returns>Difference</returns>
    public static int DateDiff(LzDatePart lzDatePart, DateTime start, DateTime end) => (end - start).Days;

    /// <summary>
    /// Convert DateDiff to Sql
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="lambdaParser">Lambda Parser</param>
    /// <param name="queryBuilder">Query Builder</param>
    /// <exception cref="NotImplementedException"></exception>
    private void ParseDateDiff(MethodCallExpression expression, LambdaParser lambdaParser, QueryBuilder queryBuilder)
    {
        queryBuilder.Append(" DATEDIFF(");
        if (!lambdaParser.GetValueFromExpression(expression.Arguments[0], out object arg1) ||
            arg1 is not LzDatePart datePart)
            throw new NotImplementedException();
        queryBuilder.Append($"{Enum.GetName(typeof(LzDatePart), datePart)}, ");
        queryBuilder.Append(expression.Arguments[1]);
        queryBuilder.Append(", ");
        queryBuilder.Append(expression.Arguments[2]);
        queryBuilder.Append(") ");
    }
}