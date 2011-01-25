using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dynamo
{
    public class CollectionProxy<T> : IList<T>  where T:Entity
    {
        private readonly Entity entity;
        private IList<T> innerList;

        public CollectionProxy(Entity entity, Property hasManyProperty)
        {
            this.entity = entity;

            var entityType = hasManyProperty.Type.GetGenericArguments()[0];
            innerList = ((IList<object>)entity.Repository.FindBySql("Select * from " + entityType.Name + " Where " + hasManyProperty.ColumnName + "=" + entity.Self.Id)).Cast<T>().ToList();
        }


        public IEnumerator<T> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            entity.Repository.Save(item);
            innerList.Add(item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return innerList.IsReadOnly; }
        }

        public int IndexOf(T item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            entity.Repository.Save(item);
            innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get { return innerList[index]; }
            set { throw new NotImplementedException(); }
        }
    }
}