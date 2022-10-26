using System;
using System.Linq.Expressions;

namespace LazySql.Engine
{
    public abstract class LazyBase
    {
        internal RelationsInformation OneToManyExpressions { get; } = new RelationsInformation();

        internal void Initialize()
        {
            InitializeTable();
        }

        public virtual void InitializeTable()
        {
            
        }

        public void AddOneToMany<T,C>(string column, Expression<Func<T,C, object>> expression) where T : LazyBase where C : LazyBase
        {
            OneToManyExpressions.Add(typeof(T), column, typeof(C), expression);
        }
    }
}