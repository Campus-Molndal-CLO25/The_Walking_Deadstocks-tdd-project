using MyZombieProject.App.Models;

namespace MyZombieProject.Test.Models
{
    public class SupplyTests
    {
        [Fact]
        public void GetSetPropertiesTest()
        {
            var supply = new Supply
            {
                Id = 42,
                Name = "Banan",
                Type = SupplyType.Food,
                AmountInStock = 100,
                ShelterId = 5
            };

            Assert.Equal(42, supply.Id);
            Assert.Equal("Banan", supply.Name);
            Assert.Equal(SupplyType.Food, supply.Type);
            Assert.Equal(100, supply.AmountInStock);
            Assert.Equal(5, supply.ShelterId);
        }

        [Fact]
        public void ShelterId_CanBeNull()
        {
            var supply = new Supply
            {
                ShelterId = null
            };

            Assert.Null(supply.ShelterId);
        }

        [Fact]
        public void DefaultValuesTest()
        {
            var supply = new Supply();

            Assert.Equal(0, supply.Id);
            Assert.Equal(0, supply.AmountInStock);
            Assert.Equal(string.Empty, supply.Name);
            Assert.Equal(default(SupplyType), supply.Type);
            Assert.Null(supply.ShelterId);
        }
    }
}
