namespace Core.Entities;

using Base.Core.Entities;

public class Move : EntityObject
{
    public int No { get; set; }

    public int Direction { get; set; }
    public int Speed     { get; set; }
    public int Duration  { get; set; }

    public int   RaceId { get; set; }
    public Race? Race { get; set; }

}