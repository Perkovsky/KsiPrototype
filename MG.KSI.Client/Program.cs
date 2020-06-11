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
				using (var client = new KsiTcpClient(settingsService, printService))
				{
					client.Message += (s, a) => Console.WriteLine($"Client-Prorgam.cs: {a.Message}");
					await client.RunAsync();
				}
			}
			catch (Exception e)
			{
				printService.PrintError($"Exception: {e}");
			}

			//Console.WriteLine("Press any key to exit...");
			//Console.ReadKey();
		}
	}
}
