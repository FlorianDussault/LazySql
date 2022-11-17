// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    /// <summary>
    /// Select in database
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="tableName">Table name (mandatory for dynamic type)</param>
    /// <returns>Enumerable</returns>
    public static ILazyEnumerable<T> Select<T>(string tableName = null) => Instance.InternalSelect<T>(tableName);

    /// <summary>
    /// Select in database
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="tableName">Table name (mandatory for dynamic type)</param>
    /// <returns>Enumerable</returns>
    /// <exception cref="LazySqlException"></exception>
    private ILazyEnumerable<T> InternalSelect<T>(string tableName)
    {
        CheckInitialization(typeof(T), out TableDefinition tableDefinition);
        if (string.IsNullOrWhiteSpace(tableName) && tableDefinition.ObjectType == ObjectType.Dynamic)
            throw new LazySqlException($"You can call the {nameof(Select)} method with a Dynamic type without a table name in argument");
        return new LazyEnumerable<T>(tableName);
    }
    
    /// <summary>
    /// Execute Query
    /// </summary>
    /// <param name="type">Type of object</param>
    /// <param name="selectQuery">Table definition</param>
    /// <returns>IEnumerable</returns>
    internal static IEnumerable<object> GetWithQuery(Type type, SelectQuery selectQuery)
    {
        CheckInitialization(type, out TableDefinition tableDefinition);
        if (tableDefinition.Relations.Count == 0)
        {
            foreach (object value in ExecuteReader(selectQuery.BuildQuery()))
                yield return value;
            yield break;
        }

        List<object> values = ExecuteReader(selectQuery.BuildQuery()).ToList();
        if (values.Count == 0) yield break;

        foreach (RelationInformation relation in tableDefinition.Relations)
            LoadChildren(type, relation, values);

        foreach (object value in values)
            yield return value;
    }

    /// <summary>
    /// Load Children
    /// </summary>
    /// <param name="parentType">Parent Type</param>
    /// <param name="relationInformation">Relation Information</param>
    /// <param name="values">Parent values</param>
    /// <exception cref="NotImplementedException"></exception>
    internal static void LoadChildren(Type parentType, RelationInformation relationInformation, List<object> values)
    {
        if (values.Count == 0) return;
        CheckInitialization(parentType, out TableDefinition parentTableDefinition);
        CheckInitialization(relationInformation.ChildType, out TableDefinition childTableDefinition);

        // SELECT * FROM T1
        // WHERE EXISTS
        //    (SELECT* FROM T2
        //    WHERE T1.a= T2.a and T1.b= T2.b)

        SelectQuery selectQuery = new(childTableDefinition);
        void WhereAction(SelectQuery query)
        {
            //LambdaParser.Parse(expression, _tableDefinition, this, type, obj);
            query.QueryBuilder.Append($" EXISTS (SELECT * FROM {parentTableDefinition.Table.TableName} AS lazy_parent WHERE ");
            query.QueryBuilder.AppendWithAliases(relationInformation.Expression, new LambdaAlias("lazy_parent", parentTableDefinition), new LambdaAlias(query.TableAlias, childTableDefinition));
            query.QueryBuilder.Append(")");
        }

        selectQuery.SetWhereQuery(new WhereFunctionQuery(WhereAction));

        Delegate delegateExpression = relationInformation.Expression.Compile();
        IEnumerable enumerableChildValues = ReflectionHelper.InvokeStaticMethod<IEnumerable>(typeof(LazyClient),
            nameof(GetWithQuery), new object[] { relationInformation.ChildType, selectQuery });

        IList childValues = ReflectionHelper.CreateList(relationInformation.ChildType);
        foreach (object enumerableValue in enumerableChildValues)
            childValues.Add(enumerableValue);

        if (relationInformation.RelationType == RelationType.OneToMany)
        {
            foreach (object parentValue in values)
            {
                IList children = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(relationInformation.ChildType));
                foreach (object childValue in childValues)
                {
                    if ((bool)delegateExpression.DynamicInvoke(parentValue, childValue))
                    {
                        children.Add(childValue);
                    }
                }
                parentType.GetProperty(relationInformation.Column)?.SetValue(parentValue, children);
            }
        }
        else if (relationInformation.RelationType == RelationType.OneToOne)
        {
            foreach (object parentValue in values)
            {
                foreach (object childValue in childValues)
                {
                    if ((bool)delegateExpression.DynamicInvoke(parentValue, childValue))
                    {
                        parentType.GetProperty(relationInformation.Column)?.SetValue(parentValue, childValue);
                        break;
                    }
                }
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
