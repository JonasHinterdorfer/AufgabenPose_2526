/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: EF Demo
 *--------------------------------------------------------------
 */

namespace Musicals.Db;

using System.IO;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Musicals.Entities;

public class MusicalsDbContext : DbContext
{
    public string DbPath { get; } = System.IO.Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Musicals.db");

    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Musical> Musicals { get; set; } = null!;
    public DbSet<Character> Characters { get; set; } = null!;
    public DbSet<MusicalAuthor> MusicalAuthors { get; set; } = null!;
    public DbSet<MusicalAuthorType> MusicalAuthorTypes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasIndex(a => a.Name)
            .IsUnique();

        modelBuilder.Entity<Musical>()
            .HasIndex(m => m.Name)
            .IsUnique();

        modelBuilder.Entity<MusicalAuthorType>()
            .HasIndex(mat => mat.Name)
            .IsUnique();

        modelBuilder.Entity<MusicalAuthor>()
            .HasIndex(ma => new { ma.MusicalId, ma.AuthorId, ma.MusicalAuthorTypeId })
            .IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}