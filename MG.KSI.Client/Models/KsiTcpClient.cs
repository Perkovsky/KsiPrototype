using Common.Services;
using MassTransit;
using MG.EventBus.Components.Helpers;
using MG.EventBus.Contracts;
using MG.EventBus.Startup;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unclassified.Net;

namespace MG.KSI.Client.Models
{
	public class KsiTcpClient : AsyncTcpClient
	{
		private readonly Encoding _encoding = Encoding.UTF8;
		private readonly string _ksiTcpClientId;
		private readonly IBusControl _bus;
		private readonly ISendEndpoint _sender;

		public KsiTcpClient(IKsiSettingsService ksiSettingsService)
		{
			var settings = ksiSettingsService?.GetKsiSettings() ?? throw new ArgumentNullException(nameof(ksiSettingsService));
			
			_ksiTcpClientId = settings.Host;
			IPAddress = IPAddress.Parse(settings.Host);
			Port = settings.Port;
			AutoReconnect = true;

			_bus = EventBusHandlerFactory.Create<KsiCommandSent>(ksiSettingsService.GetEventBusSetting(), KsiCommandSentHandler);
			_sender = _bus.GetSendEndpoint(QueueHelper.GetQueueUri<HandleKsiEvent>()).Result;
		}

		#region Private Methods

		private async Task KsiCommandSentHandler(ConsumeContext<KsiCommandSent> context)
		{
			if (context.Message.KsiTcpClientId != _ksiTcpClientId)
				return;

			await SendCommandAsync(context.Message.KsiCommand);
		}

		private async Task SendCommandAsync(string command)
		{
			byte[] bytes = _encoding.GetBytes(command);
			await Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
		}

		#endregion

		protected override async Task OnConnectedAsync(bool isReconnected)
		{
			await _bus.StartAsync();
			Console.WriteLine($"KsiTcpClient ({_ksiTcpClientId}) listening command reply or events...{Environment.NewLine}");
		}

		protected override async Task OnReceivedAsync(int count)
		{
			byte[] bytes = ByteBuffer.Dequeue(count);
			string message = _encoding.GetString(bytes, 0, bytes.Length);
			Console.WriteLine($"KsiTcpClient received: {message}");

			
			await _sender.Send<HandleKsiEvent>(new
			{
				KsiTcpClientId = _ksiTcpClientId,
				CreatedDate = DateTime.UtcNow,
				Event = message
			});
		}

		public new void Dispose()
		{
			_bus.Stop();
			base.Dispose();
		}
	}
}
