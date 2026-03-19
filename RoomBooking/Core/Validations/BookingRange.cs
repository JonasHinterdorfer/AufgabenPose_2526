using Core.Entities;

using System.ComponentModel.DataAnnotations;

namespace Core.Validations;

using System;

public class BookingRange : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        DateTime to      = (DateTime)value;
        var      booking = (Booking)validationContext.ObjectInstance;
        if (to > booking.From)
        {
            return ValidationResult.Success;
        }
        else
        {
            var result = new ValidationResult("To-Date must be after From-Date");
            return result;
        }
    }
}