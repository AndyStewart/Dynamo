using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace Dynamo
{
    public class Entity : DynamicObject, IEntity
    {
        private List<Relationship> Relationships;

        public dynamic Self
        {
            get
            {
                return this;
            }
        }

        public Entity()
        {
            Properties = new Dictionary<string, object>();
            Relationships = new List<Relationship>();
        }

        public Dictionary<string, object> Properties { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Relationships.All(q => q.Property != binder.Name))
            {
                result = Properties[binder.Name];
                return true;
            }

            var findRelationship = Relationships.FirstOrDefault(q => q.Property == binder.Name && q.Type == RelationshipType.BelongsTo);
            if (findRelationship!= null)
            {
                var propertyType = GetType().Assembly.GetTypes().FirstOrDefault(q => q.Name == findRelationship.Property);
                
                if (Properties[findRelationship.ColumnName] != DBNull.Value)
                {
                    result = Repository.GetById(propertyType, (int) Properties[findRelationship.ColumnName]);
                }
                else
                {
                    result = null;
                }

                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (Relationships.All(q => q.Property != binder.Name))
                Properties[binder.Name] = value;

            var findRelationship = Relationships.FirstOrDefault(q => q.Property == binder.Name);
            if (findRelationship != null)
            {
                Properties[findRelationship.ColumnName] = ((dynamic) value).Id;
                return true;
            }

            return true;
        }

        public IRepository Repository { get; set; }

        public void Populate(IDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                Properties.Add(reader.GetName(i), reader[i]);
            }
        }

        protected void BelongsTo(string propertyName)
        {
            Relationships.Add(new Relationship
                                  {
                                      Type = RelationshipType.BelongsTo,
                                      ColumnName = propertyName + "_Id",
                                      Property = propertyName
                                  });
        }
    }
}