namespace MyZombieProject.Test.DataLayer
{
    using MyZombieProject.App.Datalayer;
    using MyZombieProject.App.Datalayer.Repositories;
    using MyZombieProject.App.Models;
    using NSubstitute;
    using Xunit;

    public class DataFacadeTests
    {
        private readonly IShelterRepository _shelterRepository;
        private readonly ISurvivorRepository _survivorRepository;
        private readonly ISupplyRepository _supplyRepository;
        private readonly DataFacade _dataFacade;

        public DataFacadeTests()
        {
            // Arrange (gemensam setup)
            _shelterRepository = Substitute.For<IShelterRepository>();
            _survivorRepository = Substitute.For<ISurvivorRepository>();
            _supplyRepository = Substitute.For<ISupplyRepository>();

            _dataFacade = new DataFacade(
                _shelterRepository,
                _survivorRepository,
                _supplyRepository);
        }

        // Shelters

        [Fact]
        public void AddShelter_Returns_Id()
        {
            var shelter = new Shelter();
            _shelterRepository.Add(shelter).Returns(11);

            var result = _dataFacade.AddShelter(shelter);

            Assert.Equal(11, result);
            _shelterRepository.Received(1).Add(shelter);
        }

        [Fact]
        public void GetShelterById_Returns_Shelter()
        {
            var shelter = new Shelter { Id = 12 };
            _shelterRepository.GetById(12).Returns(shelter);

            var result = _dataFacade.GetShelterById(12);

            Assert.Equal(shelter, result);
            _shelterRepository.Received(1).GetById(12);
        }

        [Fact]
        public void GetAllShelters_Returns_List()
        {
            var shelters = new List<Shelter> 
            {
                new Shelter { Id = 1, Name = "Träsket" },
                new Shelter { Id = 2, Name = "Sumpmarken" }
            };

            _shelterRepository.GetAllShelters().Returns(shelters);

            var result = _dataFacade.GetAllShelters();

            Assert.Equal(shelters, result);
            _shelterRepository.Received(1).GetAllShelters();
        }

        [Fact]
        public void UpdateShelter_Calls_Update()
        {
            var shelter = new Shelter { Id = 13 };

            _dataFacade.UpdateShelter(shelter);

            _shelterRepository.Received(1).Update(shelter);
        }

        [Fact]
        public void DeleteShelter_Calls_Delete()
        {
            var shelterId = 44;

            _dataFacade.DeleteShelter(shelterId);

            _shelterRepository.Received(1).Delete(shelterId);
        }

        // Survivors

        [Fact]
        public void AddSurvivor_Retuns_Id()
        {
            var survivor = new Survivor();
            _survivorRepository.Add(survivor).Returns(55);

            var result = _dataFacade.AddSurvivor(survivor);

            Assert.Equal(55, result);
            _survivorRepository.Received(1).Add(survivor);
        }

        [Fact]
        public void GetSurvivorById_Returns_Survivor()
        {
            var survivor = new Survivor() { Id = 12 };
            _survivorRepository.GetById(12).Returns(survivor);

            var result = _dataFacade.GetSurvivorById(12);

            Assert.Equal(survivor, result);
            _survivorRepository.Received(1).GetById(12);
        }

        [Fact]
        public void GetAllSurvivors_Returns_List()
        {
            var survivors = new List<Survivor>
            {
                new Survivor { Id = 1, Name = "Allan" },
                new Survivor { Id = 2, Name = "Pelle" }
            };

            _survivorRepository.GetAllSurvivors().Returns(survivors);

            var result = _dataFacade.GetAllSurvivors();

            Assert.Equal(survivors, result);
            _survivorRepository.Received(1).GetAllSurvivors();
        }

        [Fact]
        public void UpdateSurvivor_Calls_Update()
        {
            var survivor = new Survivor { Id = 13, Name = "Allan" };

            _dataFacade.UpdateSurvivor(survivor);

            _survivorRepository.Received(1).Update(survivor);
        }

        [Fact]
        public void DeleteSurvivor_Calls_Delete()
        {
            var survivorId = 44;

            _dataFacade.DeleteSurvivor(survivorId);

            _survivorRepository.Received(1).Delete(survivorId);
        }


        // Supplies

        [Fact]
        public void AddSupply_Returns_Id()
        {
            var supply = new Supply();
            _supplyRepository.Add(supply).Returns(11);

            var result = _dataFacade.AddSupply(supply);

            Assert.Equal(11, result);
            _supplyRepository.Received(1).Add(supply);
        }

        [Fact]
        public void GetSupplyById_Returns_Supply()
        {
            var supply = new Supply { Id = 12, Name = "Banan", Type = SupplyType.Food };
            _supplyRepository.GetById(12).Returns(supply);

            var result = _dataFacade.GetSupplyById(12);

            Assert.Equal(supply, result);
            _supplyRepository.Received(1).GetById(12);
        }

        [Fact]
        public void GetAllSupplies_Returns_List()
        {
            var supplies = new List<Supply>
            {
                new Supply { Id = 1, Name = "Patron", Type = SupplyType.Ammo },
                new Supply { Id = 2, Name = "Kanonkula", Type = SupplyType.Ammo }
            };

            _supplyRepository.GetAllSupplies().Returns(supplies);

            var result = _dataFacade.GetAllSupplies();

            Assert.Equal(supplies, result);
            _supplyRepository.Received(1).GetAllSupplies();
        }

        [Fact]
        public void UpdateSupply_Calls_Update()
        {
            var supply = new Supply { Id = 1, Name = "Patron", Type = SupplyType.Ammo };

            _dataFacade.UpdateSupply(supply);

            _supplyRepository.Received(1).Update(supply);
        }

        [Fact]
        public void DeleteSupply_Calls_Delete()
        {
            var supplyId = 10;

            _dataFacade.DeleteSupply(supplyId);

            _supplyRepository.Received(1).Delete(supplyId);
        }
    }

}
