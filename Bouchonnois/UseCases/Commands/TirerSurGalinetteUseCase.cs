using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases.Commands;

public class TirerSurGalinetteUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{

    public void Handle(Guid id, string chasseur)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse == null)
        {
            throw new LaPartieDeChasseNexistePas();
        }

        if (partieDeChasse.Terrain.NbGalinettes != 0)
        {
            if (partieDeChasse.Status != PartieStatus.Apéro)
            {
                if (partieDeChasse.Status != PartieStatus.Terminée)
                {
                    var result = partieDeChasse.GetChasseur(chasseur);
                    if (result.IsFailure)
                    {
                        throw new ChasseurInconnu(chasseur);
                    }

                    var chasseurQuiTire = result.Value;

                    if (chasseurQuiTire.BallesRestantes == 0)
                    {
                        partieDeChasse.Events.Add(
                            new Event(
                                timeProvider(),
                                $"{chasseur} veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main"));
                        repository.Save(partieDeChasse);

                        throw new TasPlusDeBallesMonVieuxChasseALaMain();
                    }

                    chasseurQuiTire.BallesRestantes--;
                    chasseurQuiTire.NbGalinettes++;
                    partieDeChasse.Terrain.NbGalinettes--;
                    partieDeChasse.Events.Add(new Event(timeProvider(), $"{chasseur} tire sur une galinette"));
                }
                else
                {
                    partieDeChasse.Events.Add(
                        new Event(
                            timeProvider(),
                            $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
                    repository.Save(partieDeChasse);

                    throw new OnTirePasQuandLaPartieEstTerminée();
                }
            }
            else
            {
                partieDeChasse.Events.Add(
                    new Event(
                        timeProvider(),
                        $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
                repository.Save(partieDeChasse);
                throw new OnTirePasPendantLapéroCestSacré();
            }
        }
        else
        {
            throw new TasTropPicoléMonVieuxTasRienTouché();
        }

        repository.Save(partieDeChasse);
    }
}
