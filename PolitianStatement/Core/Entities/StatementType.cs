namespace Core.Entities;

public enum StatementType
{
    Standard, // A regular statement with no special features.
    Locked,   // A statement that is locked and cannot be accessed without proper authorization.
    AdultOnly // A statement that is intended for adult audiences and may contain mature content.
}
