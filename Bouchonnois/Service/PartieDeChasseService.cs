using Bouchonnois.Domain;

namespace Bouchonnois.Service
{
    public class PartieDeChasseService
    {
        private readonly DemarrerUnePartieDeChasseUseCase _demarrerUnePartieDeChasseUseCase;
        private readonly TirerUseCase _tirerUseCase;
        private readonly PrendreLapéroUseCase _prendreLapéroUseCase;
        private readonly ReprendreLaPartieUseCase _reprendreLaPartieUseCase;
        private readonly TerminerLaPartieUseCase _terminerLaPartieUseCase;
        private readonly ConsulterStatusUseCase _consulterStatusUseCase;

        public PartieDeChasseService(
            IPartieDeChasseRepository repository,
            Func<DateTime> timeProvider)
        {
            _demarrerUnePartieDeChasseUseCase = new DemarrerUnePartieDeChasseUseCase(repository, timeProvider);
            _tirerUseCase = new TirerUseCase(repository, timeProvider);
            _prendreLapéroUseCase = new PrendreLapéroUseCase(repository, timeProvider);
            _reprendreLaPartieUseCase = new ReprendreLaPartieUseCase(repository, timeProvider);
            _terminerLaPartieUseCase = new TerminerLaPartieUseCase(repository, timeProvider);
            _consulterStatusUseCase = new ConsulterStatusUseCase(repository);
        }

        public Guid Demarrer((string nom, int nbGalinettes) terrainDeChasse,
            List<(string nom, int nbBalles)> chasseurs) =>
            _demarrerUnePartieDeChasseUseCase.Demarrer(terrainDeChasse, chasseurs);

        public void Tirer(Guid id, string chasseur) => _tirerUseCase.Tirer(id, chasseur);

        public void PrendreLapéro(Guid id) => _prendreLapéroUseCase.PrendreLapéro(id);

        public void ReprendreLaPartie(Guid id) => _reprendreLaPartieUseCase.ReprendreLaPartie(id);

        public string TerminerLaPartie(Guid id) => _terminerLaPartieUseCase.TerminerLaPartie(id);

        public string ConsulterStatus(Guid id) => _consulterStatusUseCase.ConsulterStatus(id);
    }
}
