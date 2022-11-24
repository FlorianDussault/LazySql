namespace LazySql.Engine;

/// <summary>
/// DataLive object to retrieve or update data in database.
/// </summary>
/// <typeparam name="T">Supported Object</typeparam>
public class DataLive<T> : List<T>  where T : LazyBase
{
    private readonly IReadOnlyList<ColumnDefinition> _primaryKeys;

    /// <summary>
    /// DataLive
    /// </summary>
    /// <param name="loadAllData">if true, all the data will be load from the database</param>
    public DataLive(bool loadAllData = false)
    {
        LazyClient.CheckInitialization(typeof(T), out ITableDefinition tableDefinition);
        tableDefinition.GetColumns(out _, out _, out _, out _primaryKeys);
        if(loadAllData) Load();
    }

    /// <summary>
    /// Create DataLive object and load it
    /// </summary>
    /// <param name="expression">Expression</param>
    public DataLive(Expression<Func<T, bool>> expression)
    {
        LazyClient.CheckInitialization(typeof(T), out ITableDefinition tableDefinition);
        tableDefinition.GetColumns(out _, out _, out _, out _primaryKeys);
        Load(expression);
    }

    /// <summary>
    /// Check primary keys
    /// </summary>
    /// <param name="item1">First item</param>
    /// <param name="item2">Second item</param>
    /// <returns></returns>
    private bool AreEquals(T item1, T item2) => _primaryKeys.All(primaryKey => primaryKey.PropertyInfo.GetValue(item1).Equals(primaryKey.PropertyInfo.GetValue(item2)));

    /// <summary>
    /// Load list (will be clear before load)
    /// </summary>
    /// <param name="expression">Lambda expression to filter data</param>
    public void Load(Expression<Func<T, bool>> expression = null)
    {
        Clear();
        LoadAdditional();
    }

    /// <summary>
    /// Add additional items
    /// </summary>
    /// <param name="expression">Lambda expression to filter data</param>
    public void LoadAdditional(Expression<Func<T, bool>> expression = null)
    {
        foreach (T item in LazyClient.Select<T>().Where(expression))
        {
            if (this.Any(i => AreEquals(i, item)))
                continue;
            base.Add(item);
        }
    }

    /// <summary>
    /// Add item in list and database
    /// </summary>
    /// <param name="item">Item to add</param>
    public new void Add(T item)
    {
        item.Insert();
        base.Add(item);
    }

    /// <summary>
    /// Add a collection of items in list and database
    /// </summary>
    /// <param name="collection">Collection of items</param>
    public new void AddRange(IEnumerable<T> collection)
    {
        foreach (T value in collection.ToList())
        {
            if (this.Any(i => AreEquals(i, value)))
                continue;
            Add(value);
        }
    }
        
    /// <summary>
    /// Insert a new item in list and database at the specified index (in list)
    /// </summary>
    /// <param name="index">Index in list</param>
    /// <param name="item">Item to insert</param>
    public new void Insert(int index, T item)
    {
        if (this.Any(i => AreEquals(i, item))) return;
        item.Insert();
        base.Insert(index, item);
    }

    /// <summary>
    /// Insert a range of items in list and database at the specified index (in list)
    /// </summary>
    /// <param name="index">Index in list</param>
    /// <param name="collection">Collection of item</param>
    public new void InsertRange(int index, IEnumerable<T> collection)
    {
        List<T> items = new();

        foreach (T value in collection.ToList())
        {
            if (this.Any(i => AreEquals(i, value)))
                continue;
            items.Add(value);
        }

        foreach (T item in items)
            item.Insert();
        base.InsertRange(index, items);
    }

    /// <summary>
    /// Remove an item in list and database
    /// </summary>
    /// <param name="item"></param>
    /// <returns>return true if the item has been removed</returns>
    public new bool Remove(T item)
    {
        item.Delete();
        return base.Remove(item);
    }

    /// <summary>
    /// Remove items in list and database (based on a predicate expression)
    /// </summary>
    /// <param name="match">Predicate expression</param>
    /// <returns>Number of deleted items</returns>
    public new int RemoveAll(Predicate<T> match)
    {
        int count = 0;

        foreach (T item in this.Where(i=>match(i)).ToList())
        {
            Remove(item);
            count++;
                
        }
        return count;
    }

    /// <summary>
    /// Remove item in list and database at the specified index (in list)
    /// </summary>
    /// <param name="index">Index in list</param>
    public new void RemoveAt(int index)
    {
        this[index].Delete();
        base.RemoveAt(index);
    }

    /// <summary>
    /// Remove range of items in list and database from the specified index (in list)
    /// </summary>
    /// <param name="index">Index in list</param>
    /// <param name="count">Number of items</param>
    public new void RemoveRange(int index, int count)
    {
        for (int i = 0; i < count; i++)
            RemoveAt(index + i);
    }

}