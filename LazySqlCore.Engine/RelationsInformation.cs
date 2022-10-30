using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LazySql.Engine.Enums;

namespace LazySql.Engine
{
    internal class RelationsInformation : List<RelationInformation>
    {
        public void Add(RelationType relationType, Type parentType, string column, Type childType, LambdaExpression expression) => Add(new RelationInformation(relationType, parentType, column, childType, expression));
    }
}