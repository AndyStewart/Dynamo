using System;

namespace Dynamo
{
    public class CachedEntity
    {
        public int Id { get; set; }
        public Type Type { get; set; }
        public Entity Value { get; set; }
    }
}