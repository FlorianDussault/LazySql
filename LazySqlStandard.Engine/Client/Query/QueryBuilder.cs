using System;
using System.Linq.Expressions;
using System.Text;
using LazySql.Engine.Client.Lambda;
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

        
        public void Append(Expression expression = null, Type type = null, object obj = null)
        {
            if (expression == null) return;
            LambdaParser.Parse(expression, _tableDefinition, this, type, obj);
        }

        public string RegisterArgument(SqlType type, object obj) => _sqlArguments.Register(type, obj);

    }
}