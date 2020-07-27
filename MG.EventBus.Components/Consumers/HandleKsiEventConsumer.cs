using MassTransit;
using MG.EventBus.Contracts;
using System;
using System.Threading.Tasks;

namespace MG.EventBus.Components.Consumers
{
	public class HandleKsiEventConsumer : IConsumer<KeyboxEvent>
	{
		public async Task Consume(ConsumeContext<KeyboxEvent> context)
		{
			await Console.Out.WriteLineAsync($"Received event bus msg: ID={context.Message.KeyBoxTcpClientId}, Event='{context.Message.Event}', {context.Message.CreatedDate}");
		}
	}
}
