using Dapr.Workflow;

namespace DaprDurableTaskIssue;

public class DummyActivity(ILogger<DummyActivity> logger) : WorkflowActivity<object?, object?>
{
    public override async Task<object?> RunAsync(WorkflowActivityContext context, object? input)
    {
        logger.LogInformation("Doing some stuff...");
        await Task.Delay(1000);
        logger.LogInformation("Done doing some stuff...");

        return null;
    }
}