using Common.Models;
using Common.Services.Impl;
using MG.EventBus.Components.Services;
using MG.EventBus.Contracts;
using MG.EventBus.Startup;
using MG.KSI.DAO.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Producer
{
	class Program
	{
		static string _ksiTcpClientId;
		static readonly ServiceProvider _serviceProvider;
		static readonly IEnumerable<KsiSettings> _ksiSettings;

		static Program()
		{
			var settingsService = new KsiSettingsService();

			_ksiSettings = settingsService.GetKsiSettings();
			_serviceProvider = new ServiceCollection()
				.RegisterEventBusProducerDependencies(settingsService.GetEventBusSettings())
				.BuildServiceProvider();
		}

		static void Main(string[] args)
		{
			var producer = _serviceProvider.GetService<IEventBusProducerService>();

			Console.WriteLine("-- PRODUCER --");
			
			Console.Write("Do you want to use fake server (y/n)? ");
			string answer = Console.ReadLine();
			if (answer.Equals("y", StringComparison.InvariantCultureIgnoreCase))
				_ksiTcpClientId = "xxx"; // _ksiSettings.Last().Host;
			else
				_ksiTcpClientId = "0050c23d8000"; //_ksiSettings.First().Host;

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
				producer.Send<KeyboxCommandKeybox>(new
				{
					KeyBoxTcpClientId = _ksiTcpClientId,
					CreatedDate = DateTime.UtcNow,
					KeyBoxCommand = ksiCommand
				}, _ksiTcpClientId);
			}
		}
	}
}
