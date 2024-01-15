using BenchmarkDotNet.Running;
using TablesBatchExample;

// Run the benchmark
var summary = BenchmarkRunner.Run<TablesBenchmark>();

var tableStorageService = new TableStorageService();

#region Prepare the data

var newEntities = new List<SampleEntity>();
var batchId = Guid.NewGuid();
for (int i = 1; i <= 100; i++)
{
    var entityId = Guid.NewGuid();
    newEntities.Add(new SampleEntity
    {
        BatchId = batchId,
        EntityId = entityId,
        Name = $"Sample Entity {i}",
        RowKey = entityId.ToString(),
        PartitionKey = batchId.ToString()
    });
}

#endregion

await tableStorageService.SubmitVarietyBatchAsync();

var results = await tableStorageService.GetEntitiesAsync("", 300);

foreach (var res in results)
{
    Console.WriteLine($"{res.PartitionKey} | {res.RowKey} | {res.Name}");
}
