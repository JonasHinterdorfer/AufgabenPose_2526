using Core.Entities;

using System.ComponentModel.DataAnnotations;

namespace Core.Validations;

using System;

public class StatementRange : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        DateTime to        = (DateTime)value;
        var      statement = (Statement)validationContext.ObjectInstance;

        if (to <= statement.Created)
        {
            var result = new ValidationResult("To-Date must be after From-Date");
            return result;
        }

        return ValidationResult.Success;
    }
}
