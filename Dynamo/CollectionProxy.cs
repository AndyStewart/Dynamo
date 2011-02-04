using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dynamo
{
    public class CollectionProxy<T> : IList<T>  where T:Entity
    {
        private readonly Property hasManyProperty;
        private readonly Entity parentEntity;

        public CollectionProxy(Entity parentEntity, Property hasManyProperty)
        {
            this.parentEntity = parentEntity;
            this.hasManyProperty = hasManyProperty;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return getList().GetEnumerator();
        }

        private IList<T> getList()
        {
            int id = parentEntity.Self.Id;
            return parentEntity.EntityCache.Find(typeof (T), id).Select(q => q.Value).Cast<T>().ToList();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (item.Self.Id == null)
            {
                item.EntityCache = parentEntity.EntityCache;    
                item.EntityCache.Add(item);
                
                // Set Foriegn Key in Child
                var a = item.Properties.FirstOrDefault(q => q.ColumnName == hasManyProperty.ColumnName);
                if (a != null)
                    a.Value = item.Self.Id;
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return getList().Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            getList().CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return getList().Count; }
        }

        public bool IsReadOnly
        {
            get { return getList().IsReadOnly; }
        }

        public int IndexOf(T item)
        {
            return getList().IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            parentEntity.EntityCache.Add(item);
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get { return getList()[index]; }
            set { throw new NotImplementedException(); }
        }
    }
}