using MassTransit;
using MG.EventBus.Contracts;
using System;
using System.Threading.Tasks;

namespace MG.EventBus.Components.Consumers
{
	public class FaultHandleKsiEventConsumer : IConsumer<Fault<KeyboxEvent>>
	{
		public async Task Consume(ConsumeContext<Fault<KeyboxEvent>> context)
		{
			// error handling here
			//	logging, changing status email into DB, etc.

			await Console.Out.WriteLineAsync($">>> Consuming Fault: ID={context.Message.Message.KeyBoxTcpClientId}, Event='{context.Message.Message.Event}'");
			await Console.Out.WriteLineAsync($">>> Exception: {context.Message.Exceptions[0].Message}");
		}
	}
}
