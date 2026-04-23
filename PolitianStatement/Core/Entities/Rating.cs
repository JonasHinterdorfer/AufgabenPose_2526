namespace Core.Entities;

using Base.Core.Entities;

public class Rating : EntityObject
{
    public int     Rate      { get; set; }
    public string? Remark    { get; set; }
    public string  UserName  { get; set; } = null!;

    public int       StatementId { get; set; }
    public Statement Statement   { get; set; } = null!;
}