using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderCloud.AzureStorage;
using OrderCloud.DocRender.common;


namespace OrderCloud.DocRender.QueueProcess
{
	class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = new HostBuilder()
					
					.UseEnvironment("Development")
					.ConfigureWebJobs(b =>
					{
						
						b.AddAzureStorageCoreServices()
							// This is for QueueTrigger support
							.AddAzureStorage(options => 
							{
									
								options.BatchSize = 1;
								options.MaxPollingInterval = TimeSpan.FromSeconds(10);
								options.VisibilityTimeout = TimeSpan.FromSeconds(30);
							});
						// This is for TimerTrigger support
						b.AddTimers();
					})
					//.ConfigureAppConfiguration(b =>
					//{
						
					//	//b.SetBasePath(Directory.GetCurrentDirectory())
					//		//.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					//		//.AddEnvironmentVariables();
					//})
					.ConfigureLogging((context, b) =>
					{
						b.SetMinimumLevel(LogLevel.Debug);
					})
					.UseConsoleLifetime()
				.ConfigureServices((context, services) =>
			{
				services.AddTransient<AppSettings>(AppSettingsImplementationFactory);
				services.AddTransient<BlobService>(BlobImplementationFactory);
				services.AddTransient<FileSyncFunction>();
				
				//services.AddTransient<SomeQueueTriggerFunctions, SomeQueueTriggerFunctions>();
				// All your Di-able classes can be set up here.
			});

			var host = builder.Build();
			using (host)
			{
				await host.RunAsync();
			}
		}

		private static BlobService BlobImplementationFactory(IServiceProvider arg)
		{
			var settings = arg.GetService<AppSettings>();
			return new BlobService(settings.StorageConnection);
		}

		private static AppSettings AppSettingsImplementationFactory(IServiceProvider arg)
		{
			var cb = new ConfigurationBuilder();
			cb.AddEnvironmentVariables();
			var build = cb.Build();
			return build.Get<AppSettings>();
			
		}
	}
}
