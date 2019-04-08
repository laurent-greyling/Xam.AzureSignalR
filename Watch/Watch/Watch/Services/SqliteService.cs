using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Watch.Services
{
    public class SqliteService<T> : ISqliteService<T> where T : new()
    {
        readonly SQLiteConnection _table;

        public SqliteService()
        {
            _table = DependencyService.Get<ICreateSqliteTable>().Create<T>();
        }

        public void Add(T entity)
        {
            _table.Insert(entity);
        }

        public void Delete(T entity)
        {
            _table.Delete(entity);
        }

        public void DeleteAll()
        {
            _table.DeleteAll<T>();
        }

        public IEnumerable<T> Get()
        {
            return (from tbl in _table.Table<T>() select tbl).ToList();
        }

        public void Update(T entity)
        {
            _table.InsertOrReplace(entity);
        }
    }
}
