namespace Bouchonnois.Domain;

public class Brocouille
{
    public override string ToString() => "Brocouille";

}

public static class BrocouilleExtensions
{
    public static string EventMessage(this Brocouille brocouille)
        => "La partie de chasse est terminée, vainqueur : Brocouille";
}
