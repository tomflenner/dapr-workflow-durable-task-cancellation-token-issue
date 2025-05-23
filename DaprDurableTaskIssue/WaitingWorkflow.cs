using Dapr.Workflow;

namespace DaprDurableTaskIssue;

public class WaitingWorkflow : Workflow<object?, object?>
{
    public override async Task<object?> RunAsync(WorkflowContext context, object? input)
    {
        var logger = context.CreateReplaySafeLogger<WaitingWorkflow>();

        logger.LogInformation("Waiting for SIGNAL external event");
        await context.WaitForExternalEventAsync<object?>("SIGNAL");

        logger.LogInformation("Received SIGNAL external event");

        await context.CallActivityAsync(nameof(DummyActivity));

        return null;
    }
}