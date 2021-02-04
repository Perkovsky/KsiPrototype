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
			KsiSettings ksiSetting = null;
			if (answer.Equals("y", StringComparison.InvariantCultureIgnoreCase))
				ksiSetting = _ksiSettings.Last();
			else
				ksiSetting = _ksiSettings.First();
			
			_ksiTcpClientId = ksiSetting.DevicetId;
			_host = ksiSetting.Host;

			Console.WriteLine("Enter KeyBox commands : 1-PanelPing; 2-LightKey(1); 3-Display; 4-OpenDoor; 8-UploadPanel; 9-UploadPanel (all);");
			Console.WriteLine("\t10-Panel('get', 'panelname'); 11-Panel('get', 'panelid'); 12-Panel('get', 'num_keys'); 13-Panel('get', 'num_doors');");
			Console.WriteLine("\t14-Panel('get', 'num_users'); 15-Panel('get', 'box_function'); 16-KeyAudit All;");
			Console.WriteLine("\t17-LightKey(2); 18-LightKey(3); 19-LightKey(7); 20-LightKey(9);");
			Console.WriteLine("\t21-KeyAudit(1);22-KeyAudit(2);23-KeyAudit(7);24-KeyAudit(9);25-KeyAudit(32);");
			Console.WriteLine("\t30-UploadEvent();31-UploadEvent(3196);32-UploadEvent(3196, 3198);");
			Console.WriteLine("\t33-Door(timer=60);34-Key(timezone);35-Key(tzlist=override);");
			Console.WriteLine("\t36-Panel(doto=60);37-Panel(dsto=60);38-Panel(asto=60);");
			Console.WriteLine();
			Console.WriteLine("Enter service commands: 5-Add; 6-Remove; 7-HeathCheck (Req/Res)");
			Console.WriteLine();
			Console.WriteLine("Enter FAKE commands: 26-Remove key 14; 27-Return key 14; 28-Remove key 15; 29-Return key 15");
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
					case "10":
					case "11":
					case "12":
					case "13":
					case "14":
					case "15":
					case "16":
					case "17":
					case "18":
					case "19":
					case "20":
					case "21":
					case "22":
					case "23":
					case "24":
					case "25":
					case "26":
					case "27":
					case "28":
					case "29":
					case "30":
					case "31":
					case "32":
					case "33":
					case "34":
					case "35":
					case "36":
					case "37":
					case "38":
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
				"10" => KsiCommand.Panel("get", "panelname"),
				"11" => KsiCommand.Panel("get", "panelid"),
				"12" => KsiCommand.Panel("get", "num_keys"),
				"13" => KsiCommand.Panel("get", "num_doors"),
				"14" => KsiCommand.Panel("get", "num_users"),
				"15" => KsiCommand.Panel("get", "box_function"),
				"16" => KsiCommand.KeyAudit(type: KeyBoxKeyAuditType.All),
				"17" => KsiCommand.LightKey(2),
				"18" => KsiCommand.LightKey(3),
				"19" => KsiCommand.LightKey(7),
				"20" => KsiCommand.LightKey(9),
				"21" => KsiCommand.KeyAudit(pos: 1),
				"22" => KsiCommand.KeyAudit(pos: 2),
				"23" => KsiCommand.KeyAudit(pos: 7),
				"24" => KsiCommand.KeyAudit(pos: 9),
				"25" => KsiCommand.KeyAudit(pos: 32),
				
				"30" => KsiCommand.UploadEvent(),
				"31" => KsiCommand.UploadEvent(3196),
				"32" => KsiCommand.UploadEvent(3196, 3198),

				// FAKE COMMANDS for testing events
				"26" => "<removekey pos=14></removekey>",
				"27" => "<returnkey pos=14></returnkey>",
				"28" => "<removekey pos=15></removekey>",
				"29" => "<returnkey pos=15></returnkey>",

				// TESTING
				"33" => "<door doorlist=1 timer=60></door>",
				"34" => "<key keylist=1-40 tzlist=s1-00-00-e1-23-59-s2-00-00-e2-23-59-s3-00-00-e3-23-59-s4-00-00-e4-23-59-s5-00-00-e5-23-59-s6-00-00-e6-23-59-s7-00-00-e7-23-59></key>",
				"35" => "<key keylist=1-40 tzlist=override></key>", //timeout=1440
				"36" => "<panel type=set name=doto>60</panel>",
				"37" => "<panel type=set name=dsto>60</panel>",
				"38" => "<panel type=set name=asto>60</panel>",

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
