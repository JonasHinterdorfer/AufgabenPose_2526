using System;

namespace Core.Entities;

using Base.Core.Entities;

using System.Collections.Generic;

public class Race : EntityObject
{
    public DateTime RaceTime { get; set; }

    public int          CompetitionId { get; set; }

    public int     DriverId { get; set; }

    //TODO navigation properties

}