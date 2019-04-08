using System.IO;
using SQLite;
using Watch.Services;
using Watch.UWP.Services;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(Sqlite_Uwp))]
namespace Watch.UWP.Services
{
    public class Sqlite_Uwp : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var fileName = "Watch.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
            return new SQLiteConnection(path);
        }
    }
}
