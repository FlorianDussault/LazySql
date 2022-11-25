namespace LazySql;

public class StoredProcedureOutputTable
{
    public DataTable DataTable { get; }

    internal StoredProcedureOutputTable(DataTable dataTable)
    {
        DataTable = dataTable;
    }

    public IEnumerable<T> Parse<T>() where T : LazyBase
    {
        return DataTable.ToLazyObject<T>();
    }

    public IEnumerable<dynamic> Parse()
    {
        return DataTable.ToDynamic();
    }
}