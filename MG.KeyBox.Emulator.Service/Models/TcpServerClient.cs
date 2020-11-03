using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Unclassified.Net;

namespace MG.KeyBox.Emulator.Service.Models
{
	public class TcpServerClient : AsyncTcpClient
	{
		private const string EMULATOR = "emulator=on";

		public TcpServerClient()
		{
			Message += (s, a) => Console.WriteLine($"Message: {a.Message}.");
		}

		#region Private Methods

		private string GetEvent(KeyBoxParser keyBoxParser)
		{
			var date = DateTime.UtcNow.ToString("yyyy/MM/dd", CultureInfo.CreateSpecificCulture("en-US"));
			var time = DateTime.UtcNow.ToString("HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));

			return keyBoxParser.Tag switch
			{
				"opendoor" => $@"<doorevent panelid=b827eb82438d eventid=2413 date={date} time={time} logtype=42 usession=161 userid=admin door={keyBoxParser.Attributes["door"]} {EMULATOR}>unlock</doorevent>",
				_ => null,
			};
		}

		private string GetReply(KeyBoxParser keyBoxParser)
		{
			return keyBoxParser.Tag switch
			{
				"opendoor" => $@"<cmdreply panelid=b827eb82438d status=ack {EMULATOR}>opendoor</cmdreply>",
				"lightkey" => $@"<cmdreply panelid=b827eb82438d status=ack info=yes pos={keyBoxParser.Attributes["pos"]} {EMULATOR}>lightkey</cmdreply>",
				_ => "unknown command",
			};
		}

		private async Task SendMessageAsync(KeyBoxParser keyBoxParser, bool isCommand)
		{
			var msg = isCommand ? GetReply(keyBoxParser) : GetEvent(keyBoxParser);
			if (string.IsNullOrWhiteSpace(msg))
				return;
			
			var actionType = isCommand ? "Reply" : "Event";
			Console.WriteLine($"{actionType}: {msg}");
			byte[] bytes = Encoding.UTF8.GetBytes(msg);
			await Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
		}

		#endregion

		protected override async Task OnConnectedAsync(bool isReconnected)
		{
			//Log.Information("New client has been connected.");
			Console.WriteLine("New client has been connected.");
			await Task.CompletedTask;
		}

		protected override async Task OnReceivedAsync(int count)
		{
			byte[] bytes = ByteBuffer.Dequeue(count);
			string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			
			Console.WriteLine($"Received command: {message}");
			var keyBoxParser = new KeyBoxParser(message);

			await SendMessageAsync(keyBoxParser, isCommand: false);
			await SendMessageAsync(keyBoxParser, isCommand: true);
		}
	}
}
