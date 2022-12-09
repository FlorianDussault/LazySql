namespace LazySql;

/// <summary>
/// Lambda Parser
/// </summary>
internal class LambdaParser
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    protected Expression _expression;
    private readonly Type _type;
    private readonly object _object;
    private readonly ITableDefinition _tableDefinition;
    private readonly ITableDefinition _parentTableDefinition;
    protected readonly  QueryBuilder _queryBuilder;

    protected LambdaParser(Expression expression, ITableDefinition tableDefinition, QueryBuilder queryBuilder, Type type = null, object obj = null)
    {
        _expression = expression;
        _tableDefinition = tableDefinition;
        _queryBuilder = queryBuilder;
        _type = type;
        _object = obj;

        if (_type != null)
            LazyClient.CheckInitialization(_type, out _parentTableDefinition);
        //ParseExpression(_expression);
    }

    /// <summary>
    /// Parse an expression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="tableDefinition">Table Definition</param>
    /// <param name="queryBuilder">Query Builder</param>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    // ReSharper disable once ObjectCreationAsStatement
    internal static void Parse(Expression expression, ITableDefinition tableDefinition, QueryBuilder queryBuilder, Type type = null, object obj = null) => new LambdaParser(expression, tableDefinition, queryBuilder, type, obj).ParseExpression(expression);

    
    /// <summary>
    /// Parse Expression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal void ParseExpression(Expression expression)
    {
        switch (expression)
        {
            case LambdaExpression lambdaExpression:
                ParseLambda(lambdaExpression);
                break;
            case UnaryExpression unaryExpression:
                ParseUnary(unaryExpression);
                break;
            case BinaryExpression binaryExpression:
                ParseBinary(binaryExpression);
                break;
            case MemberExpression memberExpression:
                ParseMember(memberExpression);
                break;
            case ConstantExpression constantExpression:
                ParseConstant(constantExpression);
                break;
            case MethodCallExpression methodCallExpression:
                ParseMethodCall(methodCallExpression);
                break;
            case NewArrayExpression newArrayExpression:
                ParseNewArrayExpression(newArrayExpression);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Parse LambdaExpression
    /// </summary>
    /// <param name="expression">Expression</param>
    private void ParseLambda(LambdaExpression expression) => ParseExpression(expression.Body);

    /// <summary>
    /// Parse UnaryExpression
    /// </summary>
    /// <param name="expression">Expression</param>
    private void ParseUnary(UnaryExpression expression) => ParseExpression(expression.Operand);

    /// <summary>
    /// Parse BinaryExpression
    /// </summary>
    /// <param name="expression">Expression</param>
    private void ParseBinary(BinaryExpression expression)
    {
        if (ParseNullEquality(expression)) return;
            
        ParseExpression(expression.Left);
        ParseNodeType(expression.NodeType);
        ParseExpression(expression.Right);
    }

    /// <summary>
    /// Parse BinaryExpression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>True is null</returns>
    private bool ParseNullEquality(BinaryExpression expression)
    {
        if (expression.NodeType != ExpressionType.Equal && expression.NodeType != ExpressionType.NotEqual)
            return false;

        bool isLeftNull = IsValueExpressionNull(expression.Left);
        bool isRightNull = IsValueExpressionNull(expression.Right);

        if (!isLeftNull && !isRightNull) return false;

        string nullQuery = expression.NodeType == ExpressionType.Equal ? " IS NULL " : " IS NOT NULL ";

        // null found in expression

        if (isRightNull == isLeftNull)
        {
            // null == null
            _queryBuilder.Append($" NULL {nullQuery} ");
            return true;
        }

        if (isLeftNull)
        {
            ParseExpression(expression.Right);
            _queryBuilder.Append(nullQuery);
            return true;
        }
        ParseExpression(expression.Left);
        _queryBuilder.Append(nullQuery);
        return true;
    }

    /// <summary>
    /// Parse MemberExpression
    /// </summary>
    /// <param name="expression">Expression</param>
    internal virtual void ParseMember(MemberExpression expression)
    {
        if (_type != null && expression.Member.DeclaringType == _type)
        {
            // Find Parent
            PropertyInfo propertyInfo = (PropertyInfo) expression.Member;
            string argumentName = _queryBuilder.RegisterArgument(_parentTableDefinition.GetColumn(propertyInfo.Name).Column.SqlType, ParseMemberValue(expression));
            _queryBuilder.Append(argumentName);
            return;
        }

        if (expression.Member.DeclaringType == _tableDefinition.TableType && _tableDefinition.GetColumn(expression.Member.Name) is { } columnDefinition)
        {
            _queryBuilder.Append($" {columnDefinition.Column.SqlColumnName}");
        }
        else
        {
            object value = Expression.Lambda(expression).Compile().DynamicInvoke();
            string argumentName = _queryBuilder.RegisterArgument(value.GetType().ToSqlType(), value);
            _queryBuilder.Append(argumentName);
        }
       
    }

    /// <summary>
    /// Parse ConstantExpression
    /// </summary>
    /// <param name="expression">Expression</param>
    private void ParseConstant(ConstantExpression expression)
    {
        string argument = _queryBuilder.RegisterArgument(expression.Type.ToSqlType(), expression.Value);
        _queryBuilder.Append(argument);
    }

    /// <summary>
    /// Parse MethodCallExpression
    /// </summary>
    /// <param name="expression">Expression</param>
    private void ParseMethodCall(MethodCallExpression expression)
    {
        LambdaFunctionParser lambdaFunctionParser;
        if (expression.Method.DeclaringType!.IsSubclassOf(typeof(LambdaFunctionParser)))
            lambdaFunctionParser = (LambdaFunctionParser)Activator.CreateInstance(expression.Method.DeclaringType);
        else
            lambdaFunctionParser  = (LambdaFunctionParser)Activator.CreateInstance(typeof(LzCSharpFunctions));
        lambdaFunctionParser.Parse(expression, this, _queryBuilder);
    }

    /// <summary>
    /// Parse NewArrayExpression
    /// </summary>
    /// <param name="newArrayExpression"></param>
    private void ParseNewArrayExpression(NewArrayExpression newArrayExpression)
    {
        for (int i = 0; i < newArrayExpression.Expressions.Count; i++)
        {
            ParseExpression(newArrayExpression.Expressions[i]);
            if (i + 1 < newArrayExpression.Expressions.Count)
                _queryBuilder.Append(", ");
        }
    }

    /// <summary>
    /// Parse ExpressionType
    /// </summary>
    /// <param name="expressionType"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ParseNodeType(ExpressionType expressionType)
    {
        switch (expressionType)
        {
            case ExpressionType.Add:
                _queryBuilder.Append(" + ");
                break;
            case ExpressionType.And:
                _queryBuilder.Append(" AND ");
                break;
            case ExpressionType.AndAlso:
                _queryBuilder.Append(" AND ");
                break;
            case ExpressionType.Divide:
                _queryBuilder.Append(" / ");
                break;
            case ExpressionType.Equal:
                _queryBuilder.Append(" = ");
                break;
            case ExpressionType.ExclusiveOr:
                _queryBuilder.Append(" ^ ");
                break;
            case ExpressionType.GreaterThan:
                _queryBuilder.Append(" > ");
                break;
            case ExpressionType.GreaterThanOrEqual:
                _queryBuilder.Append(" >= ");
                break;
            case ExpressionType.LessThan:
                _queryBuilder.Append(" < ");
                break;
            case ExpressionType.LessThanOrEqual:
                _queryBuilder.Append(" <= ");
                break;
            case ExpressionType.Multiply:
                _queryBuilder.Append(" * ");
                break;
            case ExpressionType.NotEqual:
                _queryBuilder.Append(" != ");
                break;
            case ExpressionType.Or:
                _queryBuilder.Append(" OR ");
                break;
            case ExpressionType.OrElse:
                _queryBuilder.Append(" OR ");
                break;
            case ExpressionType.Subtract:
                _queryBuilder.Append(" - ");
                break;
            case ExpressionType.Decrement:
                _queryBuilder.Append(" -1 ");
                break;
            case ExpressionType.Increment:
                _queryBuilder.Append(" +1 ");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(expressionType), expressionType, null);
        }
    }

    /// <summary>
    /// Get value of MemberExpression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>Return Value</returns>
    private object ParseMemberValue(MemberExpression expression)
    {
        PropertyInfo propertyInfo = (PropertyInfo)expression.Member;
        return propertyInfo.GetValue(_object);
    }

    /// <summary>
    /// Get value from expression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="obj">Value</param>
    /// <returns>Return true if value found</returns>
    internal bool GetValueFromExpression(Expression expression, out object obj)
    {
        if (expression is ConstantExpression constantExpression)
            obj = ParseConstantValue(constantExpression);
        else if (expression is MemberExpression memberExpression && _type != null &&
                 memberExpression.Member.DeclaringType == _type)
            obj = ParseMemberValue(memberExpression);
        else if (expression is UnaryExpression unaryExpression)
        {
            return GetValueFromExpression(unaryExpression.Operand, out obj);
        }
        else
        {
            obj = null;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Check if value in expression is null
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>Return null if value null</returns>
    private bool IsValueExpressionNull(Expression expression)
    {
        if (expression is ConstantExpression constantExpression)
            return ParseConstantValue(constantExpression) == null;
        if (expression is MemberExpression memberExpression && _type != null && memberExpression.Member.DeclaringType == _type)
            return ParseMemberValue(memberExpression) == null;
        return false;
    }

    /// <summary>
    /// Get constant value
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>Value</returns>
    private object ParseConstantValue(ConstantExpression expression) => expression.Value;
}