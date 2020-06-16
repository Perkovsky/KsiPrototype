using Common.Services.Impl;
using MG.KSI.Client.Models;
using Topshelf;

namespace MG.KSI.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			var settingsService = new KsiSettingsService();
			var eventBusSettings = settingsService.GetEventBusSettings();
			var ksiSettings = settingsService.GetKsiSettings();

			HostFactory.Run(x =>
			{
				x.Service<KsiTcpClientPool>(s =>
				{
					s.ConstructUsing(name => new KsiTcpClientPool(ksiSettings, eventBusSettings));
					s.WhenStarted(async c => await c.StartAsync());
					s.WhenStopped(c => c.Stop());
				});
				x.RunAsLocalSystem();

				x.SetDescription("KSI TCP Client Pool Prototype");
				x.SetDisplayName("KSI TCP Client Pool");
				x.SetServiceName("KSI TCP Client Pool");
			});
		}
	}
}
