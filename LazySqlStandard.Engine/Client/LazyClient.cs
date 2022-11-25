namespace LazySql;

/// <summary>
/// LazyClient
/// </summary>
public sealed partial class LazyClient
{
    private List<ITableDefinition> _tables;

    #region Singleton
    private static LazyClient Instance { get; } = new();

    private LazyClient()
    {
    }

    #endregion
}