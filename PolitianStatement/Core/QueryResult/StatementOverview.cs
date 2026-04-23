namespace Core.QueryResult;

public record StatementOverview(
    int    Id,
    string Description,
    string Politician,
    string Category,
    int    CountRate1,
    int    CountRate2,
    int    CountRate3,
    int    CountRate4,
    int    CountRate5
);