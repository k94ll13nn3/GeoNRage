namespace GeoNRage.Models
{
    public class GameBase
    {
        public GameBase(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }
    }
}
