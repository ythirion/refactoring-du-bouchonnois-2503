using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Assertions;
using Bouchonnois.Tests.DataBuilders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class ReprendreLaPartieDeChasse
{
    private readonly PartieDeChasseRepositoryForTests _repository;
    private readonly PartieDeChasseService _service;

    public ReprendreLaPartieDeChasse()
    {
        _repository = new PartieDeChasseRepositoryForTests();
        _service = new PartieDeChasseService(_repository, () => DateTime.Now);
    }

    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var partieDeChasse = new PartieDeChasseBuilder().QuandLesChasseursSontALapero().AvecDesChasseurs().Build();
        _repository.Add(partieDeChasse);

        _service.ReprendreLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = _repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.BeEnCours();//Status.Should().Be(PartieStatus.EnCours);
        savedPartieDeChasse.Terrain.Nom.Should().Be(Data.Terrain);
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be(Data.Dede);
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be(Data.Bernard);
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be(Data.Robert);
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);

        savedPartieDeChasse.Chasseurs.All(chasseur => chasseur.NbGalinettes == 0).Should().BeTrue();
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var reprendrePartieQuandPartieExistePas = () => _service.ReprendreLaPartie(Guid.NewGuid());

        reprendrePartieQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        _repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var partieDeChasse = new PartieDeChasseBuilder().AvecDesChasseurs().Build();
        _repository.Add(partieDeChasse);

        var reprendreLaPartieQuandChasseEnCours = () => _service.ReprendreLaPartie(partieDeChasse.Id);

        reprendreLaPartieQuandChasseEnCours.Should()
            .Throw<LaChasseEstDéjàEnCours>();

        _repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var partieDeChasse = new PartieDeChasseBuilder().QuandLaPartieDeChasseEstTerminee().AvecDesChasseurs().Build();
        _repository.Add(partieDeChasse);

        var prendreLapéroQuandTerminée = () => _service.ReprendreLaPartie(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        _repository.SavedPartieDeChasse().Should().BeNull();
    }
}
