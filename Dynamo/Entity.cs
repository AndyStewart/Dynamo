using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace Dynamo
{
    public class Entity : DynamicObject, IEntity
    {
        public List<Property> Properties;

        public dynamic Self
        {
            get
            {
                return this;
            }
        }

        public Entity()
        {
            Properties = new List<Property>();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = Properties.FirstOrDefault(q => q.PropertyName == binder.Name);
            if (property == null)
            {
                property = new Property { ColumnName = binder.Name, PropertyName = binder.Name, PropertyType = binder.ReturnType };
                Properties.Add(property);
            }

            if (property.Type == PropertyType.Property)
            {
                result = property.Value;
                return true;
            }

            if (property.Type == PropertyType.BelongsTo)
            {
                result = property.Value != DBNull.Value ? Repository.GetById(property.PropertyType, (int) property.Value) : null;
                return true;
            }

            if (property.Type == PropertyType.HasMany)
            {
                var collectionProxyType = typeof (CollectionProxy<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                result = Activator.CreateInstance(collectionProxyType, this, property);
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var property = Properties.FirstOrDefault(q => q.PropertyName == binder.Name);
            
            if (property == null)
            {
                property = new Property { ColumnName = binder.Name, PropertyName = binder.Name, PropertyType = binder.ReturnType, Value = value };
                Properties.Add(property);
            }
            
            if (property.Type == PropertyType.Property)
                property.Value = value;

            if (property.Type == PropertyType.BelongsTo)
            {
                property.Value = ((dynamic) value);
                return true;
            }

            if (property.Type == PropertyType.HasMany)
            {
                Repository.Save((Entity) value);
                return true;
            }

            return true;
        }

        public IRepository Repository { get; set; }

        public void Populate(IDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var propertyName = reader.GetName(i);
                var property = Properties.FirstOrDefault(q => q.PropertyName == propertyName);

                if (property == null)
                    property = new Property();

                property.ColumnName = reader.GetName(i);
                property.Value = reader[i];
                property.Type = PropertyType.Property;
                property.PropertyType = property.Value.GetType();

                Properties.Add(property);
            }
        }

        protected void BelongsTo(string propertyName)
        {
            Properties.Add(new Property
                                  {
                                      Type = PropertyType.BelongsTo,
                                      ColumnName = propertyName + "_Id",
                                      PropertyName = propertyName,
                                      PropertyType = GetType().Assembly.GetTypes().FirstOrDefault(q => q.Name == propertyName)
                                  });
        }

        protected void HasMany<T>(string propertyName) where T :Entity
        {
            Properties.Add(new Property
                                  {
                                      Type = PropertyType.HasMany,
                                      ColumnName = GetType().Name + "_Id",
                                      PropertyName = propertyName,
                                      PropertyType = typeof(List<>).MakeGenericType(typeof(T))
                                  });
        }
    }

    public class CollectionProxy<T> : IList<T>  where T:Entity
    {
        private readonly Entity entity;
        private IList<T> innerList;

        public CollectionProxy(Entity entity, Property hasManyProperty)
        {
            this.entity = entity;

            var entityType = hasManyProperty.PropertyType.GetGenericArguments()[0];
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
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
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