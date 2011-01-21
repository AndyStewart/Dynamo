using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace ActiveRecord
{
    public class Entity : DynamicObject, IEntity
    {
        protected dynamic Self;
        public int Id
        {
            get
            {
                return (int) Properties["Id"];
            }
            set
            {
                Properties["Id"] = value;
            }
        }

        public Entity()
        {
            Properties = new Dictionary<string, object>();
            Self = this;
        }

        public Dictionary<string, object> Properties { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Properties[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Properties[binder.Name] = value;
            return true;
        }

        public void Populate(IDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                Properties.Add(reader.GetName(i), reader[i]);
            }
        }
    }
}