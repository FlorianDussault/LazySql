namespace LazySql;

/// <summary>
/// List of relations
/// </summary>
internal class RelationsInformation : List<RelationInformation>
{
    /// <summary>
    /// Add relation
    /// </summary>
    /// <param name="relationType">Type of relation</param>
    /// <param name="parentType">Parent Type</param>
    /// <param name="column">Column in Parent</param>
    /// <param name="childType">Child Type</param>
    /// <param name="expression">Join expression</param>
    public void Add(RelationType relationType, Type parentType, string column, Type childType, LambdaExpression expression) => Add(new RelationInformation(relationType, parentType, column, childType, expression));
}