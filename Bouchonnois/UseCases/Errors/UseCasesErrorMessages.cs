using Bouchonnois.Domain;

namespace Bouchonnois.UseCases.Errors;

public static class UseCasesErrorMessages
{
    public static Error LaPartieDeChasseNExistePas() => new("La partie de chasse n'existe pas");
}
