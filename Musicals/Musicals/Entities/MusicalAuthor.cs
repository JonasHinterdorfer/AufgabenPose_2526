/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: Musicals
 *--------------------------------------------------------------
 */

namespace Musicals.Entities;

public class MusicalAuthor : EntityObject
{
    public int MusicalId { get; set; }
    public Musical Musical { get; set; } = null!;

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    public int MusicalAuthorTypeId { get; set; }
    public MusicalAuthorType MusicalAuthorType { get; set; } = null!;
}
