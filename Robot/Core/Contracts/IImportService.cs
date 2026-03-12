namespace Core.Contracts;

using System.Threading.Tasks;

public interface IImportService
{
    Task<int> ImportRace(string driver, string competition, string moves);
}