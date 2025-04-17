using Bouchonnois.Domain;

namespace Bouchonnois.Tests.UseCases.DataBuilders;

public class ChasseurBuilder
{
    private readonly string _nomDuChassuer;
    private int _nbBalles;
    private int _nbGalinettes;

    public ChasseurBuilder(string nomDeChasseur)
    {
        _nomDuChassuer = nomDeChasseur;
    }

    public ChasseurBuilder AvecDesBalles(int nombreDeBalles)
    {
        _nbBalles = nombreDeBalles;
        return this;
    }

    public ChasseurBuilder AvecDesGalinettesDansSaBesace(int nombreDeGalinettes)
    {
        _nbGalinettes = nombreDeGalinettes;
        return this;
    }

    public Chasseur Build() => new(_nomDuChassuer, _nbBalles, _nbGalinettes);

    public static implicit operator Chasseur(ChasseurBuilder builder) => builder.Build();
}
