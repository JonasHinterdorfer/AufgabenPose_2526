﻿namespace Logic;

using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Logic.Entities;
using Logic.ImportData;
using Logic.Tools.CsvImport;

using Microsoft.EntityFrameworkCore;

public class ImportService : IImportService
{
    private readonly ApplicationDbContext _dbContext;

    public ImportService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ImportDbAsync()
    {
        var cruiserCsv = await new CsvImport<CruiserCsv>().ReadAsync("ImportData/Schiffe.txt");

        // Build a dictionary of shipping companies to avoid duplicates
        var companyDict = new Dictionary<string, ShippingCompany>(StringComparer.OrdinalIgnoreCase);

        foreach (var csv in cruiserCsv)
        {
            // Create or reuse ShippingCompany
            ShippingCompany? company = null;
            if (!string.IsNullOrWhiteSpace(csv.Reederei))
            {
                var companyName = csv.Reederei.Trim();
                if (!companyDict.TryGetValue(companyName, out company))
                {
                    company = new ShippingCompany { Name = companyName };
                    _dbContext.Companies.Add(company);
                    companyDict[companyName] = company;
                }
            }

            // Parse Bemerkungen: extract old ship names starting with "ex "
            var remark = csv.Bemerkungen ?? "";
            var shipNames = new List<string>();

            // Split by comma and look for "ex " entries
            var parts = SplitBemerkungen(remark);
            var remainingParts = new List<string>();

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (trimmed.StartsWith("ex ", StringComparison.OrdinalIgnoreCase))
                {
                    var oldName = trimmed.Substring(3).Trim();
                    if (!string.IsNullOrWhiteSpace(oldName))
                    {
                        shipNames.Add(oldName);
                    }
                }
                else
                {
                    remainingParts.Add(trimmed);
                }
            }

            var cleanedRemark = string.Join(",", remainingParts).Trim();
            // Remove trailing/leading commas
            cleanedRemark = cleanedRemark.Trim(',').Trim();

            var ship = new CruiseShip
            {
                Name               = csv.Name,
                YearOfConstruction = csv.BJ == 0 ? null : csv.BJ,
                Tonnage            = csv.BRZ.HasValue ? (int)csv.BRZ.Value : null,
                Length             = csv.Laenge,
                Cabins             = csv.Kab.HasValue ? (int)csv.Kab.Value : null,
                Passengers         = csv.Pass.HasValue ? (int)csv.Pass.Value : null,
                Crew               = csv.Bes.HasValue ? (int)csv.Bes.Value : null,
                Remark             = cleanedRemark,
                ShippingCompany    = company,
            };

            _dbContext.Ships.Add(ship);

            // Add old ship names
            foreach (var oldName in shipNames)
            {
                var shipName = new ShipName
                {
                    Name       = oldName,
                    CruiseShip = ship,
                };
                _dbContext.ShipNames.Add(shipName);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    private static List<string> SplitBemerkungen(string bemerkungen)
    {
        if (string.IsNullOrWhiteSpace(bemerkungen))
            return new List<string>();

        // Split by comma, but "ex " always starts a new token
        var result = new List<string>();
        var current = "";

        var parts = bemerkungen.Split(',');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.StartsWith("ex ", StringComparison.OrdinalIgnoreCase))
            {
                // Save current if not empty
                if (!string.IsNullOrWhiteSpace(current))
                {
                    result.Add(current.Trim());
                }
                current = trimmed;
            }
            else
            {
                if (string.IsNullOrEmpty(current))
                {
                    current = trimmed;
                }
                else
                {
                    current += "," + part;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(current))
        {
            result.Add(current.Trim());
        }

        return result;
    }
}