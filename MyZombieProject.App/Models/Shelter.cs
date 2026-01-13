namespace MyZombieProject.App.Models
{
    public class Shelter
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string GpsCoordinates { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public List<Survivor> Survivors { get; set; } = new();
    }
}
