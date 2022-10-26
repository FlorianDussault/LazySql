using System.Collections.Generic;
using LazySql.Engine.Definitions;

namespace LazySql.Engine.Client
{
    public sealed partial class SqlClient
    {
        private IReadOnlyCollection<TableDefinition> _tables = null;

        #region Singleton
        private static SqlClient Instance { get; } = new SqlClient();

        private SqlClient()
        {
        }

        #endregion
    }
}