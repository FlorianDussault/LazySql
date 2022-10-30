using LazySql.Engine.Definitions;
using System;
using System.Linq;
using System.Linq.Expressions;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Enums;
using System.Reflection;
using LazySql.Engine.Client.Functions;

namespace LazySql.Engine.Client.Lambda
{
    internal class LambdaParser
    {
        private readonly Expression _expression;
        private readonly Type _type;
        private readonly object _object;
        private readonly TableDefinition _tableDefinition;
        private readonly TableDefinition _parentTableDefinition;
        private readonly  QueryBuilder _queryBuilder;

        private LambdaParser(Expression expression, TableDefinition tableDefinition, QueryBuilder queryBuilder, Type type = null, object obj = null)
        {
            _expression = expression;
            _tableDefinition = tableDefinition;
            _queryBuilder = queryBuilder;
            _type = type;
            _object = obj;
            if (_type != null)
                SqlClient.CheckInitialization(_type, out _parentTableDefinition);
            ParseExpression(_expression);
        }

        internal static void Parse(Expression expression, TableDefinition tableDefinition, QueryBuilder queryBuilder, Type type = null, object obj = null) => new LambdaParser(expression, tableDefinition, queryBuilder, type, obj);

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
                default:
                    var a = expression.GetType();
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ParseLambda(LambdaExpression expression) => ParseExpression(expression.Body);

        private void ParseUnary(UnaryExpression expression) => ParseExpression(expression.Operand);

        private void ParseBinary(BinaryExpression expression)
        {
            ParseExpression(expression.Left);
            ParseNodeType(expression.NodeType);
            ParseExpression(expression.Right);
        }

        private void ParseMember(MemberExpression expression)
        {
            if (_type != null && expression.Member.DeclaringType == _type)
            {
                // Find Parent
                PropertyInfo propertyInfo = ((PropertyInfo) expression.Member);
                object value = propertyInfo.GetValue(_object);

                

                string argumentName = _queryBuilder.RegisterArgument(_parentTableDefinition.GetColumn(propertyInfo.Name).Column.SqlType, value);
                _queryBuilder.Append(argumentName);
                return;
            }

            ColumnDefinition columnDefinition = _tableDefinition.GetColumn(expression.Member.Name);
            if (columnDefinition == null)
            {
                throw new NotImplementedException();
                UnaryExpression objectMember = Expression.Convert(expression, typeof(object));
                ParseUnary(objectMember);
            }
            else
            {
                _queryBuilder.Append($" {columnDefinition.Column.SqlColumnName}");
            }
        }

        private void ParseConstant(ConstantExpression expression)
        {
            string argument = _queryBuilder.RegisterArgument(SqlType.Default, expression.Value);
            _queryBuilder.Append(argument);
            //Append($" {_sqlArguments.Register(SqlType.Default, constantExpression.Value)}");
        }

        private void ParseMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.DeclaringType.IsSubclassOf(typeof(LambdaFunctionParser)))
            {
                LambdaFunctionParser lambdaFunctionParser = (LambdaFunctionParser)Activator.CreateInstance(expression.Method.DeclaringType);
                lambdaFunctionParser.Parse(expression, this, _queryBuilder);
                return;
            }

            throw new NotImplementedException();

            ParseExpression(expression.Arguments[0]);



            if (expression.Method.DeclaringType == typeof(LzFunctions))
            {

            }

            ParseExpression(expression.Arguments[1]);
        }

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
    }
}
