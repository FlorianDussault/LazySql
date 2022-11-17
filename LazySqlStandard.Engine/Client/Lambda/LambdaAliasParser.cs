using LazySql.Engine.Client.Query;
using LazySql.Engine.Definitions;
using System.Linq.Expressions;

namespace LazySql.Engine.Client.Lambda;

internal sealed class LambdaAliasParser : LambdaParser
{
    private readonly Expression _expression;
    private readonly LambdaAlias _alias1;
    private readonly LambdaAlias _alias2;
    private string _parameter1;
    private string _parameter2;
    public LambdaAliasParser(Expression expression, QueryBuilder queryBuilder, LambdaAlias alias1, LambdaAlias alias2) : base(expression, null, queryBuilder, null)
    {
        _expression = expression;
        _alias1 = alias1;
        _alias2 = alias2;
        if (expression is not LambdaExpression lambdaExpression)
            throw new LazySqlException($"The following expression is not a {nameof(LambdaExpression)}: {expression}");

        _parameter1 = lambdaExpression.Parameters[0].Name;
        _parameter2 = lambdaExpression.Parameters[1].Name;
    }

    internal static void Parse(Expression expression, QueryBuilder queryBuilder, LambdaAlias alias1, LambdaAlias alias2) => new LambdaAliasParser(expression, queryBuilder, alias1, alias2).ParseExpression(expression);

    internal override void ParseMember(MemberExpression expression)
    {
        // new System.Linq.Expressions.Expression.ParameterExpressionProxy(new System.Linq.Expressions.Expression.MemberExpressionProxy(expression).Expression).Name
        if (expression.Expression is not ParameterExpression parameterExpression)
            throw new LazySqlException($"{nameof(ParameterExpression)} missing in: {expression}");

        LambdaAlias alias = null;
        if (parameterExpression.Name == _parameter1)
            alias = _alias1;
        else if (parameterExpression.Name == _parameter2)
            alias = _alias2;
        else
            throw new LazySqlException($"Alias {parameterExpression.Name} not found in: {expression}");

        ColumnDefinition columnDefinition = alias.TableDefinition.GetColumn(expression.Member.Name);
        if (columnDefinition == null)
            throw new LazySqlException($"No definition found for the column {expression.Member.Name} in: {expression}");
        _queryBuilder.Append($" {alias.SqlAlias}.{columnDefinition.Column.SqlColumnName} ");
    }

    internal void ParseParameter(ParameterExpression expression)
    {

    }
}

internal class LambdaAlias
{
    public string SqlAlias { get;  }

    public TableDefinition TableDefinition { get;  }


    public LambdaAlias(string sqlAlias, TableDefinition tableDefinition)
    {
        SqlAlias = sqlAlias;
        TableDefinition = tableDefinition;
    }
}