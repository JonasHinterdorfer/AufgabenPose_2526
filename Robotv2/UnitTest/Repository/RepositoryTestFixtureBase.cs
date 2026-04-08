using Microsoft.EntityFrameworkCore;

namespace UnitTest.Repository;

public abstract class RepositoryTestFixtureBase<TDbContext> where TDbContext : DbContext
{
    protected RepositoryTestFixtureBase()
    {
    }

    public abstract TDbContext CreateDbContext();
}