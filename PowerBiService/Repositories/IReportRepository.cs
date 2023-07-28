using Microsoft.PowerBI.Api.Models;

namespace PowerBiService.Repositories;
public interface IReportRepository
{
    Task<Stream> ExportReportStream(Report report);
    Task<Import> ImportReportStream(Stream reportStream, Guid groupId);
    Task<Report> GetReportyByNameAsync(string name, Group group);
    Task<Report> GetReportById(Guid id, Group group);
    Task<bool> RebindReportAsync(Report report, Dataset dataset);
    Task<Report> ValidateAndUpdateReport(Report report);
}
