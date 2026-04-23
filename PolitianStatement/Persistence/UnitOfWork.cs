using Core.Contracts;

namespace Persistence;

using Base.Persistence;

public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext dBContext) : base(dBContext)
    {
        Ratings    = new RatingRepository(dBContext);
        Categories = new CategoryRepository(dBContext);
        Statements = new StatementRepository(dBContext);
    }

    public ICategoryRepository  Categories { get; }
    public IRatingRepository    Ratings    { get; }
    public IStatementRepository Statements { get; }
}
