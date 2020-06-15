using Common.Services.Impl;
using GreenPipes;
using MassTransit;
using MG.EventBus.Startup;
using SimpleInjector;
using Topshelf;

namespace Consumer.WindowsService
{
	class Program
	{
        static readonly Container container;

        static Program()
        {
            var settingsService = new KsiSettingsService();
            container = new Container();
            container.RegisterHandleKsiEventConsumerDependencies(settingsService.GetEventBusSettings());
            container.Verify();
        }

        static void Main(string[] args)
        {
            var bus = container.GetInstance<IBusControl>();

            HostFactory.Run(x =>
            {
                x.Service<Consumer>(s =>
                {
                    s.ConstructUsing(name => new Consumer(bus));
                    s.WhenStarted(c => c.Start());
                    s.WhenStopped(c => c.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Consumer Windows Service Prototype");
                x.SetDisplayName("Consumer Windows Service");
                x.SetServiceName("Consumer Windows Service");
            });
        }
    }
}
