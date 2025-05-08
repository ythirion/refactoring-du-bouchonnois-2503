namespace Bouchonnois.Domain;

public record Error(string Message)
{
    public static Error OnEstDéjàEnTrainDePrendreLApéroError() => new("On est déjà en train de prendre l'apéro");
    public static Error OnNePrendPasLApéroQuandLaPartieEstTerminéeError() => new("On ne prend pas l'apéro quand la partie est terminée");
    public static Error LaPartieDeChasseEstDejaEnCoursError() => new("La partie de chasse est déjà en cours");
    public static Error QuandCestFiniCestFiniError() => new("Quand c'est fini c'est fini");
    public static Error OnTirePasPendantLapéroCestSacréError() => new("On tire pas pendant l'apéro, c'est sacré !!!");
    public static Error OnTirePasQuandLaPartieEstTerminéeError() => new("On tire pas quand la partie est terminée");
    public static Error TasPlusDeBallesMonVieuxChasseALaMainError() => new("T'as plus de balles mon vieux, chasse à la main");
    public static Error LeChasseurNestPasDansLaPartieError() => new("Chasseur inconnu");
    public static Error TasTropPicoléMonVieuxTasRienTouchéError() => new("T'as trop puicolé mon vieux t'as rien touché");
    public static Error ImpossibleDeDémarrerUnePartieSansGalinettesError() => new("Impossible de démarrer une partie sans galinettes");
    public static Error ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalleError() => new("Impossible de démarrer une partie avec un chasseur sans balle");
    public static Error IlNyAPlusDeGalinettesSurCeTerrainError() => new("Il n'y a plus de galinettes sur ce terrain");
    public static Error ImpossibleDeDémarrerUnePartieSansChasseurError() => new("Impossible de démarrer une partie sans chasseur");
}
