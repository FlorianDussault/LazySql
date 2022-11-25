namespace LazySql;

/// <summary>
/// Lambda Alias
/// </summary>
internal sealed class LambdaAlias
{
    public string SqlAlias { get;  }

    public ITableDefinition TableDefinition { get;  }


    public LambdaAlias(string sqlAlias, ITableDefinition tableDefinition)
    {
        SqlAlias = sqlAlias;
        TableDefinition = tableDefinition;
    }
}