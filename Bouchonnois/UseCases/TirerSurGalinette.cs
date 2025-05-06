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
        public void HandleUnsafe(Request request)
        {
            var id = request.Id;
            var chasseur = request.Chasseur;
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
                        var chasseurQuiTire = partieDeChasse.Chasseurs.FirstOrDefault(c => c.Nom == chasseur);
                        if (chasseurQuiTire is not null)
                        {
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
                            throw new ChasseurInconnu(chasseur);
                        }
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

        public UnitResult<Error> Handle(Request command)
        {
            try
            {
                HandleUnsafe(command);
            }
            catch (LaPartieDeChasseNexistePas)
            {
                return Errors.LaPartieDeChasseNexistePas();
            }
            catch (TasPlusDeBallesMonVieuxChasseALaMain)
            {
                return Errors.TasPlusDeBallesMonVieuxChasseALaMain();
            }

            return UnitResult.Success<Error>();
        }
    }
}