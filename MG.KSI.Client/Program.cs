using Common.Services.Impl;
using MG.KSI.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MG.KSI.Client
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var printService = new PrintService();
			var settingsService = new KsiSettingsService();
			var eventBusSetting = settingsService.GetEventBusSetting();
			var ksiSettings = settingsService.GetKsiSettings();

			//see: https://stackoverflow.com/questions/19102966/parallel-foreach-vs-task-run-and-task-whenall

			#region Single

			//try
			//{
			//	using (var client = new KsiTcpClient(ksiSettings.First(), eventBusSetting))
			//	{
			//		client.Message += (s, a) => printService.PrintInfo($"KsiTcpClient ({ksiSettings.First().Host}) handle from Endpoint: {a.Message}");
			//		await client.RunAsync();
			//	}
			//}
			//catch (Exception e)
			//{
			//	printService.PrintError($"Exception: {e}");
			//}

			#endregion

			#region Parallel.ForEach

			//Parallel.ForEach(ksiSettings, async ksiSetting =>
			//{
			//	try
			//	{
			//		using (var client = new KsiTcpClient(ksiSetting, eventBusSetting))
			//		{
			//			client.Message += (s, a) => printService.PrintInfo($"KsiTcpClient ({ksiSetting.Host}) handle from Endpoint: {a.Message}");
			//			await client.RunAsync();
			//		}
			//	}
			//	catch (Exception e)
			//	{
			//		printService.PrintError($"Exception: {e}");
			//	}
			//});

			#endregion

			#region foreach -> Task.Run

			//foreach (var ksiSetting in ksiSettings)
			//{
			//	await Task.Run(async () =>
			//	{
			//		try
			//		{
			//			using (var client = new KsiTcpClient(ksiSetting, eventBusSetting))
			//			{
			//				client.Message += (s, a) => printService.PrintInfo($"KsiTcpClient ({ksiSetting.Host}) handle from Endpoint: {a.Message}");
			//				await client.RunAsync();
			//			}
			//		}
			//		catch (Exception e)
			//		{
			//			printService.PrintError($"Exception: {e}");
			//		}
			//	});
			//}

			#endregion

			#region Task.WhenAll

			var tasks = new List<Task>();
			foreach (var ksiSetting in ksiSettings)
			{
				tasks.Add(Task.Run(async () =>
				{
					try
					{
						using (var client = new KsiTcpClient(ksiSetting, eventBusSetting))
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
			await Task.WhenAll(tasks);

			#endregion

			#region Task.Run -> Parallel.ForEach

			//await Task.Run(() => Parallel.ForEach(ksiSettings, async ksiSetting =>
			//{
			//	try
			//	{
			//		using (var client = new KsiTcpClient(ksiSetting, eventBusSetting))
			//		{
			//			client.Message += (s, a) => printService.PrintInfo($"KsiTcpClient ({ksiSetting.Host}) handle from Endpoint: {a.Message}");
			//			await client.RunAsync();
			//		}
			//	}
			//	catch (Exception e)
			//	{
			//		printService.PrintError($"Exception: {e}");
			//	}
			//}));

			#endregion

			//printService.PrintInfo("Press any key to exit...");
			//Console.ReadKey();
		}
	}
}
