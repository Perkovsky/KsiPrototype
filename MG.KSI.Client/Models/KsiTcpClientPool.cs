using Common.Models;
using Common.Services.Impl;
using MG.EventBus.Startup.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MG.KSI.Client.Models
{
	public class KsiTcpClientPool
	{
		private readonly IEnumerable<KsiSettings> _ksiSettings;
		private readonly EventBusSettings _eventBusSettings;
		private readonly List<Task> _tasks = new List<Task>();

		public KsiTcpClientPool(IEnumerable<KsiSettings> ksiSettings, EventBusSettings eventBusSettings)
		{
			_ksiSettings = ksiSettings ?? throw new ArgumentNullException(nameof(ksiSettings));
			_eventBusSettings = eventBusSettings ?? throw new ArgumentNullException(nameof(eventBusSettings));
		}

		public async Task StartAsync()
		{
			var printService = new PrintService();

			//see: https://stackoverflow.com/questions/19102966/parallel-foreach-vs-task-run-and-task-whenall
			//	   and also previous commit

			foreach (var ksiSetting in _ksiSettings)
			{
				_tasks.Add(Task.Run(async () =>
				{
					try
					{
						using (var client = new KsiTcpClient(ksiSetting, _eventBusSettings))
						{
							client.Message += (s, a) => printService.PrintInfo($"KsiTcpClient ({ksiSetting.Host}) handle from Endpoint: {a.Message}");
							await client.RunAsync();
						}
					}
					catch (Exception e)
					{
						printService.PrintError($"Exception: {e}");
					}
				}));
			}

			await Task.WhenAll(_tasks);
		}

		public void Stop()
		{
			_tasks.ForEach(x => x.Dispose());
		}
	}
}
