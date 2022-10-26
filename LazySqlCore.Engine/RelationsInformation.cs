using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazySql.Engine
{
    internal class RelationsInformation : List<RelationInformation>
    {
        public void Add(Type parentType, string column, Type childType, LambdaExpression expression) => Add(new RelationInformation(parentType, column, childType, expression));
    }
}