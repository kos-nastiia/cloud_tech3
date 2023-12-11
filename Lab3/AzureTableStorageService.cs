using System;
using System.Collections.Generic;
using Azure;
using Azure.Data.Tables;

public class AzureTableStorageService
{
    private readonly TableClient _tableClient;

    public AzureTableStorageService(TableClient tableClient)
    {
        _tableClient = tableClient ?? throw new ArgumentNullException(nameof(tableClient));
    }

    public void CreateTableIfNotExists()
    {
        _tableClient.CreateIfNotExists();
    }

    public void AddEntity(PersonEntity entity)
    {
        _tableClient.AddEntity(entity);
    }

    public void UpdateEntity(PersonEntity entity)
    {
        _tableClient.UpdateEntity(entity, ETag.All, TableUpdateMode.Replace);
    }

    public void DeleteEntity(string partitionKey, string rowKey)
    {
        _tableClient.DeleteEntity(partitionKey, rowKey);
    }

    public List<PersonEntity> GetEntities(string partitionKey)
    {
        var query = $"PartitionKey eq '{partitionKey}'";
        var entities = new List<PersonEntity>();

        foreach (var entity in _tableClient.Query<TableEntity>(filter: query))
        {
            entities.Add(MapToPersonEntity(entity));
        }

        return entities;
    }

    private PersonEntity MapToPersonEntity(TableEntity tableEntity)
    {
        return new PersonEntity
        {
            PartitionKey = tableEntity.GetString("PartitionKey"),
            RowKey = tableEntity.GetString("RowKey"),
            FirstName = tableEntity.GetString("FirstName"),
            LastName = tableEntity.GetString("LastName"),
            Phone = tableEntity.GetString("Phone"),
            PhotoUrl = tableEntity.GetString("PhotoUrl"),
        };
    }
}
