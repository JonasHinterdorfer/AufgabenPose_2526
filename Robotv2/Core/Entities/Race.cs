using System;

namespace Core.Entities;

using Base.Core.Entities;

using System.Collections.Generic;

public class Race : EntityObject
{
    public DateTime RaceTime { get; set; }

    public int          CompetitionId { get; set; }
    public Competition? Competition { get; set; }

    public int     DriverId { get; set; }
    public Driver? Driver { get; set; }

    public ICollection<Move>? Moves { get; set; }
}