using Common.Models;
using MG.EventBus.Startup.Models;
using Microsoft.Extensions.Configuration;

namespace Common.Services.Impl
{
	public class KsiSettingsService : IKsiSettingsService
	{
		public KsiSettings GetKsiSettings()
		{
			// used safe storage of app secrets
			// see: https://docs.microsoft.com/en-US/aspnet/core/security/app-secrets

			var config = new ConfigurationBuilder()
				.AddUserSecrets<KsiSettings>()
				.Build();
			var result = config.GetSection("KSI").Get<KsiSettings>();

			return result;
		}

		public EventBusSettings GetEventBusSetting()
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
