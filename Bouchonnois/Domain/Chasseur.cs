using System.Diagnostics.CodeAnalysis;

namespace Bouchonnois.Domain
{
    public class Chasseur
    {
        [SetsRequiredMembers]
        public Chasseur(string nom, int ballesRestantes)
        {
            Nom = nom;
            BallesRestantes = ballesRestantes;
        }


        public Chasseur(string nom, int ballesRestantes, int nbGalinettes)
        {
            Nom = nom;
            BallesRestantes = ballesRestantes;
            NbGalinettes = nbGalinettes;
        }

        public string Nom { get; set; }
        public int BallesRestantes { get; set; }
        public int NbGalinettes { get; set; }
    }
}