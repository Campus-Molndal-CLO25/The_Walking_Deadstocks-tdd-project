using Microsoft.EntityFrameworkCore;
using MyZombieProject.App.Datalayer;

namespace MyZombieProject.Test.DataLayer
{
    public class ZombieDataContextFactory
    {
        public static MyZombieDataContext CreateZombieDataContext()
        {
            var options = new DbContextOptionsBuilder<MyZombieDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new MyZombieDataContext(options);
        }
    }
}
