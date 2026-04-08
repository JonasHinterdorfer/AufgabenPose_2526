namespace Persistence;

using Core.Contracts;

using Base.Persistence;

using Core.Contracts.Repositories;

public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
{
    public ApplicationDbContext? ApplicationDbContext => BaseApplicationDbContext as ApplicationDbContext;

    public UnitOfWork(
        ApplicationDbContext   applicationDbContext,
        ICompetitionRepository competition
    ) : base(applicationDbContext)
    {
        Competition = competition;
        //TODO Init
    }

    public ICompetitionRepository Competition { get; }
    //TODO properties
}