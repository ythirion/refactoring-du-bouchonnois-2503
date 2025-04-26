﻿using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class PrendreLAperoUseCase
{
    private readonly IPartieDeChasseRepository _repository;
    private readonly Func<DateTime> _timeProvider;

    public PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }
    public void PrendreLapéro(Guid id)
    {
        var partieDeChasse = _repository.GetById(id);

        if (partieDeChasse == null)
        {
            throw new LaPartieDeChasseNexistePas();
        }

        if (partieDeChasse.Status == PartieStatus.Apéro)
        {
            throw new OnEstDéjàEnTrainDePrendreLapéro();
        }
        else if (partieDeChasse.Status == PartieStatus.Terminée)
        {
            throw new OnPrendPasLapéroQuandLaPartieEstTerminée();
        }
        else
        {
            partieDeChasse.Status = PartieStatus.Apéro;
            partieDeChasse.Events.Add(new Event(_timeProvider(), "Petit apéro"));
            _repository.Save(partieDeChasse);
        }
    }
}
