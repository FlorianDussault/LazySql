namespace LazySql.Engine.Attributes;

/// <summary>
/// Primary key attribute
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PrimaryKey : Attribute
{
    /// <summary>
    /// Is column autoincrement
    /// </summary>
    internal bool AutoIncrement {get;}

    /// <summary>
    /// Is column autoincrement
    /// </summary>
    /// <param name="autoIncrement"></param>
    public PrimaryKey(bool autoIncrement)
    {
        AutoIncrement = autoIncrement;
    }
}