namespace Core.Contracts;

using Base.Core.Contracts;

public interface IUnitOfWork : IBaseUnitOfWork
{
    public ICompetitionRepository Competition { get; }
    public IDriverRepository Driver { get; }
    public IRaceRepository Race { get; }
    public IMoveRepository Move { get; }
}