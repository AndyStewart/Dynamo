using System;
using System.Collections.Generic;

namespace Dynamo
{
    public interface IEntityCache
    {
        void Add(Entity entity);
        IList<CachedEntity> Find(Type type, int id);
    }
}