using Bouchonnois.Domain;

namespace Bouchonnois.Service
{
    public class PartieDeChasseService
    {
        private readonly DemarrerUnePartieDeChasseUseCase _demarrerUnePartieDeChasseUseCase;
        private readonly TirerUseCase _tirerUseCase;
        private readonly PrendreLapéroUseCase _prendreLapéroUseCase;
        private readonly ReprendreLaPartieDeChasseUseCase _reprendreLaPartieDeChasseUseCase;
        private readonly TerminerLaPartieDeChasseUseCase _terminerLaPartieDeChasseUseCase;
        private readonly ConsulterStatusUseCase _consulterStatusUseCase;

        public PartieDeChasseService(
            IPartieDeChasseRepository repository,
            Func<DateTime> timeProvider)
        {
            _consulterStatusUseCase = new ConsulterStatusUseCase(repository);
            _terminerLaPartieDeChasseUseCase = new TerminerLaPartieDeChasseUseCase(repository, timeProvider);
            _reprendreLaPartieDeChasseUseCase = new ReprendreLaPartieDeChasseUseCase(repository, timeProvider);
            _prendreLapéroUseCase = new PrendreLapéroUseCase(repository, timeProvider);
            _tirerUseCase = new TirerUseCase(repository, timeProvider);
            _demarrerUnePartieDeChasseUseCase = new DemarrerUnePartieDeChasseUseCase(repository, timeProvider);
        }

        public Guid Demarrer((string nom, int nbGalinettes) terrainDeChasse, List<(string nom, int nbBalles)> chasseurs)
            => _demarrerUnePartieDeChasseUseCase.Demarrer(terrainDeChasse, chasseurs);

        public void Tirer(Guid id, string chasseur) => _tirerUseCase.Tirer(id, chasseur);

        public void PrendreLapéro(Guid id) => _prendreLapéroUseCase.PrendreLapéro(id);

        public void ReprendreLaPartie(Guid id) => _reprendreLaPartieDeChasseUseCase.ReprendreLaPartie(id);

        public string TerminerLaPartie(Guid id) => _terminerLaPartieDeChasseUseCase.TerminerLaPartie(id);

        public string ConsulterStatus(Guid id) => _consulterStatusUseCase.ConsulterStatus(id);
    }
}
