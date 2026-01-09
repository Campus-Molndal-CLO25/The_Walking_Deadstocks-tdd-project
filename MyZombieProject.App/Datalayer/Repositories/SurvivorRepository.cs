using Microsoft.EntityFrameworkCore;
using MyZombieProject.App.Models;

namespace MyZombieProject.App.Datalayer.Repositories
{
    public class SurvivorRepository
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
            _context.Survivors.Update(survivor);
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
            return _context.Survivors.ToList();
        }
    }
}
