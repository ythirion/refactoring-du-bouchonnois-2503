using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Service
{
    public class PartieDeChasseService
    {
        private readonly IPartieDeChasseRepository _repository;
        private readonly DemarrerUnePartieDeChasseUseCase _demarrerUnePartieDeChasseUseCase;
        private readonly TirerUseCase _tirerUseCase;
        private readonly PrendreLapéroUseCase _prendreLapéroUseCase;
        private readonly ReprendreLaPartieUseCase _reprendreLaPartieUseCase;
        private readonly TerminerLaPartieUseCase _terminerLaPartieUseCase;

        public PartieDeChasseService(
            IPartieDeChasseRepository repository,
            Func<DateTime> timeProvider)
        {
            _repository = repository;
            _terminerLaPartieUseCase = new TerminerLaPartieUseCase(repository, timeProvider);
            _reprendreLaPartieUseCase = new ReprendreLaPartieUseCase(repository, timeProvider);
            _prendreLapéroUseCase = new PrendreLapéroUseCase(repository, timeProvider);
            _tirerUseCase = new TirerUseCase(repository, timeProvider);
            _demarrerUnePartieDeChasseUseCase = new DemarrerUnePartieDeChasseUseCase(repository, timeProvider);
        }

        public Guid Demarrer((string nom, int nbGalinettes) terrainDeChasse,
            List<(string nom, int nbBalles)> chasseurs) =>
            _demarrerUnePartieDeChasseUseCase.Demarrer(terrainDeChasse, chasseurs);

        public void Tirer(Guid id, string chasseur) => _tirerUseCase.Tirer(id, chasseur);

        public void PrendreLapéro(Guid id) => _prendreLapéroUseCase.PrendreLapéro(id);

        public void ReprendreLaPartie(Guid id) => _reprendreLaPartieUseCase.ReprendreLaPartie(id);

        public string TerminerLaPartie(Guid id) => _terminerLaPartieUseCase.TerminerLaPartie(id);

        public string ConsulterStatus(Guid id)
        {
            var partieDeChasse = _repository.GetById(id);

            if (partieDeChasse == null)
            {
                throw new LaPartieDeChasseNexistePas();
            }

            return string.Join(
                Environment.NewLine,
                partieDeChasse.Events
                    .OrderByDescending(@event => @event.Date)
                    .Select(@event => @event.ToString())
            );
        }
    }
}
