using Core.Validations;

using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

using System;

using Base.Core.Entities;

public class Booking : EntityObject
{
    // TODO

    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}