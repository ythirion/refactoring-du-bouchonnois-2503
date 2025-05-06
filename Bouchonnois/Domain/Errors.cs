namespace Bouchonnois.Domain;

public static class Errors
{
    public static Error LaPartieDeChasseNexistePas() 
        => new("La partie de chasse n'existe pas");

    public static Error OnEstDéjàEnTrainDePrendreLApéro() 
        => new("On est déjà en train de prendre l'apéro");

    public static Error OnNePrendPasLapéroQuandLaPartieEstTerminée()
        => new("On ne prend pas l'apéro quand la partie est terminée");
}