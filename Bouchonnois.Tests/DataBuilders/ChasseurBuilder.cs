using Bouchonnois.Domain;

namespace Bouchonnois.Tests.DataBuilders;

public class ChasseurBuilder
{
    private string _nom = "Chasseur inconnu";
    private int _ballesRestantes;
    private int _nbGalinettes;

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
