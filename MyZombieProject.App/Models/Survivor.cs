namespace MyZombieProject.App.Models
{
    public class Survivor
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public string Name { get; set; } = string.Empty;

        public int? ShelterId { get; set; }

        public Shelter? Shelter { get; set; }
    }
}
