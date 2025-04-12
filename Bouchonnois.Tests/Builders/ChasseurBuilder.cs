using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Builders;

public class ChasseurBuilder
{
    private int _ballesRestantes;
    private int _nbGalinettes;
    private string _nom = "Chasseur Inconnu";

    public static ChasseurBuilder UnChasseur() => new();

    public static ChasseurBuilder Bernard() => UnChasseur().Nommé("Bernard").AyantDesBalles(8);

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

    public ChasseurBuilder AyantAttrapéGalinettes(int nbGalinettes)
    {
        _nbGalinettes = nbGalinettes;
        return this;
    }

    public ChasseurBuilder SansGalinettes() => AyantAttrapéGalinettes(0);

    public Chasseur Build() => new(_nom, _ballesRestantes, _nbGalinettes);

    public static implicit operator Chasseur(ChasseurBuilder builder) => builder.Build();
}