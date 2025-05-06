using Bouchonnois.Domain;
using Bouchonnois.Domain.Common;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class TirerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{

    public void Handle(Guid id, string chasseur)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse == null)
        {
            throw new LaPartieDeChasseNexistePas();
        }

        if (partieDeChasse.Status != PartieStatus.Apéro)
        {
            if (partieDeChasse.Status != PartieStatus.Terminée)
            {
                var chasseurQuiTire = partieDeChasse.Chasseurs.FirstOrDefault(c => c.Nom == chasseur);
                if (chasseurQuiTire is not null)
                {
                    if (chasseurQuiTire.BallesRestantes == 0)
                    {
                        partieDeChasse.Events.Add(new Event(timeProvider(),
                            $"{chasseur} tire -> T'as plus de balles mon vieux, chasse à la main"));
                        repository.Save(partieDeChasse);

                        throw new TasPlusDeBallesMonVieuxChasseALaMain();
                    }

                    partieDeChasse.Events.Add(new Event(timeProvider(), $"{chasseur} tire"));
                    chasseurQuiTire.BallesRestantes--;
                }
                else
                {
                    throw new ChasseurInconnu(chasseur);
                }
            }
            else
            {
                partieDeChasse.Events.Add(new Event(timeProvider(),
                    $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
                repository.Save(partieDeChasse);

                throw new OnTirePasQuandLaPartieEstTerminée();
            }
        }
        else
        {
            partieDeChasse.Events.Add(new Event(timeProvider(),
                $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
            repository.Save(partieDeChasse);

            throw new OnTirePasPendantLapéroCestSacré();
        }

        repository.Save(partieDeChasse);
    }
}
