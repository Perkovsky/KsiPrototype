using Common.Services.Impl;
using MG.KSI.Client.Models;
using System.Threading.Tasks;

namespace MG.KSI.Client
{
	class Program
	{
		static async Task Main(string[] args)
		{
			
			var settingsService = new KsiSettingsService();
			var eventBusSettings = settingsService.GetEventBusSettings();
			var ksiSettings = settingsService.GetKsiSettings();

			await new KsiTcpClientPool(ksiSettings, eventBusSettings).StartAsync();

			//printService.PrintInfo("Press any key to exit...");
			//Console.ReadKey();
		}
	}
}
