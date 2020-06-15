using Common.Models;
using MG.EventBus.Startup.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Common.Services.Impl
{
	public class KsiSettingsService : IKsiSettingsService
	{
		public IEnumerable<KsiSettings> GetKsiSettings()
		{
			// used safe storage of app secrets
			// see: https://docs.microsoft.com/en-US/aspnet/core/security/app-secrets

			var config = new ConfigurationBuilder()
				.AddUserSecrets<KsiSettings>()
				.Build();

			var result = new List<KsiSettings>
			{
				config.GetSection("KSI").Get<KsiSettings>(),
				new KsiSettings { Host = "127.0.0.1", Port = 13000 }
			};

			return result;
		}

		public EventBusSettings GetEventBusSettings()
		{
			return new EventBusSettings
			{
				UseInMemory = false, // if true, then the stub method is used: use in memory (FOR DEVELOPERS ONLY)
				CloudAMQP = new CloudAMQPSettings
				{
					UserName = "oklnbiuq",
					Password = "OD0mXJWCHtbXBt8JADrigXSlebXNORMA",
					VirtualHost = "oklnbiuq",
					HostName = "barnacle.rmq.cloudamqp.com",
					Port = "5672"
				},
				AmazonMQ = new AmazonMQSettings
				{
					UserName = "admin",
					Password = "admin",
					HostName = "localhost"
				}
			};
		}
	}
}
