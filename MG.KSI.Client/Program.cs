using Common.Services.Impl;
using MG.KSI.Client.Models;
using System;
using System.Threading.Tasks;

namespace MG.KSI.Client
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var printService = new PrintService();
			var settingsService = new KsiSettingsService();

			try
			{
				using (var client = new KsiTcpClient(settingsService))
				{
					client.Message += (s, a) => printService.PrintInfo($"KsiTcpClient ({settingsService.GetKsiSettings().Host}) handle from Endpoint: {a.Message}");
					await client.RunAsync();
				}
			}
			catch (Exception e)
			{
				printService.PrintError($"Exception: {e}");
			}

			//printService.PrintInfo("Press any key to exit...");
			//Console.ReadKey();
		}
	}
}
