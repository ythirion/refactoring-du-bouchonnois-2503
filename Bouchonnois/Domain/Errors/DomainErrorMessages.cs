namespace Bouchonnois.Domain.Errors;

public static class DomainErrorMessages
{
    public const string OnEstDéjàEnTrainDePrendreLApéro = "On est déjà en train de prendre l'apéro";
    public const string OnNePrendPasLApéroQuandLaPartieEstTerminée = "On ne prend pas l'apéro quand la partie est terminée";
    public const string LaPartieDeChasseEstDejaEnCours = "La partie de chasse est déjà en cours";
    public const string QuandCestFiniCestFini = "Quand c'est fini c'est fini";
    public const string OnTirePasPendantLapéroCestSacré = "On tire pas pendant l'apéro, c'est sacré !!!";
    public const string OnTirePasQuandLaPartieEstTerminée = "On tire pas quand la partie est terminée";
    public const string TasPlusDeBallesMonVieuxChasseALaMain = "T'as plus de balles mon vieux, chasse à la main";
    public const string LeChasseurNestPasDansLaPartie = "Chasseur inconnu";
    public const string TasTropPicoléMonVieuxTasRienTouché = "T'as trop puicolé mon vieux t'as rien touché";
    public const string ImpossibleDeDémarrerUnePartieSansGalinettes = "Impossible de démarrer une partie sans galinettes";
    public const string ImpossibleDeDémarrerUnePartieSansChasseur = "Impossible de démarrer une partie sans chasseur";
    public const string ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle = "Impossible de démarrer une partie avec un chasseur sans balle";
    public const string IlNyAPlusDeGalinettesSurCeTerrain = "Il n'y a plus de galinettes sur ce terrain";
}
