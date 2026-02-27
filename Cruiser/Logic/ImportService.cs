namespace Logic;

using System.Linq;
using System.Threading.Tasks;

using Logic.Entities;
using Logic.ImportData;
using Logic.Tools.CsvImport;

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

   }
}