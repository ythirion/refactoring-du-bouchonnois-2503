namespace Bouchonnois.Domain;

public record ApéroDemarée(DateTime Date) : Event(Date, "Petit apéro")
{
    public override string ToString() => base.ToString();
}