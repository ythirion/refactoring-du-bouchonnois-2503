using Bouchonnois.Service;

namespace Bouchonnois.Domain
{
    public class PartieDeChasse
    {
        public PartieDeChasse(Guid id, PartieStatus status, List<Chasseur> chasseurs, Terrain terrain, List<Event> events)
        {
            Id = id;
            Status = status;
            Chasseurs = chasseurs;
            Terrain = terrain;
            Events = events;
        }

        public PartieDeChasse(Guid id, List<Chasseur> chasseurs, Terrain terrain, PartieStatus status)
        {
            Id = id;
            Chasseurs = chasseurs;
            Terrain = terrain;
            Status = status;
            Events = new List<Event>();
        }

        public Guid Id { get; set; }
        public List<Chasseur> Chasseurs { get; set; }
        public Terrain Terrain { get; set; }
        public PartieStatus Status { get; set; }
        public List<Event> Events { get; set; }
    }
}