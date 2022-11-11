namespace LazySql.Engine.Enums;

/// <summary>
/// Relation Type
/// </summary>
internal enum RelationType
{
    /// <summary>
    /// One to One
    /// </summary>
    OneToOne = 0,

    /// <summary>
    /// One to One (flatten)
    /// </summary>
    OneToOneFlatten = 1,

    /// <summary>
    /// One to many
    /// </summary>
    OneToMany = 2,
}