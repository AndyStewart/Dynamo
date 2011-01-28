using System.Collections.Generic;

namespace Dynamo
{
    public interface IQuery
    {
        IList<object> ToList();
        IQuery Where(string condition = null, object paramaters = null);
        int Count(string property);
        IQuery OrderBy(string orderClause);
        IQuery Include<T1>() where T1 : Entity;
    }
}