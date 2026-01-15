using MyZombieProject.App.Models;

namespace MyZombieProject.App.Datalayer.Repositories
{
    public interface ISupplyRepository
    {
        int Add(Supply supply);

        Supply GetById(int id);

        void Update(Supply supply);

        void Delete(int id);

        List<Supply> GetAllSupplies();
    }

    public class SupplyRepository : ISupplyRepository
    {
        private readonly MyZombieDataContext _context;

        public SupplyRepository(MyZombieDataContext context)
        {
            _context = context;
        }

        public int Add(Supply supply)
        {
            _context.Supplies.Add(supply);
            _context.SaveChanges();

            return supply.Id;
        }

        public Supply GetById(int id)
        {
            return _context.Supplies.Single(s => s.Id == id);
        }

        public void Update(Supply supply)
        {
            _context.Supplies.Update(supply);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var supply = _context.Supplies.Single(s => s.Id == id);

            _context.Supplies.Remove(supply);
            _context.SaveChanges();
        }

        public List<Supply> GetAllSupplies()
        {
            return _context.Supplies.ToList();
        }
    }
}
