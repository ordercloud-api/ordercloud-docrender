using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

// Namespace for CloudConfigurationManager

namespace OrderCloud.AzureStorage
{
	public class TableService
	{
		private readonly string _connectionString;
		private readonly string _env;
		private readonly CloudStorageAccount _storageAccount;
		private readonly CloudTableClient _tableClient;
		public TableService(string storageConnectionString)
		{
			_connectionString = storageConnectionString;

			_storageAccount = CloudStorageAccount.Parse(storageConnectionString);
			_tableClient = _storageAccount.CreateCloudTableClient();
		}

		//public async Task DestoryTablesForDevOnlyAsync(string onlyThisTableByName = null)
		//{
		//	if (_connectionString != "UseDevelopmentStorage=true;")
		//		throw new Exception("this will delete all your datas. only for dev/test purposes. Connection string must = \"UseDevelopmentStorage=true;\"");


		//	var tables = await _tableClient.ListTablesSegmentedAsync()
		//	foreach (var table in _tableClient.ListTables().Where(x => x.Name == onlyThisTableByName || onlyThisTableByName == null))
		//		await table.DeleteAsync();
		//}
		public async Task InsertOrReplaceAsync(ITableEntity e, string overrideTableName = null)
		{
			var table = _tableClient.GetTableReference(overrideTableName ?? e.GetType().Name);
			await table.CreateIfNotExistsAsync();
			var insertOperation = TableOperation.InsertOrReplace(e);
			await table.ExecuteAsync(insertOperation);
		}

		public async Task DeleteAsync<T>(string partitionkey, string rowkey, string overrideTableName = null) where T : ITableEntity
		{
			var table = _tableClient.GetTableReference(overrideTableName ?? typeof(T).Name);
			var get = await GetAsync<T>(partitionkey, rowkey);
			var deleteOperation = TableOperation.Delete(get);
			await table.ExecuteAsync(deleteOperation);
		}

		//public async Task DeleteAllByPartitionKeyAsync<T>(string key, string overrideTableName = null) where T : ITableEntity, new()
		//{
		//	var table = _tableClient.GetTableReference(overrideTableName ?? typeof(T).Name);
		//	var l = ListAllbyPartitionKey<T>(key);
		//	var tasks = new List<Task>();
		//	l.ForEach(item => tasks.Add(table.ExecuteAsync(TableOperation.Delete(item))));
		//	await Task.WhenAll(tasks.ToArray());
		//}

		//public async Task<IEnumerable<T>> ListAllAsync<T>(string overrideTableName = null) where T : ITableEntity, new()
		//{
		//	var table = _tableClient.GetTableReference(overrideTableName ?? typeof(T).Name);
		//	var exQuery = new TableQuery<T>();
		//	//return table.ExecuteQuery<T>(exQuery);
		//	var r = await table.ExecuteAsync(exQuery);

		//}

		//public List<T> ListAllbyPartitionKey<T>(string key, string overrideTableName = null) where T : ITableEntity, new()
		//{
		//	var table = _tableClient.GetTableReference(overrideTableName ?? typeof(T).Name);
		//	var exQuery = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key));
		//	//var qr = await table.ExecuteQuerySegmentedAsync<T>(exQuery, token);
		//	var tr = table.ExecuteQuery<T>(exQuery);
		//	return tr.ToList();
		//}

		public async Task<T> GetAsync<T>(string partitionkey, string rowkey, string overrideTableName = null) where T : ITableEntity
		{
			var table = _tableClient.GetTableReference(overrideTableName ?? typeof(T).Name);
			var retrieveOperation = TableOperation.Retrieve<T>(partitionkey, rowkey);
			var tableResult = await table.ExecuteAsync(retrieveOperation);

			if (tableResult.Result != null)
				return (T)tableResult.Result;
			else
				throw new Exceptions.NotFoundException();
		}
	}

}
