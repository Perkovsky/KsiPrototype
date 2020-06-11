using Common.Services.Impl;
using MG.EventBus.Components.Services;
using MG.EventBus.Contracts;
using MG.EventBus.Startup;
using MG.KSI.DAO.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Producer
{
	class Program
	{
		static readonly string _ksiTcpClientId;
		static readonly ServiceProvider _serviceProvider;

		static Program()
		{
			var settingsService = new KsiSettingsService();

			_ksiTcpClientId = settingsService.GetKsiSettings().Host;
			_serviceProvider = new ServiceCollection()
				.RegisterEventBusProducerDependencies(settingsService.GetEventBusSetting())
				.BuildServiceProvider();
		}

		static void Main(string[] args)
		{
			var producer = _serviceProvider.GetService<IEventBusProducerService>();

			var random = new Random();
			Console.WriteLine("-- PRODUCER --");
			Console.WriteLine("Enter command: 1-PanelPing; 2-LightKey; 3-Display; 4-OpenDoor (or quit to exit)..." + Environment.NewLine);

			while (true)
			{
				Console.Write("> ");
				string msg = Console.ReadLine();
				if (msg.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
					break;

				string ksiCommand = msg switch
				{
					"1" => KsiCommand.PanelPing(),
					"2" => KsiCommand.LightKey(1),
					"3" => KsiCommand.Display("Hi Vitaly"),
					"4" => KsiCommand.OpenDoor(1),
					_ => string.Empty,
				};

				if (string.IsNullOrWhiteSpace(ksiCommand))
				{
					Console.WriteLine($"{msg} - unkwown command.");
					continue;
				}

				Console.WriteLine($"\tSent command: {ksiCommand}");
				producer.Publish<KsiCommandSent>(new
				{
					KsiTcpClientId = _ksiTcpClientId,
					CreatedDate = DateTime.UtcNow,
					KsiCommand = ksiCommand
				});
			}
		}
	}
}
