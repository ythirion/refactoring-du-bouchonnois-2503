namespace Bouchonnois.UseCases.Exceptions;

public class ChasseurInconnu(string chasseur) : Exception($"Chasseur inconnu {chasseur}");
