using ControladoShopping.Data.Local.Models;
using SQLite;

namespace ControladoShopping.Data.Local
{
    public class VolumeScannerRepository : IDisposable
    {
        SQLiteAsyncConnection? database;

        public VolumeScannerRepository(string dbName)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName);
        }

        async Task Init()
        {
            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await database.CreateTableAsync<VolumeControlado>();
        }

        public async Task<VolumeControlado> GetVolume(string Siga, long Volume)
        {
            await Init();
            return await database.Table<VolumeControlado>().Where(i => i.Sigla == Siga && i.Volume == Volume).FirstOrDefaultAsync();
        }

        public async Task<int> CreateVolumeControlado(VolumeControlado volumeControlado)
        {
            await Init();
            return await database.InsertAsync(volumeControlado);
        }

        public async Task<List<VolumeControlado>> GetAllVolumeScanners()
        {
            await Init();
            return await database.Table<VolumeControlado>().ToListAsync();
        }

        public async Task<List<VolumeControlado>> GetVolumesNotSenders()
        {
            await Init();
            return await database.Table<VolumeControlado>().Where(v => v.Enviado == false).ToListAsync();
        }

        public async Task<int> UpdateVolumeScanner(VolumeControlado volumeControlado)
        {
            await Init();
            return await database.UpdateAsync(volumeControlado);
        }

        public void Dispose()
        {
            //database.Dispose();
        }
    }

    public static class Constants
    {
        public const string DatabaseFilename = "ColetorSQLite.db3"; //coletor.db

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}
