using MassTransit;
using MG.EventBus.Components.Services;
using MG.EventBus.Startup.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MG.EventBus.Startup
{
	public class EventBusProducerFactory
	{
		public static IEventBusProducerService Create(EventBusSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			var serviceProvider = new ServiceCollection()
				.RegisterEventBusProducerDependencies(settings)
				.BuildServiceProvider();

			var producer = serviceProvider.GetService<IEventBusProducerService>();
			if (producer == null)
				throw new ArgumentNullException(nameof(producer));

			return producer;
		}
	}
}
