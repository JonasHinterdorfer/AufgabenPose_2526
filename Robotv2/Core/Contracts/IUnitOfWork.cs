namespace Core.Contracts;

using Base.Core.Contracts;

using Core.Contracts.Repositories;

public interface IUnitOfWork : IBaseUnitOfWork
{
    public ICompetitionRepository Competition { get; }
}