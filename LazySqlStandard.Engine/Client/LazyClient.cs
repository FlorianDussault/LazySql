namespace LazySql.Engine.Client;

/// <summary>
/// LazyClient
/// </summary>
public sealed partial class LazyClient
{
    private List<TableDefinition> _tables;

    #region Singleton
    private static LazyClient Instance { get; } = new();

    private LazyClient()
    {
    }

    #endregion
}