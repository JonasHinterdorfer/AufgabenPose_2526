﻿namespace Core.Entities;

using System.Collections.Generic;

using Base.Core.Entities;

public class Competition : EntityObject
{
    public string Name { get; set; } = string.Empty;

    // navigation properties
    public IList<Race> Races { get; set; } = new List<Race>();

}