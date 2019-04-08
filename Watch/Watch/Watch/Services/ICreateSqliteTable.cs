using SQLite;

namespace Watch.Services
{
    public interface ICreateSqliteTable
    {
        SQLiteConnection Create<T>();
    }
}
