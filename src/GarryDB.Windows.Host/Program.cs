using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GarryDb.Platform;
using GarryDb.Platform.Infrastructure;

namespace GarryDB.Windows.Host
{
    public static class Program
    {
        [STAThread]
        public static async Task Main()
        {
            var garry = new Garry(fileSystem: new WindowsFileSystem());
            await garry.StartAsync("C:\\Projects\\GarryDb\\Plugins");

            Debug.WriteLine("Ending application");
        }
    }
}