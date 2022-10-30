using System.Collections.Generic;
using LazySql.Engine.Definitions;

namespace LazySql.Engine.Client
{
    public sealed partial class LazyClient
    {
        private IReadOnlyCollection<TableDefinition> _tables = null;

        #region Singleton
        private static LazyClient Instance { get; } = new LazyClient();

        private LazyClient()
        {
        }

        #endregion
    }
}