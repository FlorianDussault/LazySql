namespace LazySql;

/// <summary>
/// Relation information
/// </summary>
internal class RelationInformation
{
    /// <summary>
    /// Type of parent
    /// </summary>
    public Type ParentType { get; }

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

    public RelationInformation(RelationType relationType, Type parentType, string column, Type childType, LambdaExpression expression)
    {
        RelationType = relationType;
        ParentType = parentType;
        Column = column;
        ChildType = childType;
        Expression = expression;
    }
}
