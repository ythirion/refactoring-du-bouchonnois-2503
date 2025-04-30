using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Service
{
    public class PartieDeChasseService
    {
        private readonly IPartieDeChasseRepository _repository;
        private readonly Func<DateTime> _timeProvider;

        public PartieDeChasseService(
            IPartieDeChasseRepository repository,
            Func<DateTime> timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public Guid Demarrer((string nom, int nbGalinettes) terrainDeChasse, List<(string nom, int nbBalles)> chasseurs)
        {
            if (terrainDeChasse.nbGalinettes <= 0)
            {
                throw new ImpossibleDeDémarrerUnePartieSansGalinettes();
            }

            var partieDeChasse = new PartieDeChasse(id: Guid.NewGuid(), status: PartieStatus.EnCours,
                chasseurs: new List<Chasseur>(),
                terrain: new Terrain(nom: terrainDeChasse.nom, terrainDeChasse.nbGalinettes),
                events: new List<Event>());

            foreach (var chasseur in chasseurs)
            {
                if (chasseur.nbBalles == 0)
                {
                    throw new ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle();
                }

                partieDeChasse.Chasseurs.Add(new Chasseur(nom: chasseur.nom, ballesRestantes: chasseur.nbBalles));
            }

            if (partieDeChasse.Chasseurs.Count == 0)
            {
                throw new ImpossibleDeDémarrerUnePartieSansChasseur();
            }

            string chasseursToString = string.Join(
                ", ",
                partieDeChasse.Chasseurs.Select(c => c.Nom + $" ({c.BallesRestantes} balles)")
            );

            partieDeChasse.Events.Add(new Event(_timeProvider(),
                $"La partie de chasse commence à {partieDeChasse.Terrain.Nom} avec {chasseursToString}")
            );

            _repository.Save(partieDeChasse);

            return partieDeChasse.Id;
        }

        public void ReprendreLaPartie(Guid id)
        {
            var partieDeChasse = _repository.GetById(id);

            if (partieDeChasse == null)
            {
                throw new LaPartieDeChasseNexistePas();
            }

            if (partieDeChasse.Status == PartieStatus.EnCours)
            {
                throw new LaChasseEstDéjàEnCours();
            }

            if (partieDeChasse.Status == PartieStatus.Terminée)
            {
                throw new QuandCestFiniCestFini();
            }

            partieDeChasse.Status = PartieStatus.EnCours;
            partieDeChasse.Events.Add(new Event(_timeProvider(), "Reprise de la chasse"));
            _repository.Save(partieDeChasse);
        }

        public string TerminerLaPartie(Guid id)
        {
            var partieDeChasse = _repository.GetById(id);

            var classement = partieDeChasse
                .Chasseurs
                .GroupBy(c => c.NbGalinettes)
                .OrderByDescending(g => g.Key)
                .ToList();

            if (partieDeChasse.Status == PartieStatus.Terminée)
            {
                throw new QuandCestFiniCestFini();
            }

            partieDeChasse.Status = PartieStatus.Terminée;

            string result;

            if (classement.All(group => group.Key == 0))
            {
                result = "Brocouille";
                partieDeChasse.Events.Add(
                    new Event(_timeProvider(), "La partie de chasse est terminée, vainqueur : Brocouille")
                );
            }
            else
            {
                var vainqueurs = classement[0];
                result = string.Join(", ", vainqueurs.Select(c => c.Nom));
                partieDeChasse.Events.Add(
                    new Event(_timeProvider(),
                        $"La partie de chasse est terminée, vainqueur : {string.Join(", ", vainqueurs.Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}"
                    )
                );
            }


            _repository.Save(partieDeChasse);

            return result;
        }

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
