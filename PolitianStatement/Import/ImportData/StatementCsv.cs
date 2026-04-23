using Core.Validations;

namespace Import.ImportData;

using System;
using System.Collections.Generic;

using Base.Core.Entities;

public class StatementCsv
{
    public required string Category { get; set; } = null!;

    public required string Politician { get; set; }

    public required string Statement { get; set; }
}