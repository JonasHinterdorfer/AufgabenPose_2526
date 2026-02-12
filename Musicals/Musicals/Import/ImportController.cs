/*--------------------------------------------------------------
 *				HTBLA-Leonding / Class: 4xHIF
 *--------------------------------------------------------------
 *              Musterlösung-HA
 *--------------------------------------------------------------
 * Description: Musicals
 *--------------------------------------------------------------
 */

namespace Musicals.Import;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using Musicals.Db;
using Musicals.Entities;
using Musicals.Tools;

public class ImportController
{
    public async Task<int> ImportMusicalsAsync(string fileName)
    {
        await using (var dbContext = new MusicalsDbContext())
        {
            var musicalsCsv = await new CsvImport<CsvEntities.Musical>()
                {
                    Encoding = Encoding.Unicode
                }
                .ReadAsync(fileName);

            foreach (var csvMusical in musicalsCsv)
            {
                var musical = await dbContext.Musicals
                    .Include(m => m.Characters)
                    .Include(m => m.MusicalAuthors)
                        .ThenInclude(ma => ma.MusicalAuthorType)
                    .Include(m => m.MusicalAuthors)
                        .ThenInclude(ma => ma.Author)
                    .FirstOrDefaultAsync(m => m.Name == csvMusical.Name);

                if (musical == null)
                {
                    musical = new Musical
                    {
                        Name = csvMusical.Name
                    };
                    dbContext.Musicals.Add(musical);
                }

                if (csvMusical.Year.HasValue)
                {
                    musical.Year = csvMusical.Year;
                }
                if (!string.IsNullOrEmpty(csvMusical.Content))
                {
                    musical.Content = csvMusical.Content;
                }

                if (!string.IsNullOrEmpty(csvMusical.Characters))
                {
                    var characterNames = csvMusical.Characters
                        .Split(',')
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList();

                    var charactersToRemove = musical.Characters
                        .Where(c => !characterNames.Contains(c.Name))
                        .ToList();
                    foreach (var character in charactersToRemove)
                    {
                        dbContext.Characters.Remove(character);
                    }

                    foreach (var characterName in characterNames)
                    {
                        if (!musical.Characters.Any(c => c.Name == characterName))
                        {
                            musical.Characters.Add(new Character
                            {
                                Name = characterName,
                                Musical = musical
                            });
                        }
                    }
                }

                var authorFields = new[] { csvMusical.Author1, csvMusical.Author2, csvMusical.Author3 }
                    .Where(a => !string.IsNullOrEmpty(a))
                    .ToList();

                var authorsByType = new Dictionary<string, List<string>>();

                foreach (var authorField in authorFields)
                {
                    var parts = authorField.Split(':');
                    if (parts.Length == 2)
                    {
                        var typeName = parts[0].Trim();
                        var authorNames = parts[1]
                            .Split(new[] { " and " }, StringSplitOptions.None)
                            .Select(n => n.Trim())
                            .Where(n => !string.IsNullOrEmpty(n))
                            .ToList();

                        if (!authorsByType.ContainsKey(typeName))
                        {
                            authorsByType[typeName] = new List<string>();
                        }
                        authorsByType[typeName].AddRange(authorNames);
                    }
                }

                foreach (var kvp in authorsByType)
                {
                    var typeName = kvp.Key;
                    var authorNames = kvp.Value;
                    // Get or create MusicalAuthorType - check both database and tracked entities
                    var authorType = await dbContext.MusicalAuthorTypes
                        .FirstOrDefaultAsync(mat => mat.Name == typeName);
                    
                    if (authorType == null)
                    {
                        // Check if the type is already tracked but not yet saved
                        authorType = dbContext.MusicalAuthorTypes.Local
                            .FirstOrDefault(mat => mat.Name == typeName);
                    }
                    
                    if (authorType == null)
                    {
                        authorType = new MusicalAuthorType { Name = typeName };
                        dbContext.MusicalAuthorTypes.Add(authorType);
                    }

                    var musicalAuthorsToRemove = musical.MusicalAuthors
                        .Where(ma => ma.MusicalAuthorType.Name == typeName || ma.MusicalAuthorTypeId == authorType.Id)
                        .Where(ma => ma.Author != null && !authorNames.Contains(ma.Author.Name))
                        .ToList();
                    foreach (var musicalAuthor in musicalAuthorsToRemove)
                    {
                        if (musicalAuthor.Id > 0)
                        {
                            dbContext.MusicalAuthors.Remove(musicalAuthor);
                        }
                        else
                        {
                            musical.MusicalAuthors.Remove(musicalAuthor);
                        }
                    }

                    foreach (var authorName in authorNames){
                        // Get or create Author - check both database and tracked entities
                        var author = await dbContext.Authors
                            .FirstOrDefaultAsync(a => a.Name == authorName);
                        
                        if (author == null)
                        {
                            // Check if the author is already tracked but not yet saved
                            author = dbContext.Authors.Local
                                .FirstOrDefault(a => a.Name == authorName);
                        }
                        
                        if (author == null)
                        {
                            author = new Author { Name = authorName };
                            dbContext.Authors.Add(author);
                        }

                        var existingLink = musical.MusicalAuthors
                            .FirstOrDefault(ma => 
                                ma.Author.Name == authorName && 
                                (ma.MusicalAuthorType.Name == typeName || ma.MusicalAuthorTypeId == authorType.Id));

                        if (existingLink == null)
                        {
                            musical.MusicalAuthors.Add(new MusicalAuthor
                            {
                                Musical = musical,
                                Author = author,
                                MusicalAuthorType = authorType
                            });
                        }
                    }
                }
            }

            await dbContext.SaveChangesAsync();
            return musicalsCsv.Count;
        }
    }

    public async Task<int> ImportAuthorsAsync(string fileName)
    {
        await using (var dbContext = new MusicalsDbContext())
        {
            var authorsCsv = await new CsvImport<CsvEntities.Author>()
                {
                    DateFormat = "MMMM d, yyyy",
                    TimeFormat = string.Empty,
                    Encoding   = Encoding.Unicode
                }
                .ReadAsync(fileName);

            foreach (var csvAuthor in authorsCsv)
            {
                var author = await dbContext.Authors
                    .FirstOrDefaultAsync(a => a.Name == csvAuthor.Name);

                if (author == null)
                {
                    author = new Author
                    {
                        Name = csvAuthor.Name
                    };
                    dbContext.Authors.Add(author);
                }

                if (csvAuthor.DateOfBirth.HasValue)
                {
                    author.DateOfBirth = csvAuthor.DateOfBirth;
                }
                if (!string.IsNullOrEmpty(csvAuthor.Born))
                {
                    author.Born = csvAuthor.Born;
                }
            }

            await dbContext.SaveChangesAsync();
            return authorsCsv.Count;
        }
    }

}