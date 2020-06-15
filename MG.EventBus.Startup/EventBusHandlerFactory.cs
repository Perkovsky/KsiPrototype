using MassTransit;
using MG.EventBus.Components.Helpers;
using MG.EventBus.Startup.Models;
using System;

namespace MG.EventBus.Startup
{
	public class EventBusHandlerFactory
	{
		public static IBusControl Create<TMessage>(string uniqueQueueSuffix, EventBusSettings settings, MessageHandler<TMessage> handler)
			where TMessage : class
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			string hostName = settings.CloudAMQP?.HostName;
			string vhost = settings.CloudAMQP?.VirtualHost;
			string port = settings.CloudAMQP?.Port;
			string username = settings.CloudAMQP?.UserName;
			string password = settings.CloudAMQP?.Password;

			return Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				var host = cfg.Host(new Uri($@"rabbitmq://{hostName}:{port}/{vhost}/"), h =>
				{
					h.Username(username);
					h.Password(password);

					//h.UseSsl(s =>
					//{
					//	s.Protocol = SslProtocols.Tls12;
					//});

					string queueName = QueueHelper.GetQueueName<TMessage>(uniqueQueueSuffix);
					cfg.ReceiveEndpoint(queueName, e => e.Handler<TMessage>(handler));
				});
			});
		}
	}
}
