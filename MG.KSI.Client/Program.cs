using Common.Services.Impl;
using MG.KSI.Client.Models;
using System;
using System.Threading.Tasks;

namespace MG.KSI.Client
{
	class Program
	{
		static async Task SendCommandAsync(KsiClient client, string command)
		{
			Console.WriteLine($"Sent command: {command}");
			var response = await client.SendCommandAsync(command);
			Console.WriteLine($"Response: {(!string.IsNullOrWhiteSpace(response) ? response : "<none>")}");
		}

		static async Task Main(string[] args)
		{
			var printService = new PrintService();
			var settingsService = new KsiSettingsService();

			try
			{
				using (var client = new KsiClient(settingsService, printService))
				{
					await SendCommandAsync(client, Commands.PanelPing());
					await SendCommandAsync(client, Commands.LightKey(1));
					await SendCommandAsync(client, Commands.Display("Hi Vitaly"));
					await SendCommandAsync(client, Commands.OpenDoor(1));
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
