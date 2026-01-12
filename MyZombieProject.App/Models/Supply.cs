using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyZombieProject.App.Models
{
    public enum SupplyType { Food, Ammo, Medicine }

    public class Supply
    {
        public int Id { get; set; }

        public int AmountInStock { get; set; }

        public string Name { get; set; } = string.Empty;

        public SupplyType Type { get; set; }
    }
}
