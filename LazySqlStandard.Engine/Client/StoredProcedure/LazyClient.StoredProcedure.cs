namespace LazySql;

public sealed partial class LazyClient
{

    private StoredProcedureResult InternalStoredProcedure(string procedureName, SqlArguments arguments)
    {
        using SqlConnector connector = Open();
        (DataSet dataset, SqlArguments outputArguments, int? returnValue) result = connector.ExecuteStoredProcedure(procedureName, arguments);

        StoredProcedureResult storedProcedureResult = new()
        {
            ReturnValue = result.returnValue
        };

        IDictionary<string, object> output = new ExpandoObject();
        foreach (SqlArgument sqlArgument in result.outputArguments)
            output.Add(sqlArgument.Name.Replace("@", string.Empty), sqlArgument.Value);
        storedProcedureResult.Output = output;

        List<StoredProcedureOutputTable> tables = new();
        foreach (DataTable dataTable in result.dataset.Tables) tables.Add(new StoredProcedureOutputTable(dataTable));

        storedProcedureResult.Tables = tables;

        return storedProcedureResult;
    }

    public static StoredProcedureResult StoredProcedure(StoredQuery storedQuery) => Instance.InternalStoredProcedure(storedQuery.ProcedureName, storedQuery.SqlArguments);
}