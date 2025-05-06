namespace Bouchonnois.Domain.Exceptions;

public class ChasseurInconnu(string chasseur) : Exception($"Chasseur inconnu {chasseur}");
