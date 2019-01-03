using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using OrderCloud.AzureStorage;

namespace OrderCloud.DocRender
{
	public static class Container
	{
		private static readonly IServiceProvider _serviceProvider;

		static Container()
		{
			var builder = new ConfigurationBuilder();
			builder.AddEnvironmentVariables();
			var build = builder.Build();
			var appSettings = build.Get<AppSettings>();
			
			var s = new ServiceCollection();
			s.AddTransient<JobService>();
			s.AddTransient(x => new TableService(appSettings.StorageConnection));
			s.AddTransient(x=> new BlobService(appSettings.StorageConnection));
			s.AddTransient<OcAuthorizationService>();
			s.AddTransient<QueueService>(x => new QueueService(appSettings.StorageConnection));
			s.AddSingleton<AppSettings>(appSettings);
			_serviceProvider = s.BuildServiceProvider();
		}

		public static T Get<T>()
		{
			var o = _serviceProvider.GetService<T>();
			if(o == null)
				throw new Exception("type not registered in ioc " + typeof(T).FullName);
			return o;
		}
	}
}
