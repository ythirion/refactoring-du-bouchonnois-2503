using Bouchonnois.Domain;
using Bouchonnois.Domain.Common;
using Bouchonnois.UseCases.Common;
using Bouchonnois.UseCases.Exceptions;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases;

public static class TirerSurGalinette
{
    public record Request(Guid Id, string Chasseur) : IRequest;

    public class UseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
        : IUseCase<Request, UnitResult<Error>>
    {
        public UnitResult<Error> Handle(Request request)
        {
            var id = request.Id;
            var chasseur = request.Chasseur;
            var partieDeChasse = repository.GetById(id);

            if (partieDeChasse == null)
            {
                return Errors.LaPartieDeChasseNexistePas();
            }

            if (partieDeChasse.Terrain.NbGalinettes == 0)
            {
                return Errors.TasTropPicoléMonVieuxTasRienTouché();
            }
            
            var chasseurQuiTire = partieDeChasse.Chasseurs.FirstOrDefault(c => c.Nom == chasseur);
            if (chasseurQuiTire is null)
            {
                return Errors.ChasseurInconnu(chasseur);
            }

            if (partieDeChasse.Status == PartieStatus.Apéro)
            {
                partieDeChasse.Events.Add(
                    new Event(
                        timeProvider(),
                        $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
                repository.Save(partieDeChasse);
                return Errors.OnTirePasPendantLapéroCestSacré();
            }

            if (partieDeChasse.Status == PartieStatus.Terminée)
            {
                partieDeChasse.Events.Add(
                    new Event(
                        timeProvider(),
                        $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
                repository.Save(partieDeChasse);

                return Errors.OnTirePasQuandLaPartieEstTerminée();
            }

            

            if (chasseurQuiTire.BallesRestantes == 0)
            {
                partieDeChasse.Events.Add(
                    new Event(
                        timeProvider(),
                        $"{chasseur} veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main"));
                repository.Save(partieDeChasse);

                return Errors.TasPlusDeBallesMonVieuxChasseALaMain();
            }

            chasseurQuiTire.BallesRestantes--;
            chasseurQuiTire.NbGalinettes++;
            partieDeChasse.Terrain.NbGalinettes--;
            partieDeChasse.Events.Add(new Event(timeProvider(), $"{chasseur} tire sur une galinette"));

            repository.Save(partieDeChasse);

            return UnitResult.Success<Error>();
        }
    }
}