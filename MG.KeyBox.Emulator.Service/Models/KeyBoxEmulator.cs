using System;
using System.Net;
using System.Threading.Tasks;
using Unclassified.Net;

namespace MG.KeyBox.Emulator.Service.Models
{
	public class KeyBoxEmulator : AsyncTcpClient
	{
		private readonly AsyncTcpListener<TcpServerClient> _server;

		public KeyBoxEmulator(KeyBoxEmulatorSettings settings)
		{
			_server = new AsyncTcpListener<TcpServerClient>
			{
				IPAddress = IPAddress.Parse(settings.IPAddress).MapToIPv6(),
				Port = settings.Port
			};
		}

		public async Task StartAsync()
		{
			//Log.Information("KeyBox Emulator has been started.");
			Console.WriteLine("KeyBox Emulator has been started.");

			await _server.RunAsync();
		}

		public void Stop()
		{
			//Log.Information("KeyBox Emulator has been stopped.");
			Console.WriteLine("KeyBox Emulator has been stopped.");

			_server.Stop(true);
		}
	}
}
