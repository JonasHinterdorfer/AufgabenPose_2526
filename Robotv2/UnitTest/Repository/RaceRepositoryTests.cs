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
public sealed class RaceRepositoryTests : RepositoryTest
{
    #region crt and overrides

    public RaceRepositoryTests(RepositoryTestFixture testFixture) : base(testFixture)
    {
    }

    #endregion

    [Theory]
    [InlineData("Competition 1", "Maxi",           5)]
    [InlineData("Competition 2", "Maxi",           1)]
    [InlineData("Competition 2", "Driver No Race", 0)]
    public async Task RaceCountTest(string competitionName, string driverName, int expected)
    {
        var uow = GetUnitOfWork();

        var count = await uow.Race.GetRaceCount(driverName, competitionName);
        count.Should().Be(expected);
    }
}