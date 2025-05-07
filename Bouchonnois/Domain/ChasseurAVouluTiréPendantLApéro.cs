using Bouchonnois.Domain.Common;

namespace Bouchonnois.Domain;

public record ChasseurAVouluTiréPendantLApéro(DateTime Date, string Chasseur) :
    Event(Date, $"{Chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    
public record ChasseurSansBallesAVouluTiré(DateTime Date, string Chasseur) :
    Event(Date, $"{Chasseur} veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main");