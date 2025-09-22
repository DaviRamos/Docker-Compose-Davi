// ReSharper disable ALL

namespace SOLID;

// S - Single Responsibility Principle (SRP) 
// A class should have only one reason to change, it should have only one job or responsibility.
// BAD EXAMPLE
public class Report
{
    private string? _content;

    public void Generate()
    {
        // Simulate generating a report
        _content = "Report generated on " + DateTime.Now;
        Console.WriteLine("Report generated.");
    }

    public void SaveToFile()
    {
        // Save the report content to a text file
        File.WriteAllText("report.txt", _content);
        Console.WriteLine("Report saved to file.");
    }
}

// GOOD EXAMPLE
public class Report2
{
    public string Generate()
    {
        return "Report generated on " + DateTime.Now;
    }
}

public class FileReportSaver
{
    public void Save(string content)
    {
        File.WriteAllText("report.txt", content);
        Console.WriteLine("Report saved to file.");
    }
}

public class ReportManager
{
    private readonly Report2 _reportEngine;
    private readonly FileReportSaver _fileReportSaver;

    public ReportManager(Report2 reportEngine, FileReportSaver fileReportSaver)
    {
        _reportEngine = reportEngine;
        _fileReportSaver = fileReportSaver;
    }

    public void CreateAndSaveReport()
    {
        var content = _reportEngine.Generate();
        _fileReportSaver.Save(content);
    }
}

// BETTER EXAMPLE
public interface IReport
{
    string Generate();
}

public class Report3 : IReport
{
    public string Generate()
    {
        return "Report generated on " + DateTime.Now;
    }
}

public interface IReportSaver
{
    void Save(string content);
}

public class FileReportSaver2 : IReportSaver
{
    public void Save(string content)
    {
        File.WriteAllText("report.txt", content);
        Console.WriteLine("Report saved to file.");
    }
}

public class ReportManager2
{
    private readonly IReport _report;
    private readonly IReportSaver _saver;
    
    public ReportManager2(IReport report, IReportSaver saver)
    {
        _report = report;
        _saver = saver;
    }

    public void CreateAndSaveReport()
    {
        string content = _report.Generate();
        _saver.Save(content);
    }
}