using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage.Table;

namespace OrderCloud.AzureStorage
{

	public class AzureTableConfigationSource : IConfigurationSource
	{
		public AzureTableConfigationSource()
		{
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new AzureTableConfigurationProvider();
		}
	}

	public class AzureTableConfigurationProvider : ConfigurationProvider
	{
		public AzureTableConfigurationProvider()
		{
		}

		public override void Load()
		{
			LoadAsync().Wait();
		}

		private async Task LoadAsync()
		{
			var conn = AzureTableConfigurationProviderExtensions.GetConn();
			var table = new TableService(conn.StorageConn);
			var source = await table.GetAsync<DynamicObjectTableEntity>(conn.Env, "settings", $"appsettings{conn.AppName}");
			source.properties.ToList().ForEach(x => {
				Data[x.Key.Replace("11", ":")] = x.Value.PropertyAsObject.ToString();
			});
		}
	}

	public static class AzureTableConfigurationProviderExtensions
	{
		internal static (string StorageConn, string Env, string AppName) GetConn()
		{
			string getEnvVar(string name, bool required)
			{
				var v = Environment.GetEnvironmentVariable(name);
				if (string.IsNullOrEmpty(v) && required)
					throw new Exceptions.MissingEnvironmentVar(name);
				return v;
			}
			var appName = getEnvVar("DEVOPS_APPNAME", false);
			var forceAppSettings = !string.IsNullOrEmpty(appName);
			return (getEnvVar("DEVOPS_STORAGE", forceAppSettings), getEnvVar("ASPNETCORE_ENVIRONMENT", forceAppSettings), appName);
		}

		public static IConfigurationBuilder AddAzureTableConfiguration(this IConfigurationBuilder builder)
		{
			var conn = GetConn();
			if (!string.IsNullOrEmpty(conn.StorageConn))
				return builder.Add(new AzureTableConfigationSource());
			else
				return builder;
		}
	}
}
