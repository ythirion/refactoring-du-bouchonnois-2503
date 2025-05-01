using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.UseCases.Errors;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class TirerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{

    public UnitResult<Error> Handle(Guid id, string chasseur)
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
            if (chasseurQuiTire is null)
            {
                return UnitResult.Failure(new Error(UseCasesErrorMessages.LeChasseurNestPasDansLaPartie));
            }

            if (chasseurQuiTire.BallesRestantes == 0)
            {
                partieDeChasse.Events.Add(new Event(timeProvider(),
                    $"{chasseur} tire -> T'as plus de balles mon vieux, chasse à la main"));
                repository.Save(partieDeChasse);

                return UnitResult.Failure(new Error(DomainErrorMessages.TasPlusDeBallesMonVieuxChasseALaMain));
            }

            partieDeChasse.Events.Add(new Event(timeProvider(), $"{chasseur} tire"));
            chasseurQuiTire.BallesRestantes--;

            repository.Save(partieDeChasse);
            return UnitResult.Success<Error>();
        }
        catch
        {
            return UnitResult.Failure(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas));
        }
    }
}
