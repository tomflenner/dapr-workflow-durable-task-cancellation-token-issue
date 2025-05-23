# Dapr Workflow CancellationToken Bug Reproduction (.NET 9 + Dapr.Workflow 1.16.0-rc01)

This project demonstrates a potential bug in `Dapr.Workflow` version `1.16.0-rc01` related to `CancellationToken`
behavior in a workflow running on .NET 9.

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### Running the Project

To start the project, simply run:

```bash
docker-compose up -d
```

This will start all necessary services, including:

- The workflow application (exposed on HTTP port 3501)
- Dapr sidecar

## 🧪 Reproducing the Bug

### Step 1: Start a Workflow
Make an HTTP POST request to start a new workflow:

```bash
curl -X POST http://localhost:3501/start-workflow
```

This will trigger the workflow logic. You will receive the workflow instance ID in the response.

### Step 2: Wait for Cancellation

Wait approximately 60 seconds before proceeding. This waiting period simulates the timeout duration where the
CancellationToken is expected to be triggered.

I am currently not sure that is the correct time to wait, but it should be between 60 and 100 seconds at least.

### ⏳ Why wait?

This bug is theorized to manifest after a certain delay, when the CancellationToken expires or the workflow enters a
cancelled state.

### Step 3: Raise a Signal

Send a signal to the workflow using the captured workflowInstanceId:

```bash
curl -X POST http://localhost:3501/raise-signal/<workflowInstanceId>
```

Replace <workflowInstanceId> with the actual ID you obtained in Step 1.

## 🐞 Expected vs Actual

Expected: The workflow should handle the signal gracefully.

Actual: The behavior might be incorrect due to a bug in how CancellationToken is handled and lead to a GrpcException.

Please check the application logs and workflow state to investigate the bug.