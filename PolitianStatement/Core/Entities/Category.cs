namespace Core.Entities;

using System.Collections.Generic;

using Base.Core.Entities;

public class Category : EntityObject
{
    public string Description { get; set; } = null!;

    public ICollection<Statement> Statements { get; set; } = new List<Statement>();
}