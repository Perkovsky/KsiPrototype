using Common.Models;
using Microsoft.Extensions.Configuration;

namespace Common.Services.Impl
{
	public class KsiSettingsService : IKsiSettingsService
	{
		public KsiSettings GetSettings()
		{
			// used safe storage of app secrets
			// see: https://docs.microsoft.com/en-US/aspnet/core/security/app-secrets

			var config = new ConfigurationBuilder()
				.AddUserSecrets<KsiSettings>()
				.Build();
			var result = config.GetSection("KSI").Get<KsiSettings>();

			return result;
		}
	}
}
