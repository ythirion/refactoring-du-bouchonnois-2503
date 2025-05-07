using Bouchonnois.Domain.Common;

namespace Bouchonnois.Domain;

public record ApéroDémarré(DateTime Date) : Event(Date, "Petit apéro");