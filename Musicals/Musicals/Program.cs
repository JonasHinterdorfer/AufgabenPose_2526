/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: Musicals
 *--------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Musicals.Db;
using Musicals.Entities;
using Musicals.Import;

Console.WriteLine("Musicals");
Console.WriteLine("=====================");

await using (var dbContext = new MusicalsDbContext())
{
    Console.WriteLine("Delete Database");
    await dbContext.Database.EnsureDeletedAsync();
    Console.WriteLine("Migrate Database");
    await dbContext.Database.MigrateAsync();
}


#region Import

Console.WriteLine("=====================");
Console.WriteLine("Import musicals");
int countTotal = 0;
var import     = new ImportController();

foreach (var fullFileName in Directory.GetFiles("Import", "Musical*.csv"))
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    var count = await import.ImportMusicalsAsync(fullFileName);
    countTotal += count;

    stopwatch.Stop();

    Console.WriteLine($"Imported {fullFileName} / {count} in {stopwatch.Elapsed}");
}

Console.WriteLine($"Import done: {countTotal}");

Console.WriteLine("=====================");
Console.WriteLine("Import authors");
countTotal = 0;


foreach (var fullFileName in Directory.GetFiles("Import", "Authors*.csv"))
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    var count = await import.ImportAuthorsAsync(fullFileName);
    countTotal += count;

    stopwatch.Stop();

    Console.WriteLine($"Imported {fullFileName} / {count} in {stopwatch.Elapsed}");
}

Console.WriteLine($"Import done: {countTotal}");

#endregion

#region Queries

Console.WriteLine("=====================");

Console.WriteLine("All musicals:");

await using (var dbContext = new MusicalsDbContext())
{
    var musicals = await dbContext.Musicals
        .Include(m => m.Characters)
        .Include(m => m.MusicalAuthors)
            .ThenInclude(ma => ma.Author)
        .Include(m => m.MusicalAuthors)
            .ThenInclude(ma => ma.MusicalAuthorType)
        .OrderBy(m => m.Name)
        .ToListAsync();

    foreach (var musical in musicals)
    {
        Console.WriteLine(musical.Name);
        
        if (musical.Year.HasValue)
        {
            Console.WriteLine($"Year:\n {musical.Year}");
        }

        if (!string.IsNullOrEmpty(musical.Content))
        {
            Console.Write("Content:\n ");
            WrapText(musical.Content, 80, " ");
        }

        if (musical.Characters.Any())
        {
            var characters = string.Join(", ", musical.Characters.OrderBy(c => c.Name).Select(c => c.Name));
            Console.WriteLine($"Characters:\n {characters}");
        }

        // Group authors by type and display
        var authorsByType = musical.MusicalAuthors
            .GroupBy(ma => ma.MusicalAuthorType.Name)
            .OrderBy(g => g.Key);

        foreach (var group in authorsByType)
        {
            foreach (var musicalAuthor in group.OrderBy(ma => ma.Author.Name))
            {
                Console.WriteLine($"{group.Key}:\n {musicalAuthor.Author.Name}");
            }
        }
    }
}

#endregion

static void WrapText(string text, int maxLineLength, string indent)
{
    var words = text.Split(' ');
    var currentLine = "";
    var isFirstLine = true;

    foreach (var word in words)
    {
        var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
        var lineWithIndent = (isFirstLine ? "" : indent) + testLine;

        if (lineWithIndent.Length > maxLineLength && !string.IsNullOrEmpty(currentLine))
        {
            // Print current line
            Console.WriteLine((isFirstLine ? "" : indent) + currentLine);
            currentLine = word;
            isFirstLine = false;
        }
        else
        {
            currentLine = testLine;
        }
    }

    // Print last line
    if (!string.IsNullOrEmpty(currentLine))
    {
        Console.WriteLine((isFirstLine ? "" : indent) + currentLine);
    }
}
