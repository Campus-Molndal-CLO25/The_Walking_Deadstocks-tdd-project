using MyZombieProject.App.Datalayer.Repositories;
using MyZombieProject.App.Models;

namespace MyZombieProject.App.Datalayer
{
    public class DataFacade
    {
        private readonly ShelterRepository _shelterRepository;
        private readonly SurvivorRepository _survivorRepository;
        private readonly SupplyRepository _supplyRepository;

        public DataFacade(
            ShelterRepository shelterRepository,
            SurvivorRepository survivorRepository,
            SupplyRepository supplyRepository)
        {
            _shelterRepository = shelterRepository;
            _survivorRepository = survivorRepository;
            _supplyRepository = supplyRepository;
        }

        // Shelters

        public int AddShelter(Shelter shelter)
        {
            return _shelterRepository.Add(shelter);
        }

        public Shelter GetShelterById(int id)
        {
            return _shelterRepository.GetById(id);
        }

        public List<Shelter> GetAllShelters()
        {
            return _shelterRepository.GetAllShelters();
        }

        public void UpdateShelter(Shelter shelter)
        {
            _shelterRepository.Update(shelter);
        }

        public void DeleteShelter(int id)
        {
            _shelterRepository.Delete(id);
        }

        // Survivors 

        public int AddSurvivor(Survivor survivor)
        {
            return _survivorRepository.Add(survivor);
        }

        public Survivor GetSurvivorById(int id)
        {
            return _survivorRepository.GetById(id);
        }

        public List<Survivor> GetAllSurvivors()
        {
            return _survivorRepository.GetAllSurvivors();
        }

        public void UpdateSurvivor(Survivor survivor)
        {
            _survivorRepository.Update(survivor);
        }

        public void DeleteSurvivor(int id)
        {
            _survivorRepository.Delete(id);
        }

        // Supplies

        public int AddSupply(Supply supply)
        {
            return _supplyRepository.Add(supply);
        }

        public Supply GetSupplyById(int id)
        {
            return _supplyRepository.GetById(id);
        }

        public List<Supply> GetAllSupplies()
        {
            return _supplyRepository.GetAllSupplies();
        }

        public void UpdateSupply(Supply supply)
        {
            _supplyRepository.Update(supply);
        }

        public void DeleteSupply(int id)
        {
            _supplyRepository.Delete(id);
        }
    }

}
