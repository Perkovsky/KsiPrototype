using Common.Models;
using Common.Services.Impl;
using MassTransit;
using MG.EventBus.Components.Services;
using MG.EventBus.Components.Services.Impl;
using MG.EventBus.Contracts;
using MG.EventBus.Startup;
using MG.KSI.DAO.Infrastructure;
using MG.KSI.DAO.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Producer
{
	class Program
	{
		static string _serverId = "114";
		static string _ksiTcpClientId;
		static string _host;
		static readonly ServiceProvider _serviceProvider;
		static readonly IEnumerable<KsiSettings> _ksiSettings;

		static Program()
		{
			try
			{
				var settingsService = new KsiSettingsService();

				_ksiSettings = settingsService.GetKsiSettings();
				_serviceProvider = new ServiceCollection()
					.RegisterEventBusProducerDependencies(settingsService.GetEventBusSettings())
					.BuildServiceProvider();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Register EventBusProducerService error.");
				Console.WriteLine(ex);
			}
		}

		static void MyHandler(object sender, UnhandledExceptionEventArgs args)
		{
			Exception e = (Exception)args.ExceptionObject;
			Console.WriteLine("MyHandler caught : " + e.Message);
			Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
		}

		static void Main(string[] args)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

			IBusControl bus = null;
			IEventBusProducerService producer = null;
			try
			{
				bus = _serviceProvider.GetService<IBusControl>();
				producer = _serviceProvider.GetService<IEventBusProducerService>();
				
				//NOTE:
				//	MassTransit uses a temporary non-durable queue and has a consumer to handle responses. This temporary queue only get
				//	configured and created when you start the bus. If you forget to start the bus in your application code, the request
				//	client will fail with a timeout, waiting for a response.
				bus.StartAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Resolve EventBusProducerService error.");
				Console.WriteLine(ex);
			}

			Console.WriteLine("-- PRODUCER --");
			
			Console.Write("Do you want to use fake server (y/n)? ");
			string answer = Console.ReadLine();
			if (answer.Equals("y", StringComparison.InvariantCultureIgnoreCase))
			{
				_ksiTcpClientId = "xxx";
				_host = _ksiSettings.Last().Host;
			}
			else
			{
				_ksiTcpClientId = "0050c23d8000";
				_host = _ksiSettings.First().Host;
			}

			Console.WriteLine("Enter KeyBox commands : 1-PanelPing; 2-LightKey; 3-Display; 4-OpenDoor; 8-UploadPanel; 9-UploadPanel (all);");
			Console.WriteLine("Enter service commands: 5-Add; 6-Remove; 7-HeathCheck (Req/Res)");
			Console.WriteLine("or type 'quit' to exit..." + Environment.NewLine);

			while (true)
			{
				Console.Write("> ");
				string msg = Console.ReadLine();
				if (msg.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
				{
					//NOTE:
					//	In order to a temporary non-durable queue to be automatically deleted you must stop the bus.
					bus.StopAsync();
					break;
				}

				switch (msg)
				{
					case "1":
					case "2":
					case "3":
					case "4":
					case "8":
					case "9":
						KsiCommandHandler(producer, msg);
						break;
					case "5":
					case "6":
						ServiceCommandHandler(producer, msg);
						break;
					case "7":
						HeathCheckCommandHandler(producer);
						break;
					default:
						break;
				}
			}
		}

		static void KsiCommandHandler(IEventBusProducerService producer, string msg)
		{
			string ksiCommand = msg switch
			{
				"1" => KsiCommand.PanelPing(),
				"2" => KsiCommand.LightKey(1),
				"3" => KsiCommand.Display("Hi Vitaly"),
				"4" => KsiCommand.OpenDoor(1),
				"8" => KsiCommand.UploadPanel(),
				"9" => KsiCommand.UploadPanel(KeyBoxUploadControlType.All),
				_ => string.Empty,
			};

			if (string.IsNullOrWhiteSpace(ksiCommand))
			{
				Console.WriteLine($"{msg} - unkwown KeyBox command.");
				return;
			}

			Console.WriteLine($"\tSent KeyBox command: {ksiCommand}");
			producer.Send<KeyboxCommandKeybox>(new
			{
				KeyBoxTcpClientId = _ksiTcpClientId,
				CreatedDate = DateTime.UtcNow,
				KeyBoxCommand = ksiCommand
			}, _ksiTcpClientId);
		}

		static void ServiceCommandHandler(IEventBusProducerService producer, string msg)
		{
			ServiceCommandType? serviceCommand = msg switch
			{
				"5" => ServiceCommandType.Add,
				"6" => ServiceCommandType.Remove,
				_ => default,
			};

			if (!serviceCommand.HasValue)
			{
				Console.WriteLine($"{msg} - unkwown service command.");
				return;
			}

			Console.WriteLine($"\tSent service command: {serviceCommand?.ToString()}");
			producer.Send<KeyboxCommandService>(new
			{
				KeyBoxTcpClientId = _ksiTcpClientId,
				IPAddress = _host,
				Command = serviceCommand
			}, _serverId);
		}

		static void HeathCheckCommandHandler(IEventBusProducerService producer)
		{
			try
			{
				Console.WriteLine($"\tSent service HealthCheck command (Req)...");
				var res = producer.SendRequest<KeyboxCommandService, HealthCheckServiceResponse>(new
				{
					KeyBoxTcpClientId = _ksiTcpClientId,
					IPAddress = _host,
					Command = ServiceCommandType.HealthCheck
				}, _serverId);
				Console.WriteLine($"\tSent service HealthCheck command (Res): ID: {res.ServerId}; LA: {res.LastActivity}.");
			}
			catch (AggregateException ex)
			{
				for (int i = 0; i < ex.InnerExceptions.Count; i++)
				{
					Console.WriteLine($"InnerExceptions #{i + 1}:");
					Console.WriteLine(ex.InnerExceptions[i]);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
