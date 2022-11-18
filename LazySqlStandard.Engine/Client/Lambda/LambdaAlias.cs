namespace LazySql.Engine.Client.Lambda;

/// <summary>
/// Lambda Alias
/// </summary>
internal sealed class LambdaAlias
{
    public string SqlAlias { get;  }

    public TableDefinition TableDefinition { get;  }


    public LambdaAlias(string sqlAlias, TableDefinition tableDefinition)
    {
        SqlAlias = sqlAlias;
        TableDefinition = tableDefinition;
    }
}