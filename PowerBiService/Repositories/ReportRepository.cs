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
    public async Task<Stream> ExportReportStream(Report report)
    {
        //report = await _client.Reports.CloneReportAsync(report.Id, new CloneReportRequest());
        var reportStream = await _client.Reports.ExportReportAsync(report.Id);
        return reportStream;
    }

    public async Task<Import> ImportReportStream(Stream reportStream, Guid groupId)
    {
        // Start the import
        var importResult = await _client.Imports.PostImportWithFileAsync(groupId, reportStream, nameConflict: "CreateOrOverwrite");

        // Wait for the import to complete
        while (importResult.ImportState != "Succeeded" && importResult.ImportState != "Failed")
        {
            importResult = await _client.Imports.GetImportAsync(groupId, importResult.Id);
            await Task.Delay(1000);
        }

        // Check if the import succeeded
        if (importResult.ImportState != "Succeeded")
        {
            throw new Exception("Publish of .pbix file failed!");
        }
        return importResult;

    }

    public async Task<Report> GetReportById(Guid id, Group group)
    {
        var report = await _client.Reports.GetReportInGroupAsync(group.Id, id);
        return report;
    }

    public async Task<Report> GetReportyByNameAsync(string name, Group group)
    {
        var reports = await _client.Reports.GetReportsAsync(group.Id);
        var report = reports.Value.FirstOrDefault(r => Regex.IsMatch(r.Name, name)) ?? throw new Exception($"Report with name {name} not found");
        return report;
    }

    public async Task<bool> RebindReportAsync(Report report, Dataset dataset)
    {
        // Rebind the report to the new dataset
        RebindReportRequest rebindReportRequest = new()
        {
            DatasetId = dataset.Id
        };
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
