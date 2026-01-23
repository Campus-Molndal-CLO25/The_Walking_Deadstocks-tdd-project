using MyZombieProject.App.Models;

namespace MyZombieProject.Test.Models
{

    public class ShelterTests
    {
        [Fact]
        public void GetSetPropertiesTest()
        {
            var shelter = new Shelter
            {
                Id = 1,
                Name = "Träsket",
                GpsCoordinates = "593293",
                Capacity = 10,
                Survivors = new List<Survivor>
                {
                    new Survivor { Id = 1, Name = "Pelle" },
                    new Survivor { Id = 2, Name = "Nisse" }
                }
            };

            Assert.Equal(1, shelter.Id);
            Assert.Equal("Träsket", shelter.Name);
            Assert.Equal("593293", shelter.GpsCoordinates);
            Assert.Equal(10, shelter.Capacity);
            Assert.NotNull(shelter.Survivors);
            Assert.Equal(2, shelter.Survivors.Count);
            Assert.Equal("Pelle", shelter.Survivors[0].Name);
            Assert.Equal("Nisse", shelter.Survivors[1].Name);
        }

        [Fact]
        public void DefaultValuesTest()
        {
            var shelter = new Shelter();

            Assert.Equal(0, shelter.Id);
            Assert.Equal(string.Empty, shelter.Name);
            Assert.Equal(string.Empty, shelter.GpsCoordinates);
            Assert.Equal(0, shelter.Capacity);
            Assert.NotNull(shelter.Survivors);
            Assert.Empty(shelter.Survivors);
        }

        [Fact]
        public void SurvivorsList_IsCorrect()
        {
            var shelter = new Shelter();
            var survivor = new Survivor { Id = 1, Name = "Allan" };

            shelter.Survivors.Add(survivor);

            Assert.Single(shelter.Survivors);
            Assert.Equal("Allan", shelter.Survivors[0].Name);
        }
    }
}
