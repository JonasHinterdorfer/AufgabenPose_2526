/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 1xHIF
 *--------------------------------------------------------------
 *              Musterlösung
 *--------------------------------------------------------------
 * Description: Robot UnitTest
 *--------------------------------------------------------------
 */

using System;
using System.Linq;
using System.Threading.Tasks;

using Core.Contracts;
using Core.Entities;

using FluentAssertions;

using Persistence;

using Xunit;

namespace UnitTest.Repository;

[Collection(nameof(MyRepositoryTests))]
public sealed class CompetitionRepositoryTests : RepositoryTest
{
    #region crt and overrides

    public CompetitionRepositoryTests(RepositoryTestFixture testFixture) : base(testFixture)
    {
    }

    #endregion

    [Fact]
    public async Task ReadAllTest()
    {
        var uow = GetUnitOfWork();

        var all = await uow.Competition.GetAsync(null, null, nameof(Competition.Races), "Races.Driver", "Races.Moves");

        // Check Mapping!

        var competition      = "Competition 1";
        var firstCompetition = all.First(c => c.Name == competition);
        firstCompetition.Name.Should().Be(competition);

        firstCompetition.Races.Should().HaveCount(9);
        var raceTime  = new DateTime(2024, 1, 8, 11, 42, 10);
        var firstRace = firstCompetition.Races!.First(r => r.RaceTime == raceTime);
        firstRace.RaceTime.Should().Be(raceTime);
        firstRace.Driver!.Name.Should().Be("Maxi");
        firstRace.Competition!.Name.Should().Be(competition);

        competition      = "Competition 2";
        firstCompetition = all.First(c => c.Name == competition);
        firstCompetition.Name.Should().Be(competition);

        firstCompetition.Races.Should().HaveCount(1);
        raceTime  = new DateTime(2023, 1, 8, 11, 42, 10);
        firstRace = firstCompetition.Races!.First(r => r.RaceTime == raceTime);
        firstRace.Driver!.Name.Should().Be("Maxi");
        firstRace.Competition!.Name.Should().Be(competition);
    }

    [Fact]
    public async Task GetEmptyCompetitionsTest()
    {
        var uow = GetUnitOfWork();

        var comp = await uow.Competition.GetAllWithRace();
        comp.Should().HaveCount(2);
    }
}