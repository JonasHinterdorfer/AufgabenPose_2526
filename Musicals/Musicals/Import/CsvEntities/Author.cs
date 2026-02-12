/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: Musicals
 *--------------------------------------------------------------
 */

namespace Musicals.Import.CsvEntities;

using System;

public class Author
{
    public string Name { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Born { get; set; }
}