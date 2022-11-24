namespace LazySql.Engine.Client.Query;

/// <summary>
/// Query Builder
/// </summary>
internal sealed class QueryBuilder
{
    private readonly StringBuilder _stringBuilder = new();
    private readonly SqlArguments _sqlArguments = new();
    private readonly ITableDefinition _tableDefinition;

    public QueryBuilder(ITableDefinition tableDefinition)
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
    public ITableDefinition GetTableDefinition() => _tableDefinition;

    /// <summary>
    /// Append Sql
    /// </summary>
    /// <param name="sql">Sql</param>
    /// <param name="expression">Expression</param>
    public void Append(string sql = null, Expression expression = null)
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
    /// Append expression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="type">Type of object</param>
    /// <param name="obj">object</param>
    /// <param name="alias1"></param>
    /// <param name="alias2"></param>
    public void AppendWithAliases(Expression expression, LambdaAlias alias1, LambdaAlias alias2)
    {
        if (expression == null) return;
        LambdaAliasParser.Parse(expression, this, alias1, alias2);
    }

    /// <summary>
    /// Register new argument
    /// </summary>
    /// <param name="type">Sql Type of argument</param>
    /// <param name="obj">Value</param>
    /// <returns>SQL Variable name</returns>
    public string RegisterArgument(SqlType type, object obj) => _sqlArguments.Register(type, obj);
    
    /// <summary>
    /// Add SqlArgument
    /// </summary>
    /// <param name="argument"></param>
    public void AddSqlArgument(SqlArgument argument) => _sqlArguments.Add(argument);

    /// <summary>
    /// Add SqlArguments
    /// </summary>
    /// <param name="arguments"></param>
    public void AddSqlArguments(SqlArguments arguments)
    {
        if (arguments == null) return;
        _sqlArguments.AddRange(arguments);
    }
}