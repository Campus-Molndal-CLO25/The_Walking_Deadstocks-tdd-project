using Microsoft.EntityFrameworkCore;
using MyZombieProject.App.Models;

namespace MyZombieProject.App.Datalayer.Repositories
{
    public interface ISurvivorRepository
    {
        int Add(Survivor survivor);

        Survivor GetById(int id);

        void Update(Survivor survivor);

        void Delete(int id);

        List<Survivor> GetAllSurvivors();
    }

    public class SurvivorRepository : ISurvivorRepository
    {
        private readonly MyZombieDataContext _context;

        public SurvivorRepository(MyZombieDataContext context)
        {
            _context = context;
        }

        public int Add(Survivor survivor)
        {
            _context.Survivors.Add(survivor);
            _context.SaveChanges();

            return survivor.Id;
        }

        public Survivor GetById(int id)
        {
            return _context.Survivors.Single(s => s.Id == id);
        }

        public void Update(Survivor survivor)
        {
            _context.Entry(survivor).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var survivor = _context.Survivors.Single(s => s.Id == id);

            _context.Survivors.Remove(survivor);
            _context.SaveChanges();
        }

        public List<Survivor> GetAllSurvivors()
        {
            return _context.Survivors
                .Include(s => s.Shelter) 
                .AsNoTracking()
                .ToList();
        }
    }
}
