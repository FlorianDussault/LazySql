namespace LazySql.Engine.Client;

/// <summary>
/// LazyClient
/// </summary>
// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    private bool _initialized;

    /// <summary>
    /// Return state of the initialization
    /// </summary>
    public static bool Initialized => Instance._initialized;
        
    #region Initialize
        
    /// <summary>
    /// Initialize LazyClient
    /// </summary>
    /// <param name="connectionString">Connection String</param>
    /// <param name="types">Type of supported item</param>
    public static void Initialize(string connectionString, params Type[] types) => Instance.InternalInitialize(connectionString, types);

    /// <summary>
    /// Initialize LazyClient
    /// </summary>
    /// <param name="connectionString">Connection String</param>
    /// <param name="types">Type of supported item</param>
    /// <exception cref="LazySqlInitializeException"></exception>
    private void InternalInitialize(string connectionString, Type[] types)
    {
        if (_initialized)
            throw new LazySqlInitializeException("SqlClient already initialized");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new LazySqlInitializeException("Connection string cannot be null or empty");

        CheckTables(types);
        ConnectionString = connectionString;
        _initialized = true;

    }
    #endregion

    #region CheckTables

    /// <summary>
    /// Check tables
    /// </summary>
    /// <param name="types">Types of items</param>
    /// <exception cref="LazySqlInitializeException"></exception>
    private void CheckTables(Type[] types)
    {
        List<TableDefinition> tables = new List<TableDefinition>();

        foreach (Type type in types) 
            tables.Add(CheckTable(type));

        if (tables.Count == 0)
            throw new LazySqlInitializeException("No table declared");

        _tables = tables;
    }

    /// <summary>
    /// Check a table
    /// </summary>
    /// <param name="type">Type of the item</param>
    /// <returns></returns>
    /// <exception cref="LazySqlInitializeException"></exception>
    private TableDefinition CheckTable(Type type)
    {
        if (type == null)
            throw new LazySqlInitializeException($"Type of supported table cannot be null");
        if (type.GetTypeInfo().IsAbstract)
            throw new LazySqlInitializeException($"Abstract object is not supported ({type.Name})");
        if (!type.GetTypeInfo().IsClass)
            throw new LazySqlInitializeException($"{type.Name} is not a class");
        if (type.IsAssignableFrom(typeof(LazyTable)))
            throw new LazySqlInitializeException($"{type.Name} does not implement the class {nameof(LazyBase)}");

        LazyTable tableAttribute = (LazyTable) Attribute.GetCustomAttribute(type, typeof(LazyTable)) ?? new LazyTable(type.Name);

        if (string.IsNullOrWhiteSpace(tableAttribute.TableName))
            throw new LazySqlInitializeException($"The table name is missing in the attribute {nameof(LazyTable)} in class {type.FullName}");

        TableDefinition tableDefinition = new(type, tableAttribute);

        LazyBase obj = (LazyBase)Activator.CreateInstance(type);
        
        obj.Initialize();
        if (obj.Relations != null)
            tableDefinition.Relations = obj.Relations;

        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo propertyInfo in properties)
        {
            if (!(propertyInfo.GetCustomAttribute(typeof(LazyColumn)) is LazyColumn columnAttribute))
                continue;

            if (string.IsNullOrWhiteSpace(columnAttribute.ColumnName))
                columnAttribute.SetColumnName(propertyInfo.Name);

            PrimaryKey primaryKey = propertyInfo.GetCustomAttribute(typeof(PrimaryKey)) as PrimaryKey;


            tableDefinition.Add(propertyInfo, columnAttribute, primaryKey);
        }

        if (tableDefinition.Count == 0)
            throw new LazySqlInitializeException($"No column attribute {nameof(LazyColumn)} found in class {type.FullName}");

        return tableDefinition;
    }
    #endregion

    /// <summary>
    /// Check initialization and get information about the table
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    /// <param name="tableDefinition">Table definition</param>
    private void CheckInitialization<T>(out TableDefinition tableDefinition) where T : LazyBase => InternalCheckInitialization(typeof(T), out tableDefinition);

    /// <summary>
    /// Check initialization and get information about the table
    /// </summary>
    /// <param name="type">Type of item</param>
    /// <param name="tableDefinition">Table definition</param>
    internal static void CheckInitialization(Type type, out TableDefinition tableDefinition) => Instance.InternalCheckInitialization(type, out tableDefinition);

    /// <summary>
    /// Check initialization and get information about the table
    /// </summary>
    /// <param name="type">Type of item</param>
    /// <param name="tableDefinition">Table definition</param>
    /// <exception cref="LazySqlInitializeException"></exception>
    private void InternalCheckInitialization(Type type, out TableDefinition tableDefinition)
    {
        if (!_initialized)
            throw new LazySqlInitializeException($"Call method {nameof(Initialize)} first.");
        TableDefinition table = _tables.FirstOrDefault(t => t.TableType == type);

        if (table == null)
        {
            CheckTable(type);
        }

        tableDefinition = table ?? throw new LazySqlInitializeException($"Table not found in method {nameof(Initialize)}");
    }
}