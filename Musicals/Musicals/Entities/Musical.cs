/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: Musicals
 *--------------------------------------------------------------
 */

namespace Musicals.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Musical : EntityObject
{
    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    public int? Year { get; set; }

    public string? Content { get; set; }

    public ICollection<Character> Characters { get; set; } = new List<Character>();

    public ICollection<MusicalAuthor> MusicalAuthors { get; set; } = new List<MusicalAuthor>();
}
