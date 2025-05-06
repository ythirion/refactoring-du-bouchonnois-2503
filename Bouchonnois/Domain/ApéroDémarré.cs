namespace Bouchonnois.Domain;

public record ApéroDémarré(DateTime Date) : Event(Date, "Petit apéro")
{
    public override string ToString() => base.ToString();
}