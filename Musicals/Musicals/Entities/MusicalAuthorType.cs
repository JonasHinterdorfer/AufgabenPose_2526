/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: Musicals
 *--------------------------------------------------------------
 */

namespace Musicals.Entities;

using System.ComponentModel.DataAnnotations;

public class MusicalAuthorType : EntityObject
{
    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
}
