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
            TableName = GetType().Name;
        }


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
                if (property.Value == null)
                {
                    result = null;
                    return true;
                }

                var id= (int)property.Value;
                result = EntityCache.Find(property.Type, id).FirstOrDefault().Value;
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
                var entity = (Entity)value;
                property.Value = entity.Self.Id;

                if (EntityCache == null)
                    EntityCache = entity.EntityCache;

                EntityCache.Add(entity);
                return true;
            }

            return true;
        }

        public IEntityCache EntityCache { get; set; }

        // Note: Not to happy with this one, but It'll do for now
        public string TableName { get; set; }

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

            EntityCache.Add(this);
        }

        protected void BelongsTo<T>(string propertyName = null, string columnName = null)
        {
            Properties.Add(new Property
                                  {
                                      PropertyType = PropertyType.BelongsTo,
                                      ColumnName = columnName ?? (propertyName ?? typeof(T).Name) + "_Id",
                                      PropertyName = propertyName ?? typeof(T).Name,
                                      Type = typeof(T)
                                  });
        }

        protected void HasMany<T>(string propertyName = null, string columnName = null) where T :Entity
        {
            Properties.Add(new Property
                                  {
                                      PropertyType = PropertyType.HasMany,
                                      ColumnName = columnName ?? GetType().Name + "_Id",
                                      PropertyName = propertyName ?? typeof(T).Name,
                                      Type = typeof(List<>).MakeGenericType(typeof(T))
                                  });
        }

        protected void Property<T>(string propertyName, string columnName)
        {
            Properties.Add(new Property
                               {
                                   PropertyType = PropertyType.Property,
                                   ColumnName = columnName,
                                   PropertyName = propertyName,
                                   Type = typeof(T)
                               });
        }
    }
}