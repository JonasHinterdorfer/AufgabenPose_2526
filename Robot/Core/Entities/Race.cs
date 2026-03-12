using System;
using System.Collections.Generic;

namespace Core.Entities;

using Base.Core.Entities;

public class Race : EntityObject
{
    public DateTime RaceTime { get; set; }

    public int CompetitionId { get; set; }

    public int DriverId { get; set; }

    // navigation properties
    public Competition? Competition { get; set; }

    public Driver? Driver { get; set; }

    public IList<Move> Moves { get; set; } = new List<Move>();

}