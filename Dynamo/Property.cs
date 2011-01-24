using System;

namespace Dynamo
{
    public class Property
    {
        public PropertyType Type { get; set; }
        public string ColumnName { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public object Value { get; set; }
    }
}