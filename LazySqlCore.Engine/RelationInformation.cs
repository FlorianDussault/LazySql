using System;
using System.Linq.Expressions;
using LazySql.Engine.Enums;

namespace LazySql.Engine
{
    internal class RelationInformation
    {
        public Type ParentType { get; }
        public string Column { get; }
        public Type ChildType { get; }
        public LambdaExpression Expression { get; }
        public RelationType RelationType { get; }
        public RelationInformation(RelationType relationType, Type parentType, string column, Type childType, LambdaExpression expression)
        {
            RelationType = relationType;
            ParentType = parentType;
            Column = column;
            ChildType = childType;
            Expression = expression;
        }
    }
}