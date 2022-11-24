using System;
using System.Collections.Generic;
using System.Text;

namespace LazySql.Engine.Client.Query
{
    internal abstract class QueryBase
    {
        protected string TableName { get; }
        protected IWhereQuery WhereQuery { get; private set; }


        /// <summary>
        /// Query Builder
        /// </summary>
        protected internal QueryBuilder QueryBuilder { get; }

        /// <summary>
        /// Table Definition
        /// </summary>
        protected ITableDefinition TableDefinition { get; }

        /// <summary>
        /// Alias Name
        /// </summary>
        public string TableAlias { get; }


        protected QueryBase(ITableDefinition tableDefinition, string tableName)
        {
            TableDefinition = tableDefinition;
            QueryBuilder = new QueryBuilder(tableDefinition);
            TableName = tableDefinition.GetTableName(tableName);
            TableAlias = $"{TableName}_{Guid.NewGuid().ToString().Substring(0, 4)}";
        }

        /// <summary>
        /// Set WHERE
        /// </summary>
        /// <param name="whereQuery"></param>
        public void SetWhereQuery(IWhereQuery whereQuery) => WhereQuery = whereQuery;

        public virtual QueryBuilder BuildQuery() => throw new NotImplementedException();

    }
}
