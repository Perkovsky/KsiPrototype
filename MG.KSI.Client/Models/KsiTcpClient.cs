using Common.Services;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unclassified.Net;

namespace MG.KSI.Client.Models
{
	public class KsiTcpClient : AsyncTcpClient
	{
		private readonly Encoding _encoding = Encoding.ASCII;
		private readonly IPrintService _printer;

		public string KsiTcpClientId { get; private set; }

		public KsiTcpClient(IKsiSettingsService ksiSettingsService, IPrintService printer)
		{
			var settings = ksiSettingsService?.GetSettings() ?? throw new ArgumentNullException(nameof(ksiSettingsService));
			_printer = printer ?? throw new ArgumentNullException(nameof(printer));

			KsiTcpClientId = settings.Host;
			IPAddress = IPAddress.Parse(settings.Host);
			Port = settings.Port;
			AutoReconnect = true;
		}

		private async Task SendAsync(string command)
		{
			Console.WriteLine($"Sent command: {command}");
			await SendCommandAsync(command);
		}

		protected override async Task OnConnectedAsync(bool isReconnected)
		{
			await SendAsync(Commands.PanelPing());
			await SendAsync(Commands.LightKey(1));
			//await SendAsync(Commands.Display("Hi Vitaly"));
			//await SendAsync(Commands.OpenDoor(1));
		}

		protected override Task OnReceivedAsync(int count)
		{
			byte[] bytes = ByteBuffer.Dequeue(count);
			string message = _encoding.GetString(bytes, 0, bytes.Length);
			Console.WriteLine("Client: received: " + message);
			return Task.CompletedTask;
		}

		public async Task SendCommandAsync(string command)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(command);
			await Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
		}
	}
}
