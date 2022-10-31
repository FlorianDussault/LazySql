﻿using System;
using System.Linq.Expressions;
using LazySql.Engine.Enums;

namespace LazySql.Engine
{

    /// <summary>
    /// Base class (abstract) used to declare supported object.
    /// </summary>
    public abstract class LazyBase
    {
        internal RelationsInformation Relations { get; } = new RelationsInformation();

        internal void Initialize()
        {
            InitializeTable();
        }

        /// <summary>
        /// Override it to initialize relations
        /// </summary>
        public virtual void InitializeTable()
        {
            
        }

        /// <summary>
        /// Add relation of type One to Many (0,1=>0,n)
        /// </summary>
        /// <typeparam name="T">Parent table</typeparam>
        /// <typeparam name="C">Child table</typeparam>
        /// <param name="column"></param>
        /// <param name="expression"></param>
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