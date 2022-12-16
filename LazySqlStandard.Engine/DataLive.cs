namespace LazySql;

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
    private bool AreEquals(T item1, T item2)
    {
        if (_primaryKeys == null || _primaryKeys.Count == 0) return false;
        return _primaryKeys.All(primaryKey =>
            primaryKey.PropertyInfo.GetValue(item1).Equals(primaryKey.PropertyInfo.GetValue(item2)));
    }

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
    /// Bind additional items
    /// </summary>
    /// <param name="expression">Lambda expression to filter data</param>
    public void LoadAdditional(Expression<Func<T, bool>> expression = null)
    {
        foreach (T item in LazyClient.Select(expression))
        {
            if (this.Any(i => AreEquals(i, item)))
                continue;
            base.Add(item);
        }
    }

    /// <summary>
    /// Bind item in list and database
    /// </summary>
    /// <param name="item">Item to add</param>
    public new int Add(T item)
    {
        var cnt = item.Insert();
        if (cnt > 0)
            base.Add(item);
        return cnt;
    }

    /// <summary>
    /// Bind a collection of items in list and database
    /// </summary>
    /// <param name="collection">Collection of items</param>
    public new int AddRange(IEnumerable<T> collection)
    {
        int cnt = 0;
        foreach (T value in collection.ToList())
        {
            if (this.Any(i => AreEquals(i, value)))
                continue;
            cnt += Add(value);
        }

        return cnt;
    }
        
    /// <summary>
    /// Insert a new item in list and database at the specified index (in list)
    /// </summary>
    /// <param name="index">Index in list</param>
    /// <param name="item">Item to insert</param>
    public new int Insert(int index, T item)
    {
        int cnt = 0;
        if (this.Any(i => AreEquals(i, item))) return cnt;
        cnt = item.Insert();
        if (cnt > 0)
            base.Insert(index, item);
        return cnt;
    }

    /// <summary>
    /// Insert a range of items in list and database at the specified index (in list)
    /// </summary>
    /// <param name="index">Index in list</param>
    /// <param name="collection">Collection of item</param>
    public new int InsertRange(int index, IEnumerable<T> collection)
    {
        List<T> items = new();
        int cnt = 0;
        foreach (T value in collection.ToList())
        {
            if (this.Any(i => AreEquals(i, value)))
                continue;
            items.Add(value);
        }

        foreach (T item in items)
            cnt += item.Insert();
        base.InsertRange(index, items);
        return cnt;
    }

    /// <summary>
    /// Remove an item in list and database
    /// </summary>
    /// <param name="item"></param>
    /// <returns>return true if the item has been removed</returns>
    public new bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index < 0) return false;
        return item.Delete() > 0 && base.Remove(item);
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
            if (Remove(item))
                count++;
                
        }
        return count;
    }

    /// <summary>
    /// Remove item in list and database at the specified index (in list)
    /// </summary>
    /// <param name="index">Index in list</param>
    public new int RemoveAt(int index)
    {
        int cnt = this[index].Delete();
        if (cnt > 0) 
            base.RemoveAt(index);
        return cnt;
    }

    /// <summary>
    /// Remove range of items in list and database from the specified index (in list)
    /// </summary>
    /// <param name="index">Index in list</param>
    /// <param name="count">Number of items</param>
    public new int RemoveRange(int index, int count)
    {
        int cnt = 0;
        for (int i = 0; i < count; i++)
        {
            int cntUnit = RemoveAt(index);
            if (cntUnit == 0) index++;
            cnt += cntUnit;
        }
        return cnt;
    }

}