namespace Bouchonnois.UseCases.Exceptions
{
    public class ChasseurInconnu : Exception
    {
        public ChasseurInconnu(string chasseur)
            : base($"Chasseur inconnu {chasseur}")
        {
        }
    }
}
