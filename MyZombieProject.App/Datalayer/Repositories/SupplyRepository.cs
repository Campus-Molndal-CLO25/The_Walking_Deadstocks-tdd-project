using MyZombieProject.App.Models;

namespace MyZombieProject.App.Datalayer.Repositories
{
    public class SupplyRepository
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
            // 1. Hitta om det redan finns en instans av detta objekt i minnet
            var local = _context.Supplies
                .Local
                .FirstOrDefault(entry => entry.Id == supply.Id);

            // 2. Om den finns, säg åt EF att sluta bevaka den (Detach)
            if (local != null)
            {
                _context.Entry(local).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            // 3. Nu kan vi säkert be EF att börja bevaka vårt NYA objekt och sätta det som ändrat
            _context.Entry(supply).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

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
