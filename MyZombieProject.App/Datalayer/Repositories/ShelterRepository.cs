using Microsoft.EntityFrameworkCore;

using MyZombieProject.App.Models;

namespace MyZombieProject.App.Datalayer.Repositories
{
    public interface IShelterRepository
    {
        int Add(Shelter shelter);

        Shelter GetById(int id);

        void Update(Shelter shelter);

        void Delete(int id);

        List<Shelter> GetAllShelters();
    }

    public class ShelterRepository : IShelterRepository
    {
        private readonly MyZombieDataContext _context;

        public ShelterRepository(MyZombieDataContext context)
        {
            _context = context;
        }

        public int Add(Shelter shelter)
        {
            _context.Shelters.Add(shelter);
            _context.SaveChanges();

            return shelter.Id;
        }

        public Shelter GetById(int id)
        {
            return _context.Shelters
                .Include(s => s.Survivors)
                .Single(s => s.Id == id);
        }

        public void Update(Shelter shelter)
        {
            var local = _context.Shelters.Local.FirstOrDefault(entry => entry.Id == shelter.Id);

            if (local != null)
            {
                _context.Entry(local).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            _context.Entry(shelter).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var shelter = _context.Shelters.Single(s => s.Id == id);

            _context.Shelters.Remove(shelter);
            _context.SaveChanges();
        }

        public List<Shelter> GetAllShelters()
        {
            return _context.Shelters
                .Include(s => s.Survivors) 
                .AsNoTracking()
                .ToList();
        }
    }
}
