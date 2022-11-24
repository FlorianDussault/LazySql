using LazySql.Engine.Definitions;

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

        RegisterTableObjects(types);
        ConnectionString = connectionString;
        _initialized = true;

    }
    #endregion

    #region RegisterTableObjects

    /// <summary>
    /// Check tables
    /// </summary>
    /// <param name="types">Types of items</param>
    /// <exception cref="LazySqlInitializeException"></exception>
    private void RegisterTableObjects(Type[] types)
    {
        List<ITableDefinition> tables = new();

        foreach (Type type in types) 
            tables.Add(RegisterTableObject(type));

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
    private ITableDefinition RegisterTableObject(Type type)
    {
        if (type == null)
            throw new LazySqlInitializeException($"Type of supported table cannot be null");
        if (type.GetTypeInfo().IsAbstract)
            throw new LazySqlInitializeException($"Abstract object is not supported ({type.Name})");
        if (!type.GetTypeInfo().IsClass)
            throw new LazySqlInitializeException($"{type.Name} is not a class");

        switch (GetObjectType(type))
        {
            case ObjectType.LazyObject:
                return RegisterLazyObject(type);
            case ObjectType.Object:
                return RegisterObject(type);
            case ObjectType.Dynamic:
                return RegisterDynamicObject(type);
            default:
                throw new ArgumentOutOfRangeException();
        }

    }



    #endregion

    private TableDefinitionLazy RegisterLazyObject(Type type)
    {
        if (Attribute.GetCustomAttribute(type, typeof(LazyTable)) is not LazyTable tableAttribute)
            throw new LazySqlInitializeException($"{nameof(LazyTable)} attribute is missing in class {type.FullName}");

        if (string.IsNullOrWhiteSpace(tableAttribute.TableName))
            throw new LazySqlInitializeException($"The table name is missing in the attribute {nameof(LazyTable)} in class {type.FullName}");

        TableDefinitionLazy tableDefinition = new(type, tableAttribute);

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

    private TableDefinitionObject RegisterObject(Type type)
    {
        List<PropertyInfo> properties = type.GetProperties().Where(p => p.CanRead).ToList();

        if (properties.Count == 0)
            throw new NotImplementedException(); // TODO : to remove ?
        //return new TableDefinition(type, new LazyTable(type.Name), ObjectType.Dynamic);

        TableDefinitionObject tableDefinition = new(type, new LazyTable(type.Name));

        foreach (PropertyInfo propertyInfo in properties)
        {
            LazyColumn columnAttribute = new(propertyInfo.Name, propertyInfo.PropertyType.ToSqlType());
            tableDefinition.Add(propertyInfo, columnAttribute, null);
        }

        return tableDefinition;
    }

    private ITableDefinition RegisterDynamicObject(Type type) => new TableDefinitionDynamic(type, new LazyTable(type.Name));

    /// <summary>
    /// Check initialization and get information about the table
    /// </summary>
    /// <param name="type">Type of item</param>
    /// <param name="tableDefinition">Table definition</param>
    internal static void CheckInitialization(Type type, out ITableDefinition tableDefinition) => Instance.InternalCheckInitialization(type, out tableDefinition);

    /// <summary>
    /// Check initialization and get information about the table
    /// </summary>
    /// <param name="type">Type of item</param>
    /// <param name="tableDefinition">Table definition</param>
    /// <exception cref="LazySqlInitializeException"></exception>
    private void InternalCheckInitialization(Type type, out ITableDefinition tableDefinition)
    {
        if (!_initialized)
            throw new LazySqlInitializeException($"Call method {nameof(Initialize)} first.");

        if (GetObjectType(type) == ObjectType.Dynamic)
        {
            tableDefinition = RegisterTableObject(type);
            return;
        }

        tableDefinition = _tables.FirstOrDefault(t => t.TableType == type);
        if (tableDefinition != null) return;

        tableDefinition = RegisterTableObject(type);
        _tables.Add(tableDefinition);
    }

    private ObjectType GetObjectType(Type type)
    {
        if (typeof(LazyBase).IsAssignableFrom(type))
            return ObjectType.LazyObject;
        if (type.GetProperties().Any(p => p.CanRead))
            return ObjectType.Object;
        return ObjectType.Dynamic;
    }
}