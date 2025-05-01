using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.UseCases.Errors;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

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
                var chasseurQuiTire = partieDeChasse.GetChasseurs().FirstOrDefault(c => c.Nom == chasseur);
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

    public UnitResult<Error> HandleWithoutException(Guid id, string chasseur)
    {
        try
        {
            var partieDeChasse = repository.GetById(id);

            if (partieDeChasse.Status == PartieStatus.Apéro)
            {
                partieDeChasse.Events.Add(new Event(timeProvider(),
                    $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
                repository.Save(partieDeChasse);

                return UnitResult.Failure(new Error(DomainErrorMessages.OnTirePasPendantLapéroCestSacré));
            }

            if (partieDeChasse.Status == PartieStatus.Terminée)
            {
                partieDeChasse.Events.Add(new Event(timeProvider(),
                    $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
                repository.Save(partieDeChasse);

                return UnitResult.Failure(new Error(DomainErrorMessages.OnTirePasQuandLaPartieEstTerminée));
            }

            var chasseurQuiTire = partieDeChasse.GetChasseurs().FirstOrDefault(c => c.Nom == chasseur);
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

            repository.Save(partieDeChasse);
            return UnitResult.Success<Error>();
        }
        catch
        {
            return UnitResult.Failure(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas));
        }
    }
}
