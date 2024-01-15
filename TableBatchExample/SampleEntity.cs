using Azure;
using Azure.Data.Tables;

namespace TablesBatchExample;

public class SampleEntity : ITableEntity
{
    public Guid BatchId { get; set; }

    public Guid EntityId { get; set; }

    public string Name { get; set; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}
