using Common.Services;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MG.KSI.Client.Models
{
	public class KsiClient : IDisposable
	{
		private readonly Encoding _encoding = Encoding.ASCII;
		private readonly IPrintService _printer;
		private readonly TcpClient _client;
		private readonly NetworkStream _stream;

		public KsiClient(IKsiSettingsService ksiSettingsService, IPrintService printer)
		{
			var settings = ksiSettingsService?.GetSettings() ?? throw new ArgumentNullException(nameof(ksiSettingsService));
			_printer = printer ?? throw new ArgumentNullException(nameof(printer));

			_client = new TcpClient(settings.Host, settings.Port);
			_stream = _client.GetStream();
			
			ReadAsync(_stream).Wait();
		}

		#region Private Methods

		private async Task<string> ReadAsync(NetworkStream stream)
		{
			var data = new byte[256]; // buffer to store the response bytes
			var sb = new StringBuilder();
			int bytes = 0;
			do
			{
				bytes = await stream.ReadAsync(data, 0, data.Length);
				sb.Append(_encoding.GetString(data, 0, bytes));
			}
			while (stream.DataAvailable);
			
			return sb.ToString();
		}

		#endregion

		public async Task<string> SendCommandAsync(string command)
		{
			string response = string.Empty;

			try
			{
				var data = _encoding.GetBytes(command);
				await _stream.WriteAsync(data, 0, data.Length);

				// problem: we can intercept the event, not the response from the command
				response = await ReadAsync(_stream);
			}
			catch (ArgumentNullException e)
			{
				_printer.PrintError($"ArgumentNullException: {e}");
			}
			catch (SocketException e)
			{
				_printer.PrintError($"SocketException: {e}");
			}
			catch (Exception e)
			{
				_printer.PrintError($"Exception: {e}");
			}

			return response;
		}

		public void Dispose()
		{
			_stream.Close();
			_client.Close();
		}
	}
}
