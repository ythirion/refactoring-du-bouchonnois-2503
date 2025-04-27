using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Service;

public class TirerUseCase
{
    private readonly IPartieDeChasseRepository _repository;
    private readonly Func<DateTime> _timeProvider;

    public TirerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public void Tirer(Guid id, string chasseur)
    {
        var partieDeChasse = _repository.GetById(id);

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
                        partieDeChasse.Events.Add(new Event(
                            _timeProvider(),
                            $"{chasseur} tire -> T'as plus de balles mon vieux, chasse à la main")
                        );
                        _repository.Save(partieDeChasse);

                        throw new TasPlusDeBallesMonVieuxChasseALaMain();
                    }

                    partieDeChasse.Events.Add(new Event(_timeProvider(), $"{chasseur} tire"));
                    chasseurQuiTire.BallesRestantes--;
                }
                else
                {
                    throw new ChasseurInconnu(chasseur);
                }
            }
            else
            {
                partieDeChasse.Events.Add(new Event(
                    _timeProvider(),
                    $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"
                ));
                _repository.Save(partieDeChasse);

                throw new OnTirePasQuandLaPartieEstTerminée();
            }
        }
        else
        {
            partieDeChasse.Events.Add(new Event(
                _timeProvider(),
                $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"
            ));
            _repository.Save(partieDeChasse);

            throw new OnTirePasPendantLapéroCestSacré();
        }

        _repository.Save(partieDeChasse);
    }
}
