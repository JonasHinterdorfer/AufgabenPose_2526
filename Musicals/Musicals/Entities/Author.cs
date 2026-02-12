/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: Musicals
 *--------------------------------------------------------------
 */

namespace Musicals.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Author : EntityObject
{
    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(256)]
    public string? Born { get; set; }

    public ICollection<MusicalAuthor> MusicalAuthors { get; set; } = new List<MusicalAuthor>();
}
