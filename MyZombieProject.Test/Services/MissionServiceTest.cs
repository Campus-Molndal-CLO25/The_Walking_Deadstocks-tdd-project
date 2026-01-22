using System;
using Xunit;

public class MissionServiceTests
{
    [Fact]
    public void AddLog_AddsMessageToGlobalLog()
    {
        var service = new MissionService();


        service.AddLog("Test message");


        Assert.Single(service.GlobalLog);
        Assert.Contains("Test message", service.GlobalLog[0]);
    }

    [Fact]
    public void AddLog_InsertsNewestLogFirst()
    {

        var service = new MissionService();

        service.AddLog("First");
        service.AddLog("Second");

        // Assert
        Assert.Contains("Second", service.GlobalLog[0]);
        Assert.Contains("First", service.GlobalLog[1]);
    }

    [Fact]
    public void AddLog_KeepsOnly10LatestLogs()
    {
        // Arrange
        var service = new MissionService();

        // Act
        for (int i = 1; i <= 11; i++)
        {
            service.AddLog($"Log {i}");
        }

        // Assert
        Assert.Equal(10, service.GlobalLog.Count);
        Assert.DoesNotContain(service.GlobalLog, x => x.Contains("Log 1"));
    }
}
