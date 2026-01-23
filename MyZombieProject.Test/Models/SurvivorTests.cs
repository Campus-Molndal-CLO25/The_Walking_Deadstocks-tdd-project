using MyZombieProject.App.Models;

namespace MyZombieProject.Test.Models
{
    public class SurvivorTests
    {
        [Fact]
        public void GetSetPropertiesTest()
        {
            var shelter = new Shelter { Id = 1, Name = "Träsket" };
            var survivor = new Survivor
            {
                Id = 42,
                Name = "Ove",
                Age = 35,
                Health = 85,
                IsOnMission = true,
                ShelterId = shelter.Id,
                Shelter = shelter
            };

            Assert.Equal(42, survivor.Id);
            Assert.Equal("Ove", survivor.Name);
            Assert.Equal(35, survivor.Age);
            Assert.Equal(85, survivor.Health);
            Assert.True(survivor.IsOnMission);
            Assert.Equal(shelter.Id, survivor.ShelterId);
            Assert.NotNull(survivor.Shelter);
            Assert.Equal("Träsket", survivor.Shelter!.Name);
        }

        [Fact]
        public void DefaultValuesTest()
        {
            var survivor = new Survivor();

            Assert.Equal(0, survivor.Id);
            Assert.Equal(0, survivor.Age);
            Assert.Equal(string.Empty, survivor.Name);
            Assert.Null(survivor.Shelter);
            Assert.Null(survivor.ShelterId);
            Assert.Equal(100, survivor.Health);
            Assert.False(survivor.IsOnMission);
            Assert.False(survivor.IsInjured); // Health == 100
        }

        [Fact]
        public void IsInjured_ReturnsTrue_whenHealthUnder100()
        {
            var survivor = new Survivor { Health = 50 };

            Assert.True(survivor.IsInjured);
        }

        [Fact]
        public void IsInjured_ReturnsFalse_WhenHealthIs100()
        {
            var survivor = new Survivor { Health = 100 };

            Assert.False(survivor.IsInjured);
        }
    }
}
