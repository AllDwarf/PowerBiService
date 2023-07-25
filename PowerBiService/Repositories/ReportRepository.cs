using System.Text.RegularExpressions;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Group = Microsoft.PowerBI.Api.Models.Group;

namespace PowerBiService.Repositories;
public class ReportRepository : IReportRepository
{
    private readonly IPowerBIClient _client;
    public ReportRepository(IPowerBIClient client)
    {
        _client = client;
    }
    public async Task<Report> CloneReportAsync(Report report)
    {
        report = await _client.Reports.CloneReportAsync(report.Id, new CloneReportRequest());
        return report;
    }

    public async Task<Report> GetReportById(Guid id, Group group)
    {
        var report = await _client.Reports.GetReportInGroupAsync(group.Id, id);
        return report;
    }

    public async Task<Report> GetReportyByNameAsync(string name, Group group)
    {
        var reports = await _client.Reports.GetReportsAsync(group.Id);
        var report = reports.Value.FirstOrDefault(r => Regex.IsMatch(r.Name, name));
        if (report == null)
        {
            throw new Exception($"Report with name {name} not found");
        }
        return report;
    }

    public async Task<bool> RebindReportAsync(Report report, Dataset dataset)
    {
        // Rebind the report to the new dataset
        RebindReportRequest rebindReportRequest = new();
        rebindReportRequest.DatasetId = dataset.Id;
        rebindReportRequest.Validate();
        await _client.Reports.RebindReportAsync(report.Id, rebindReportRequest);
        return true;
    }

    public async Task<Report> ValidateAndUpdateReport(Report report)
    {
        //Refresh and validate given report
        var sourceReport = new SourceReport(report.Id);
        var updateRequest = new UpdateReportContentRequest(sourceReport);

        var updatedReport = await _client.Reports.UpdateReportContentAsync(report.Id, updateRequest);
        updatedReport.Validate();
        return updatedReport;
    }
}
