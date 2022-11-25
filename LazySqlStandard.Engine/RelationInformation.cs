namespace LazySql;

/// <summary>
/// Relation information
/// </summary>
internal class RelationInformation
{
    /// <summary>
    /// Parent column
    /// </summary>
    public string Column { get; }

    /// <summary>
    /// Type of child object
    /// </summary>
    public Type ChildType { get; }

    /// <summary>
    /// Join Expression
    /// </summary>
    public LambdaExpression Expression { get; }

    /// <summary>
    /// Type of relation
    /// </summary>
    public RelationType RelationType { get; }

    public RelationInformation(RelationType relationType, string column, Type childType, LambdaExpression expression)
    {
        RelationType = relationType;
        Column = column;
        ChildType = childType;
        Expression = expression;
    }
}
