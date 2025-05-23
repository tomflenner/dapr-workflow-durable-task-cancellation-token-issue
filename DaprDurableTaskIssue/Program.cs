using Dapr.Workflow;
using DaprDurableTaskIssue;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<WaitingWorkflow>();
    options.RegisterActivity<DummyActivity>();
});

var app = builder.Build();

app.MapPost("/start-workflow", async (ILogger<Program> logger, DaprWorkflowClient workflowClient) =>
{
    var workflowId = Guid.NewGuid();

    try
    {
        logger.LogInformation("Scheduling Workflow ID: {WorkflowId}", workflowId);
        await workflowClient.ScheduleNewWorkflowAsync(nameof(WaitingWorkflow), workflowId.ToString());
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to start workflow...");
        return Results.InternalServerError(ex);
    }

    logger.LogInformation("Workflow scheduled successfully with ID: {WorkflowId}", workflowId);


    try
    {
        logger.LogInformation("Waiting for Workflow ID: {WorkflowId} to start...", workflowId);
        await workflowClient.WaitForWorkflowStartAsync(workflowId.ToString());
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to wait for workflow start...");
        return Results.InternalServerError(ex);
    }

    logger.LogInformation("Workflow started successfully with ID: {WorkflowId}", workflowId);

    return Results.Ok(new { WorkflowId = workflowId});
});

app.MapPost("/raise-signal/{workflowId:guid}",
    async (Guid workflowId, DaprWorkflowClient workflowClient, ILogger<Program> logger) =>
    {
        try
        {
            logger.LogInformation("Raising SIGNAL for Workflow ID: {WorkflowId}", workflowId);
            await workflowClient.RaiseEventAsync(workflowId.ToString(), "SIGNAL");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to raise signal...");
            return Results.InternalServerError(ex);
        }

        logger.LogInformation("SIGNAL raised successfully for Workflow ID: {WorkflowId}", workflowId);

        return Results.Ok();
    });


await app.RunAsync();