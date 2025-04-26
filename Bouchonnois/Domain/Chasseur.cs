namespace Bouchonnois.Domain;

public class Chasseur
{
    public Chasseur(string nom, int ballesRestantes, int nbGalinettes = 0)
    {
        Nom = nom;
        BallesRestantes = ballesRestantes;
        NbGalinettes = nbGalinettes;
    }

    public string Nom { get; }

    public int BallesRestantes { get; set; }

    public int NbGalinettes { get; set; }
}
