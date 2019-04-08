using SQLite;

namespace Watch.Services
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
