namespace Persistence;

using Core.Contracts;

using Base.Persistence;

public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
{
    public ApplicationDbContext? ApplicationDbContext => BaseApplicationDbContext as ApplicationDbContext;

    public UnitOfWork(
        ApplicationDbContext   applicationDbContext,
        ICompetitionRepository competition,
        IDriverRepository     driver,
        IRaceRepository       race,
        IMoveRepository       move
    ) : base(applicationDbContext)
    {
        Competition = competition;
        Driver = driver;
        Race = race;
        Move = move;
    }

    public ICompetitionRepository Competition { get; }
    public IDriverRepository Driver { get; }
    public IRaceRepository Race { get; }
    public IMoveRepository Move { get; }
}