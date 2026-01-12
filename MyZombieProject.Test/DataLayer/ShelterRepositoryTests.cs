namespace MyZombieProject.Test.DataLayer
{
    using Microsoft.EntityFrameworkCore;
    using MyZombieProject.App.Datalayer;
    using MyZombieProject.App.Datalayer.Repositories;
    using MyZombieProject.App.Models;
    using Xunit;

    public class ShelterRepositoryTests
    {
        [Fact]
        public void Add_Returns_Id()
        {
            using var context = CreateZombieDataContext();
            var repository = new ShelterRepository(context);

            var shelter = new Shelter
            {
                Name = "Träsket"
            };

            var result = repository.Add(shelter);

            Assert.True(result > 0); 
            Assert.Equal(1, context.Shelters.Count());
        }

        [Fact]
        public void GetById_Returns_Correct_Shelter()
        {
            using var context = CreateZombieDataContext();
            var repository = new ShelterRepository(context);
            var shelter = new Shelter { Name = "Sumpmarken" };
            context.Shelters.Add(shelter);
            context.SaveChanges();

            var result = repository.GetById(shelter.Id);

            Assert.NotNull(result);
            Assert.Equal("Sumpmarken", result.Name);
        }

        [Fact]
        public void GetAllShelters_Return_List()
        {
            using var context = CreateZombieDataContext();
            var repository = new ShelterRepository(context);
            context.Shelters.AddRange(
                new Shelter { Name = "Träsket" },
                new Shelter { Name = "Sumpmarken" }
            );
            context.SaveChanges();

            var result = repository.GetAllShelters();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Update_Do_Update()
        {
            using var context = CreateZombieDataContext();
            var repository = new ShelterRepository(context);

            var shelter = new Shelter { Name = "Träsket" };
            context.Shelters.Add(shelter);
            context.SaveChanges();
            
            shelter.Name = "Rishögen";
            repository.Update(shelter);

            var updatedShelter = context.Shelters.First();
            Assert.Equal("Rishögen", updatedShelter.Name);
        }

        [Fact]
        public void Delete_Removes_Shelter_From_Database()
        {
            using var context = CreateZombieDataContext();
            var repository = new ShelterRepository(context);

            var shelter = new Shelter { Name = "RödaRummet" };
            context.Shelters.Add(shelter);
            context.SaveChanges();

            repository.Delete(shelter.Id);

            Assert.Empty(context.Shelters);
        }

        private MyZombieDataContext CreateZombieDataContext()
        {
            var options = new DbContextOptionsBuilder<MyZombieDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new MyZombieDataContext(options);
        }
    }
}
