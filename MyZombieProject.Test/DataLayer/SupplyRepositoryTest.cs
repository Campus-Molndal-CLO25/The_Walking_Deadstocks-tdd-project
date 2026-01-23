using Microsoft.EntityFrameworkCore;
using MyZombieProject.App.Datalayer.Repositories;
using MyZombieProject.App.Models;


namespace MyZombieProject.Test.DataLayer
{
    public class SupplyRepositoryTest
    {
        [Fact]
        public void Add_ShouldAddSupplyAndReturnId()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SupplyRepository(context);

            var supply = new Supply
            {
                Name = "Bandage",
                Type = SupplyType.Medicine
            };

            var id = repository.Add(supply);

            Assert.True(id > 0);
            Assert.Equal(1, context.Supplies.Count());
        }

        [Fact]
        public void GetById_ShouldReturnCorrectSupply()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SupplyRepository(context);

            var supply = new Supply { Name = "Bandage", Type = SupplyType.Medicine };
            context.Supplies.Add(supply);
            context.SaveChanges();

            var result = repository.GetById(supply.Id);

            Assert.NotNull(result);
            Assert.Equal("Bandage", result.Name);
            Assert.Equal(SupplyType.Medicine, result.Type);
        }

        [Fact]
        public void Update_ShouldModifyExistingSupply()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SupplyRepository(context);

            var supply = new Supply { Name = "Bandage", Type = SupplyType.Medicine };
            context.Supplies.Add(supply);
            context.SaveChanges();

            supply.Name = "Hårda bangade";

            repository.Update(supply);

            var result = context.Supplies.Single(s => s.Id == supply.Id);
            Assert.Equal("Hårda bangade", result.Name);
        }

        [Fact]
        public void Delete_ShouldRemoveSupply()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SupplyRepository(context);

            var supply = new Supply { Name = "Aplesiner" };
            context.Supplies.Add(supply);
            context.SaveChanges();

            repository.Delete(supply.Id);

            Assert.Empty(context.Supplies);
        }

        [Fact]
        public void GetAllSupplies_ShouldReturnAllSupplies()
        {
            using var context = ZombieDataContextFactory.CreateZombieDataContext();
            var repository = new SupplyRepository(context);

            context.Supplies.AddRange(
                new Supply { Name = "Aplesiner" },
                new Supply { Name = "Bananer" }
            );
            context.SaveChanges();

            var result = repository.GetAllSupplies();

            Assert.Equal(2, result.Count);
        }
    }
}
