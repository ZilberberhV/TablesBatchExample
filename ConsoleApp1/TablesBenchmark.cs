using BenchmarkDotNet.Attributes;

namespace TablesBatchExample;

[MemoryDiagnoser]
public class TablesBenchmark
{
    private TableStorageService _tableStorageService;
    private List<SampleEntity> _entities;

    [IterationSetup]
    public void Setup()
    {
        _tableStorageService = new TableStorageService();
        _entities = new List<SampleEntity>();

        var batchId = Guid.NewGuid();
        for (int i = 1; i <= 1000; i++)
        {
            var entityId = Guid.NewGuid();
            _entities.Add(new SampleEntity
            {
                BatchId = batchId,
                EntityId = entityId,
                Name = $"Sample Entity {i}",
                RowKey = entityId.ToString(),
                PartitionKey = batchId.ToString()
            });
        }
    }

    [Benchmark]
    public async Task BenchmarkTransactionBatch()
    {
        while (_entities.Any())
        {
            await _tableStorageService.InsertBatchAsync(_entities.Take(100).ToList());
            _entities = _entities.Skip(100).ToList();
        }
        
    }

    [Benchmark]
    public async Task BenchmarkIndividualBatch()
    {
        foreach (var entity in _entities)
        {
            await _tableStorageService.InsertEntityAsync(entity);
        }
    }
}
