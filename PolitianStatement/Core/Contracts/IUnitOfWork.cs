namespace Core.Contracts;

using Base.Core.Contracts;

public interface IUnitOfWork : IBaseUnitOfWork
{
    ICategoryRepository  Categories { get; }
    IRatingRepository    Ratings    { get; }
    IStatementRepository Statements { get; }
}