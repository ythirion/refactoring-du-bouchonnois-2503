using Bouchonnois.Domain;

namespace Bouchonnois.Tests.DataBuilders;

public class ChasseurBuilder
{
    private string _nom = "Chasseur inconnu";
    private int _ballesRestantes;
    private int _nbGalinettes;
    public static Chasseur UnChasseurNomméDédé() => new ChasseurBuilder().Nommé("Dédé").AyantDesBallesRestantes(20).Build();
    public static Chasseur UnChasseurNomméBernard() => new ChasseurBuilder().Nommé("Bernard").AyantDesBallesRestantes(8).Build();
    public static Chasseur UnChasseurNomméRobert() => new ChasseurBuilder().Nommé("Robert").AyantDesBallesRestantes(12).Build();

    public ChasseurBuilder Nommé(string nom)
    {
        _nom = nom;
        return this;
    }

    public ChasseurBuilder AyantDesBallesRestantes(int ballesRestantes)
    {
        _ballesRestantes = ballesRestantes;
        return this;
    }

    public ChasseurBuilder AyantDesGalinettes(int galinettes)
    {
        _nbGalinettes = galinettes;
        return this;
    }

    public Chasseur Build()
    {
        return new Chasseur(
            nom: _nom,
            ballesRestantes: _ballesRestantes,
            nbGalinettes: _nbGalinettes);
    }
}
