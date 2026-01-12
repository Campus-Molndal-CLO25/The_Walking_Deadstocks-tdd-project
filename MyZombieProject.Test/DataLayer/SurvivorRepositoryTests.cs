using MyZombieProject.App.Datalayer.Repositories;
using MyZombieProject.App.Models;

namespace MyZombieProject.Test.DataLayer
{
    public class SurvivorRepositoryTests
    {
        [Fact]
        public void Add_Returns_Id()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SurvivorRepository(context);

            var survivor = new Survivor
            {
                Name = "Allan",
                Age = 15,
            };

            var result = repository.Add(survivor);

            Assert.True(result > 0);
            Assert.Equal(1, context.Survivors.Count());
        }

        [Fact]
        public void GetById_Returns_Survivor()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SurvivorRepository(context);
            var survivor = new Survivor
            {
                Name = "Allan",
                Age = 15,
            };

            context.Survivors.Add(survivor);
            context.SaveChanges();

            var result = repository.GetById(survivor.Id);

            Assert.NotNull(result);
            Assert.Equal("Allan", result.Name);
        }

        [Fact]
        public void GetAllShelters_Return_List()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SurvivorRepository(context);
            context.Survivors.AddRange(
                new Survivor { Name = "Allan", Age = 15 },
                new Survivor { Name = "Pelle", Age = 33}
            );
            context.SaveChanges();

            var result = repository.GetAllSurvivors();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Update_Do_Update()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SurvivorRepository(context);

            var survivor = new Survivor { Name = "Allan", Age = 15 };
            context.Survivors.Add(survivor);
            context.SaveChanges();

            survivor.Name = "Ove";
            survivor.Age = 17;
            repository.Update(survivor);

            var updatedSurvivor = context.Survivors.First();
            Assert.Equal("Ove", updatedSurvivor.Name);
            Assert.Equal(17, updatedSurvivor.Age);
        }

        [Fact]
        public void Delete_Do_Delete()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SurvivorRepository(context);

            var survivor = new Survivor { Name = "Allan", Age = 15 };
            context.Survivors.Add(survivor);
            context.SaveChanges();

            repository.Delete(survivor.Id);

            Assert.Empty(context.Shelters);
        }
    }
}
