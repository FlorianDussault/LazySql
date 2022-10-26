using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using LazySql.Engine.Client.Functions;
using LazySql.Engine.Definitions;
using LazySql.Engine.Enums;

namespace LazySql.Engine.Client.Query
{
    internal sealed class QueryBuilder
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly SqlArguments _sqlArguments = new SqlArguments();
        private readonly TableDefinition _tableDefinition;

        public QueryBuilder(TableDefinition tableDefinition)
        {
            _tableDefinition = tableDefinition;
        }

        public string GetQuery() => _stringBuilder.ToString();
        public SqlArguments GetArguments() => _sqlArguments;
        public TableDefinition GetTableDefinition() => _tableDefinition;

        public void Append(string sql = null, LambdaExpression expression = null)
        {
            if (sql != null)
                _stringBuilder.Append(sql);
            Append(expression);
        }
    

        public void Append(LambdaExpression expression = null, Type type = null, object obj = null)
        {
            if (expression == null) return;
            if (!(expression.Body is UnaryExpression unaryExpression))
                throw new NotImplementedException();
            AnalyzeUnaryExpression(unaryExpression);
        }

        private void AnalyzeUnaryExpression(UnaryExpression unaryExpression, Type type = null, object obj = null)
        {
            if (unaryExpression.NodeType == ExpressionType.Convert)
            {
                // new System.Linq.Expressions.Expression.MethodCallExpressionProxy(new System.Linq.Expressions.Expression.UnaryExpressionProxy(unaryExpression).Operand).Method

            }

            if (unaryExpression.Operand is MethodCallExpression methodCallExpression)
            {
                AnalyzeMethodCallExpression(methodCallExpression);
                return;
            }

            if (!(unaryExpression.Operand is BinaryExpression binaryExpression))
                throw new NotImplementedException();
            AnalyzeOperand(binaryExpression, type, obj);
        }

        private void AnalyzeMethodCallExpression(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method.DeclaringType == typeof(LzFunctions))
            {
                if (methodCallExpression.Method.Name == nameof(LzFunctions.Like))
                {
                    if (methodCallExpression.Arguments[0] is UnaryExpression unaryExpression)
                    {
                        //AnalyzeUnaryExpression(unaryExpression);
                        AnalyzeSide(unaryExpression.Operand);

                    }

                    if (methodCallExpression.Arguments[1] is ConstantExpression unaryExpression2)
                    {
                        AnalyzeSide(methodCallExpression.Arguments[1]);
                        // new System.Linq.Expressions.Expression.ConstantExpressionProxy(new System.Collections.Generic.ICollectionDebugView<System.Linq.Expressions.Expression>(methodCallExpression.Arguments).Items[1]).Value
                        //AnalyzeSide(unaryExpression2.Operand);

                    }
                }
            }
        }

        public void Append(BinaryExpression expression, Type type = null, object obj = null)
        {
            AnalyzeOperand(expression, type, obj);
        }

        public string RegisterArgument(SqlType type, object obj) => _sqlArguments.Register(type, obj);

        private void AnalyzeOperand(BinaryExpression expression, Type type = null, object obj = null)
        {
            Append(" (");

            AnalyzeSide(expression.Left, type, obj);
            Append(AnalyzeNode(expression.NodeType));
            AnalyzeSide(expression.Right, type, obj);

            Append(")");
        }

        private void AnalyzeSide(Expression expression, Type type = null, object obj = null)
        {
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                    AnalyzeOperand(binaryExpression, type, obj);
                    break;
                case MemberExpression memberExpression:
                {
                    if (type != null && memberExpression.Member.DeclaringType == type)
                    {
                        AnalyzeMemberObject(memberExpression, type, obj);
                        return;
                    }

                    ColumnDefinition columnDefinition = _tableDefinition.GetColumn(memberExpression.Member.Name);

                    if (columnDefinition == null)
                    {
                        UnaryExpression objectMember = Expression.Convert(memberExpression, typeof(object));
                        Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);
                        Func<object> getter = getterLambda.Compile();

                        Append($" {_sqlArguments.Register(columnDefinition.Column.SqlType ,getter())}");
                    }
                    else
                        Append($" {columnDefinition.Column.SqlColumnName}");

                    break;
                }
                case ConstantExpression constantExpression:
                {
                    Append($" {_sqlArguments.Register(SqlType.Default,constantExpression.Value)}");
                    break;
                }
                default:

                    throw new NotImplementedException();
            }
        }

        private void AnalyzeMemberObject(MemberExpression memberExpression, Type type, object obj)
        {
            object value = ((PropertyInfo)memberExpression.Member).GetValue(obj);
            Append($" {_sqlArguments.Register(SqlType.Default, value)}");

            // ((System.Reflection.PropertyInfo)memberExpression.Member).GetMethod
        }

        private string AnalyzeNode(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Add:
                    return " + ";
                case ExpressionType.And:
                    return " AND ";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.ExclusiveOr:
                    return " ^ ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.Multiply:
                    return " * ";
                case ExpressionType.NotEqual:
                    return " != ";
                case ExpressionType.Or:
                    return " OR ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Subtract:
                    return " - ";
                case ExpressionType.Decrement:
                    return " -1 ";
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Increment:
                    return " +1 ";
                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionType), expressionType, null);
            }

            return "";
        }

    }
}