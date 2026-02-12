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

public class Character : EntityObject
{
    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    public int MusicalId { get; set; }
    public Musical Musical { get; set; } = null!;
}
