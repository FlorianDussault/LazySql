using System;
using System.Linq.Expressions;

namespace LazySql.Engine
{
    internal class RelationInformation
    {
        public Type ParentType { get; }
        public string Column { get; }
        public Type ChildType { get; }
        public LambdaExpression Expression { get; }
        public RelationInformation(Type parentType, string column, Type childType, LambdaExpression expression)
        {
            ParentType = parentType;
            Column = column;
            ChildType = childType;
            Expression = expression;
        }
    }
}