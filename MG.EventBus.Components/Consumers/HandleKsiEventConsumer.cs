using MassTransit;
using MG.EventBus.Contracts;
using System;
using System.Threading.Tasks;

namespace MG.EventBus.Components.Consumers
{
	public class HandleKsiEventConsumer : IConsumer<HandleKsiEvent>
	{
		public async Task Consume(ConsumeContext<HandleKsiEvent> context)
		{
			await Console.Out.WriteLineAsync($"Received event bus msg: ID={context.Message.KsiTcpClientId}, Event='{context.Message.Event}', {context.Message.CreatedDate}");
		}
	}
}
