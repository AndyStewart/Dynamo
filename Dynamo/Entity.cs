using System;
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
                var entity = property.Value as Entity;

                if (entity != null)
                {
                    result = entity;
                    return true;
                }

                if (property.Value == null)
                {
                    result = null;
                    return true;
                }

                result = Repository.GetById(property.PropertyType, (int) property.Value);
                return true;
            }

            if (property.Type == PropertyType.HasMany)
            {
                if (property.Value == null)
                {
                    var collectionProxyType = typeof (CollectionProxy<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                    property.Value = Activator.CreateInstance(collectionProxyType, this, property);
                }
                result = property.Value;
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
                property.Value = value;
                return true;
            }

            return true;
        }

        public IRepository Repository { get; set; }

        public void Populate(IDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var property = Properties.FirstOrDefault(q => q.ColumnName == columnName);

                if (property == null)
                {
                    property = new Property
                                   {
                                       PropertyName = reader.GetName(i),
                                       Type = PropertyType.Property
                                   };
                    Properties.Add(property);
                }

                property.ColumnName = reader.GetName(i);
                property.Value = reader[i] == DBNull.Value ? null : reader[i];

                if (property.Value != null && property.Type == PropertyType.Property)
                    property.PropertyType = property.Value.GetType();
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
}