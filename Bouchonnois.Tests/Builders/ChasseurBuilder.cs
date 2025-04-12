using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Builders;

public class ChasseurBuilder
{
    private int _ballesRestantes;
    private int _nbGalinettes;
    private string _nom = "Chasseur Inconnu";

    public static ChasseurBuilder UnChasseur() => new();

    public static ChasseurBuilder Dédé() => UnChasseur().Nommé("Dédé").AyantDesBalles(20);
    public static ChasseurBuilder Bernard() => UnChasseur().Nommé("Bernard").AyantDesBalles(8);
    public static ChasseurBuilder Robert() => UnChasseur().Nommé("Robert").AyantDesBalles(12);

    public ChasseurBuilder Nommé(string nom)
    {
        _nom = nom;
        return this;
    }

    public ChasseurBuilder AyantDesBalles(int ballesRestantes)
    {
        _ballesRestantes = ballesRestantes;
        return this;
    }

    public ChasseurBuilder SansBalles() => AyantDesBalles(0);

    public ChasseurBuilder AyantCapturéGalinettes(int nbGalinettes)
    {
        _nbGalinettes = nbGalinettes;
        return this;
    }

    public ChasseurBuilder Brocouille() => AyantCapturéGalinettes(0);

    public Chasseur Build() => new(_nom, _ballesRestantes, _nbGalinettes);

    public static implicit operator Chasseur(ChasseurBuilder builder) => builder.Build();
}