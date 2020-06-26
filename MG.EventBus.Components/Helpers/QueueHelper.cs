using MassTransit;
using MassTransit.Definition;
using MG.EventBus.Components.Models;
using System;

namespace MG.EventBus.Components.Helpers
{
	public static class QueueHelper
	{
		public static string GetQueueNameSuffix(QueuePriority priority = QueuePriority.Normal)
		{
			return (priority == QueuePriority.Normal) ? "" : $"-{priority.ToString().ToLower()}";
		}

		public static string GetQueueName<T>(QueuePriority priority = QueuePriority.Normal)
			where T : class, IConsumer
		{
			return KebabCaseEndpointNameFormatter.Instance.Consumer<T>() + GetQueueNameSuffix(priority);
		}

		public static string GetQueueName<T>(string queueSuffix = null)
			where T : class
		{
			var queueName = KebabCaseEndpointNameFormatter.Instance.SanitizeName(typeof(T).Name);
			return string.IsNullOrWhiteSpace(queueSuffix) ? queueName : $"{queueName}-{queueSuffix}";
		}

		public static Uri GetQueueUri<T>(QueuePriority priority = QueuePriority.Normal)
			where T : class, IConsumer
		{
			string queueName = GetQueueName<T>(priority);
			return new Uri($"queue:{queueName}");
		}

		public static Uri GetQueueUri<T>(string queueSuffix = null)
			where T : class
		{
			string queueName = GetQueueName<T>(queueSuffix);
			return new Uri($"queue:{queueName}");
		}
	}
}
