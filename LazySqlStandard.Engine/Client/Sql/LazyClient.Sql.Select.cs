﻿// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    public static ILazyEnumerable<T> Select<T>(string tableName = null) => Instance.InternalSelect<T>(tableName);

    private ILazyEnumerable<T> InternalSelect<T>(string tableName)
    {
        CheckInitialization(typeof(T), out TableDefinition tableDefinition);
        if (string.IsNullOrWhiteSpace(tableName) && tableDefinition.ObjectType == ObjectType.Dynamic)
            throw new LazySqlException($"You can call the {nameof(Select)} method with a Dynamic type without a table name in argument");
        return new LazyEnumerable<T>(tableName);
    }


    internal static IEnumerable<object> GetWithQuery(Type type, QueryBuilder queryBuilder)
    {
        CheckInitialization(type, out TableDefinition tableDefinition);
        if (tableDefinition.Relations.Count == 0)
        {
            foreach (object value in ExecuteReader(queryBuilder))
                yield return value;
            yield break;
        }

        List<object> values = ExecuteReader(queryBuilder).ToList();
        if (values.Count == 0) yield break;

        foreach (RelationInformation relation in tableDefinition.Relations)
            LoadChildren(type, relation, values);

        foreach (object value in values)
            yield return value;
    }

    /// <summary>
    /// Build SELECT
    /// </summary>
    /// <param name="tableDefinition">Table definition</param>
    /// <param name="queryBuilder">Query Builder</param>
    /// <param name="top">TOP</param>
    internal static void BuildSelect(TableDefinition tableDefinition, string tableName, QueryBuilder queryBuilder, int? top = null)
    {
        tableDefinition.GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out _, out _, out _);

        string columnsList;
        if (tableDefinition.ObjectType == ObjectType.LazyObject)
            columnsList = string.Join(", ", allColumns.Where(c => c.Column.SqlType != SqlType.Children).Select(c => c.Column.SqlColumnName));
        else
            columnsList = "*";

        queryBuilder.Append($"SELECT {(top != null ? $" TOP {top} " : string.Empty)} {columnsList} FROM {tableName ?? tableDefinition.Table.TableName}");

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
        CheckInitialization(relationInformation.ChildType, out TableDefinition childTableDefinition);

        QueryBuilder queryBuilder = new(childTableDefinition);

        BuildSelect(childTableDefinition, null, queryBuilder);
        queryBuilder.Append(" WHERE ");

        for (int index = 0; index < values.Count; index++)
        {
            queryBuilder.Append("(");
            queryBuilder.Append(relationInformation.Expression, parentType, values[index]);
            queryBuilder.Append(")");
            if (index + 1 < values.Count)
                queryBuilder.Append(" OR ");
        }

        Delegate delegateExpression = relationInformation.Expression.Compile();
        IEnumerable enumerableChildValues = ReflectionHelper.InvokeStaticMethod<IEnumerable>(typeof(LazyClient),
            nameof(GetWithQuery), new object[] { relationInformation.ChildType, queryBuilder });

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
