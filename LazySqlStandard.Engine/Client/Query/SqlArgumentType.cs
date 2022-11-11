namespace LazySql.Engine.Client.Query;

internal enum SqlArgumentType
{
    /// <summary>
    /// In
    /// </summary>
    In = 0,
    /// <summary>
    /// Out
    /// </summary>
    Out = 1,
    /// <summary>
    /// Return Value
    /// </summary>
    ReturnValue = 2
}