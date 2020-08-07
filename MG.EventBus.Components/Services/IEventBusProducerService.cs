using MassTransit;
using MG.EventBus.Components.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MG.EventBus.Components.Services
{
	public interface IEventBusProducerService
	{
		Task Publish<T>(object values)
			where T : class;

		Task PublishAsync<T>(object values, CancellationToken cancellationToken = default)
			where T : class;

		Task Send<TContract, TConsumer>(object values, QueuePriority priority = QueuePriority.Normal)
			where TContract : class
			where TConsumer : class, IConsumer<TContract>;

		Task SendAsync<TContract, TConsumer>(object values, QueuePriority priority = QueuePriority.Normal, CancellationToken cancellationToken = default)
			where TContract : class
			where TConsumer : class, IConsumer<TContract>;

		Task Send<TContract>(object values, string queueSuffix = null)
			where TContract : class;

		Task SendAsync<TContract>(object values, string queueSuffix = null, CancellationToken cancellationToken = default)
			where TContract : class;

		TResponse SendRequest<TRequest, TResponse>(object values, string queueSuffix = null)
			where TRequest : class
			where TResponse : class;

		Task<TResponse> SendRequestAsync<TRequest, TResponse>(object values, string queueSuffix = null, CancellationToken cancellationToken = default)
			where TRequest : class
			where TResponse : class;
	}
}
