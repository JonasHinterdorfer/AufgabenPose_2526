using Persistence;

using Xunit;

namespace UnitTest.Repository;

using Core.Contracts;

using Persistence.Repositories;

using RaceRepository = Persistence.RaceRepository;

[CollectionDefinition(nameof(MyRepositoryTests))]
public class MyRepositoryTests : ICollectionFixture<RepositoryTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[Collection(nameof(MyRepositoryTests))]
public abstract class RepositoryTest : RepositoryTestBase<ApplicationDbContext>
{
    protected RepositoryTest(RepositoryTestFixture testFixture) : base(testFixture)
    {
    }

    protected IUnitOfWork GetUnitOfWork()
    {
        var context        = TestFixture.CreateDbContext();
        var competitionRep = new CompetitionRepository(context);
        var driverRep      = new DriverRepository(context);
        var raceRep        = new RaceRepository(context);
        var moveRep        = new MoveRepository(context);
        var uow            = new UnitOfWork(context, competitionRep, driverRep, raceRep, moveRep);
        return uow;
    }
}