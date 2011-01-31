using System;
using System.Collections.Generic;
using System.Linq;

namespace Dynamo
{
    public class EntityCache : IEntityCache
    {
        private readonly ISession session;
        private readonly IList<CachedEntity> entities;

        public EntityCache(ISession session)
        {
            this.session = session;
            entities = new List<CachedEntity>();
        }

        public void Add(Entity entity)
        {
            if (entity.Self.Id == null)
                session.Save(entity);
            
            var existingObject = ((IList<CachedEntity>)Find(entity.GetType(), entity.Self.Id)).FirstOrDefault();
            if (existingObject == null)
            {
                entities.Add(new CachedEntity {Id = entity.Self.Id, Type = entity.GetType(), Value = entity});
            }
            else
            {
                existingObject.Value = entity;
            }
        }

        public IList<CachedEntity> Find(Type type, int id)
        {
            return entities.Where(q => q.Type == type && q.Id == id).ToList();
        }
    }
}