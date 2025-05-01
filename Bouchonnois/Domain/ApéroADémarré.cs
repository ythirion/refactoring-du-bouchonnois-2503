namespace Bouchonnois.Domain;

public sealed record ApéroADémarré(Guid Id, DateTime Date) : Event(Date, "Petit apéro")
{
    public override string ToString() => base.ToString();
}
