using System.Collections.Generic;

namespace Watch.Services
{
    public interface ISqliteService<T>
    {
        void Add(T entity);
        void Delete(T entity);
        void DeleteAll();
        IEnumerable<T> Get();
        void Update(T entity);
    }
}
