using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Abstractions;
using Bouchonnois.Tests.Builders;

namespace Bouchonnois.Tests.UseCases;

public class ReprendreLaPartieDeChasse : BaseTest
{
    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstALApero()
            .AvecDesChasseurs(new List<Chasseur>
            {
                Dédé,
                Bernard,
                new("Robert", 12)
            })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        service.ReprendreLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = Repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.EnCours);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Dédé");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be("Bernard");
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(0);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var reprendrePartieQuandPartieExistePas = () => service.ReprendreLaPartie(id);

        reprendrePartieQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var partieDeChasse = new PartieDeChasseBuilder()
            .AvecDesChasseurs(
                new List<Chasseur>
                {
                    Dédé,
                    Bernard,
                    new("Robert", 12)
                })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var reprendreLaPartieQuandChasseEnCours = () => service.ReprendreLaPartie(partieDeChasse.Id);

        reprendreLaPartieQuandChasseEnCours.Should()
            .Throw<LaChasseEstDéjàEnCours>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstTerminée()
            .AvecDesChasseurs(new List<Chasseur>
            {
                Dédé,
                Bernard,
                new("Robert", 12)
            }).Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var prendreLapéroQuandTerminée = () => service.ReprendreLaPartie(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
