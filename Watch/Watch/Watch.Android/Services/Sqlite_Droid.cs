using System.IO;
using SQLite;
using Watch.Droid.Services;
using Watch.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Sqlite_Droid))]
namespace Watch.Droid.Services
{
    public class Sqlite_Droid : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var fileName = "Watch.db3";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, fileName);

            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}