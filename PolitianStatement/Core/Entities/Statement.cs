namespace Core.Entities;

using System;
using System.Collections.Generic;

using Base.Core.Entities;

public class Statement : EntityObject
{
    public string        Description   { get; set; } = null!;
    public string        Politician    { get; set; } = null!;
    public DateTime      Created       { get; set; }
    public DateTime?     Modified      { get; set; }
    public StatementType StatementType { get; set; }

    public int      CategoryId { get; set; }
    public Category Category   { get; set; } = null!;

    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}