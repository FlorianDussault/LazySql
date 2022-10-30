using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LazySql.Engine.Client;
using LazySql.Engine.Definitions;

namespace LazySql.Engine
{
    
    public class DataLive<T> : List<T>  where T : LazyBase
    {
        private readonly TableDefinition _tableDefinition;
        private IReadOnlyList<ColumnDefinition> _primaryKeys;

        public DataLive()
        {
            LazyClient.CheckInitialization(typeof(T), out _tableDefinition);
            _tableDefinition.GetColumns(out _, out _, out _, out _primaryKeys);
        }

        private bool AreEquals(T item1, T item2) => _primaryKeys.All(primaryKey =>
        {
            return primaryKey.PropertyInfo.GetValue(item1).Equals(primaryKey.PropertyInfo.GetValue(item2));
        });

        public void Load(Expression<Func<T, bool>> expression = null)
        {
            Clear();
            LoadAdditional();
        }

        public void LoadAdditional(Expression<Func<T, bool>> expression = null)
        {
            foreach (T item in LazyClient.Get(expression))
            {
                if (this.Any(i => AreEquals(i, item)))
                    continue;
                base.Add(item);
            }
        }

        public new void Add(T item)
        {
            item.Insert();
            base.Add(item);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (T value in collection.ToList())
            {
                if (this.Any(i => AreEquals(i, value)))
                    continue;
                Add(value);
            }
        }
        
        public new void Insert(int index, T item)
        {
            if (this.Any(i => AreEquals(i, item))) return;
            item.Insert();
            base.Insert(index, item);
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            List<T> items = new List<T>();

            foreach (T value in collection.ToList())
            {
                if (this.Any(i => AreEquals(i, value)))
                    continue;
                items.Add(value);
            }

            foreach (T item in items)
                item.Insert();
            base.InsertRange(index, items);
        }

        public new bool Remove(T item)
        {
            item.Delete();
            return base.Remove(item);
        }

        public new int RemoveAll(Predicate<T> match)
        {
            int count = 0;

            foreach (T item in this.Where(i=>match(i)).ToList())
            {
                Remove(item);
                count++;
                
            }
            return count;
        }

        public new void RemoveAt(int index)
        {
            this[index].Delete();
            base.RemoveAt(index);
        }

        public new void RemoveRange(int index, int count)
        {
            for (int i = 0; i < count; i++)
                RemoveAt(index + i);
        }

    }
}
