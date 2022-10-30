using System;
using System.Linq.Expressions;
using LazySql.Engine.Enums;

namespace LazySql.Engine
{
    public abstract class LazyBase
    {
        internal RelationsInformation Relations { get; } = new RelationsInformation();

        internal void Initialize()
        {
            InitializeTable();
        }

        public virtual void InitializeTable()
        {
            
        }

        // ReSharper disable once UnusedMember.Global
        public void AddOneToMany<T,C>(string column, Expression<Func<T,C, bool>> expression) where T : LazyBase where C : LazyBase
        {
            Relations.Add(RelationType.OneToMany, typeof(T), column, typeof(C), expression);
        }

        // ReSharper disable once UnusedMember.Global
        public void AddOneToOne<T, C>(string column, Expression<Func<T, C, bool>> expression) where T : LazyBase where C : LazyBase
        {
            Relations.Add(RelationType.OneToOne, typeof(T), column, typeof(C), expression);
        }
    }
}