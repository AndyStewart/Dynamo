using System;

namespace Dynamo
{
    public class Property
    {
        public Type MappedType { get; set; }
        public PropertyType PropertyType { get; set; }
        public string ColumnName { get; set; }
        public string PropertyName { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return PropertyName + "=" + Value;
        }
    }
}