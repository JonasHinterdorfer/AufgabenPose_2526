/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: Musicals
 *--------------------------------------------------------------
 */

namespace Musicals.Import.CsvEntities;

public class Musical
{
    public string  Name       { get; set; } = string.Empty;
    public int?    Year       { get; set; }
    public string? Characters { get; set; }
    public string? Content    { get; set; }
    public string? Author1    { get; set; }
    public string? Author2    { get; set; }
    public string? Author3    { get; set; }
}