namespace LazySql.Engine.Client.Query;

/// <summary>
/// Query Builder
/// </summary>
internal sealed class QueryBuilder
{
    private readonly StringBuilder _stringBuilder = new StringBuilder();
    private readonly SqlArguments _sqlArguments = new SqlArguments();
    private readonly TableDefinition _tableDefinition;

    public QueryBuilder(TableDefinition tableDefinition)
    {
        _tableDefinition = tableDefinition;
    }

    /// <summary>
    /// Get Sql Query
    /// </summary>
    /// <returns></returns>
    public string GetQuery() => _stringBuilder.ToString();

    /// <summary>
    /// Get Arguments
    /// </summary>
    /// <returns></returns>
    public SqlArguments GetArguments() => _sqlArguments;

    /// <summary>
    /// Get Current Table Definition
    /// </summary>
    /// <returns></returns>
    public TableDefinition GetTableDefinition() => _tableDefinition;

    /// <summary>
    /// Append Sql
    /// </summary>
    /// <param name="sql">Sql</param>
    /// <param name="expression">Expression</param>
    public void Append(string sql = null, LambdaExpression expression = null)
    {
        if (sql != null)
            _stringBuilder.Append(sql);
        Append(expression);
    }

    /// <summary>
    /// Append expression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="type">Type of object</param>
    /// <param name="obj">object</param>
    public void Append(Expression expression = null, Type type = null, object obj = null)
    {
        if (expression == null) return;
        LambdaParser.Parse(expression, _tableDefinition, this, type, obj);
    }

    /// <summary>
    /// Register new argument
    /// </summary>
    /// <param name="type">Sql Type of argument</param>
    /// <param name="obj">Value</param>
    /// <returns>SQL Variable name</returns>
    public string RegisterArgument(SqlType type, object obj) => _sqlArguments.Register(type, obj);

}