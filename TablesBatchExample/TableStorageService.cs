using Azure.Data.Tables;

namespace TablesBatchExample;

public class TableStorageService
{
    private readonly TableClient _tableClient;

    public TableStorageService() 
    {
        var connString = ConfigurationProvider.Configuration["AzureTablesStorage:ConnectionString"];
        var tableServiceClient = new TableServiceClient(connString);
        _tableClient = tableServiceClient.GetTableClient("BatchTable");
    }

    public Task InsertEntityAsync(SampleEntity entity)
    {
        return _tableClient.AddEntityAsync(entity);
    }

    public Task InsertBatchAsync(List<SampleEntity> entities)
    {
        var batchTransactions = entities
            .Select(e => new TableTransactionAction(TableTransactionActionType.Add, e));

        return _tableClient.SubmitTransactionAsync(batchTransactions);
    }

    public async Task<List<SampleEntity>?> GetEntitiesAsync(string partition, int size)
    {
        var page = await _tableClient.QueryAsync<SampleEntity>(x => x.PartitionKey == partition, size).AsPages().FirstOrDefaultAsync();

        return page?.Values.OrderBy(x => x.Name).ToList();
    }

    public async Task SubmitVarietyBatchAsync()
    {
        var existingBatchId = Guid.Parse(""); // Existing batch id

        var insertEntityId = Guid.NewGuid();
        var insertAction = new TableTransactionAction(TableTransactionActionType.Add, new SampleEntity
        {
            BatchId = existingBatchId,
            EntityId = insertEntityId,
            Name = "Inserted Entity",
            PartitionKey = existingBatchId.ToString(),
            RowKey = insertEntityId.ToString()
        });

        var updateEntityId = Guid.Parse(""); // Existing entity id
        var updateAction = new TableTransactionAction(TableTransactionActionType.UpdateMerge, new SampleEntity
        {
            BatchId = existingBatchId,
            EntityId = updateEntityId, 
            Name = "Updated Entity",
            PartitionKey = existingBatchId.ToString(),
            RowKey = updateEntityId.ToString()
        });

        var deleteEntityId = Guid.Parse(""); // Existing entity id
        var deleteAction = new TableTransactionAction(TableTransactionActionType.Delete, new SampleEntity
        {
            BatchId = existingBatchId,
            EntityId = deleteEntityId,
            PartitionKey = existingBatchId.ToString(),
            RowKey = deleteEntityId.ToString()
        });

        await _tableClient.SubmitTransactionAsync(new[] { insertAction, updateAction, deleteAction });
    }
}
