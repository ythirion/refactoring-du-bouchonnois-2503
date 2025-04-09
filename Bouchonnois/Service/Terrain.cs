namespace Bouchonnois.Service
{
    public class Terrain
    {
        public Terrain(string nom, int nbGalinettes)
        {
            Nom = nom;
            NbGalinettes = nbGalinettes;
        }

        public string Nom { get; init; }
        public int NbGalinettes { get; set; }
    }
}
