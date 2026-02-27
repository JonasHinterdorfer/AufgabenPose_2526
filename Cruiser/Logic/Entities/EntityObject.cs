namespace Logic.Entities;

using System.ComponentModel.DataAnnotations;

public class EntityObject
{
    [Key]
    public int Id { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}