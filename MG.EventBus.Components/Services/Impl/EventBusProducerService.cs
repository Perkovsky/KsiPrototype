using MassTransit;
using MG.EventBus.Components.Helpers;
using MG.EventBus.Components.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MG.EventBus.Components.Services.Impl
{
	public class EventBusProducerService : IEventBusProducerService
	{
		private readonly IBusControl _bus;
		private readonly IPublishEndpoint _publishEndpoint;
		private readonly ISendEndpointProvider _sendEndpointProvider;

		public EventBusProducerService(IBusControl bus, IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
		{
			_bus = bus ?? throw new ArgumentNullException(nameof(bus));
			_publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
			_sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
		}

		public Task Publish<T>(object values)
			where T : class
		{
			return PublishAsync<T>(values);
		}

		public async Task PublishAsync<T>(object values, CancellationToken cancellationToken = default)
			where T : class
		{
			await _publishEndpoint.Publish<T>(values, cancellationToken);
		}

		public Task Send<TContract, TConsumer>(object values, QueuePriority priority = QueuePriority.Normal)
			where TContract : class
			where TConsumer : class, IConsumer<TContract>
		{
			return SendAsync<TContract, TConsumer>(values, priority);
		}

		public async Task SendAsync<TContract, TConsumer>(object values, QueuePriority priority = QueuePriority.Normal, CancellationToken cancellationToken = default)
			where TContract : class
			where TConsumer : class, IConsumer<TContract>
		{
			var endpoint = await _sendEndpointProvider.GetSendEndpoint(QueueHelper.GetQueueUri<TConsumer>(priority));
			await endpoint.Send<TContract>(values, cancellationToken);
		}

		public Task Send<TContract>(object values, string queueSuffix = null)
			where TContract : class
		{
			return SendAsync<TContract>(values, queueSuffix);
		}

		public async Task SendAsync<TContract>(object values, string queueSuffix = null, CancellationToken cancellationToken = default)
			where TContract : class
		{
			var endpoint = await _sendEndpointProvider.GetSendEndpoint(QueueHelper.GetQueueUri<TContract>(queueSuffix));
			await endpoint.Send<TContract>(values, cancellationToken);
		}

		public TResponse SendRequest<TRequest, TResponse>(object values, string queueSuffix = null)
			where TRequest : class
			where TResponse : class
		{
			var task = Task.Run<TResponse>(async () => await SendRequestAsync<TRequest, TResponse>(values, queueSuffix));
			return task.Result;
		}

		public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(object values, string queueSuffix = null, CancellationToken cancellationToken = default)
			where TRequest : class
			where TResponse : class
		{
			var client = _bus.CreateRequestClient<TRequest>(QueueHelper.GetQueueUri<TRequest>(queueSuffix));
			var response = await client.GetResponse<TResponse>(values, cancellationToken);
			return response.Message;
		}
	}
}
