namespace LazySql.Engine.Client.StoredProcedure;

public class StoredProcedureResult
{
    internal StoredProcedureResult()
    {

    }

    public int? ReturnValue { get; internal set; }

    public dynamic Output { get; internal set; }

    public IReadOnlyList<StoredProcedureOutputTable> Tables { get; internal set; }
}