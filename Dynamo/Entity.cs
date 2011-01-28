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

        public Type Type { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = Properties.FirstOrDefault(q => q.PropertyName == binder.Name);
            if (property == null)
            {
                property = new Property { ColumnName = binder.Name, PropertyName = binder.Name, Type = binder.ReturnType };
                Properties.Add(property);
            }

            if (property.PropertyType == PropertyType.Property)
            {
                result = property.Value;
                return true;
            }

            if (property.PropertyType == PropertyType.BelongsTo)
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

                result = Session.GetById(property.Type, (int) property.Value);
                return true;
            }

            if (property.PropertyType == PropertyType.HasMany)
            {
                if (property.Value == null)
                {
                    var collectionProxyType = typeof (CollectionProxy<>).MakeGenericType(property.Type.GetGenericArguments()[0]);
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
                property = new Property { ColumnName = binder.Name, PropertyName = binder.Name, Type = binder.ReturnType, Value = value };
                Properties.Add(property);
            }
            
            if (property.PropertyType == PropertyType.Property)
                property.Value = value;

            if (property.PropertyType == PropertyType.BelongsTo)
            {
                property.Value = value;
                return true;
            }

            return true;
        }

        public ISession Session { get; set; }

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
                                       PropertyType = PropertyType.Property
                                   };
                    Properties.Add(property);
                }

                property.ColumnName = reader.GetName(i);
                property.Value = reader[i] == DBNull.Value ? null : reader[i];

                if (property.Value != null && property.PropertyType == PropertyType.Property)
                    property.Type = property.Value.GetType();
            }

            Session.Cache.Add(new CachedItem { Id = Self.Id, Type = GetType(), Value = this });
        }

        protected void BelongsTo(string propertyName)
        {
            Properties.Add(new Property
                                  {
                                      PropertyType = PropertyType.BelongsTo,
                                      ColumnName = propertyName + "_Id",
                                      PropertyName = propertyName,
                                      Type = GetType().Assembly.GetTypes().FirstOrDefault(q => q.Name == propertyName)
                                  });
        }

        protected void HasMany<T>(string propertyName) where T :Entity
        {
            Properties.Add(new Property
                                  {
                                      PropertyType = PropertyType.HasMany,
                                      ColumnName = GetType().Name + "_Id",
                                      PropertyName = propertyName,
                                      Type = typeof(List<>).MakeGenericType(typeof(T))
                                  });
        }
    }
}