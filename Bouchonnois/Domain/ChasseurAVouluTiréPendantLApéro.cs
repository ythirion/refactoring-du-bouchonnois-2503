using Bouchonnois.Domain.Common;

namespace Bouchonnois.Domain;

public record ChasseurAVouluTiréPendantLApéro(DateTime Date, string Chasseur) :
    Event(Date, $"{Chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");