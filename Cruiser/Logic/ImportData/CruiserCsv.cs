namespace Logic.ImportData;

using Logic.Tools.CsvImport;

internal class CruiserCsv
{
    public required string Name { get; set; }
    public          int    BJ   { get; set; }

    [CsvImportFormat(Culture = "de")]
    public decimal? BRZ { get; set; }

    [CsvImportFormat(Culture = "de")]
    public decimal? Laenge { get; set; }

    [CsvImportFormat(Culture = "de")]
    public decimal? Kab { get; set; }

    [CsvImportFormat(Culture = "de")]
    public decimal? Pass { get; set; }

    [CsvImportFormat(Culture = "de")]
    public decimal? Bes { get; set; }

    public string? Reederei    { get; set; }
    public string? Bauklasse   { get; set; }
    public string? Bemerkungen { get; set; }
}