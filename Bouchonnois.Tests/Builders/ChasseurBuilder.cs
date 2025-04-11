using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Builders;

public class ChasseurBuilder
{
    private int _ballesRestantes;
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

    public Chasseur Build() => new(_nom, _ballesRestantes);

    public static implicit operator Chasseur(ChasseurBuilder builder) => builder.Build();
}