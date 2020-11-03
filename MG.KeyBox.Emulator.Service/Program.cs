using MG.KeyBox.Emulator.Service.Models;
using Topshelf;

namespace MG.KeyBox.Emulator.Service
{
	class Program
	{
        static void Main(string[] args)
        {
            //TODO: get from appSettings.json
            var settings = new KeyBoxEmulatorSettings
            {
                Port = 1010,
                IPAddress = "127.0.0.1"
            };

            HostFactory.Run(x =>
            {
                x.Service<KeyBoxEmulator>(s =>
                {
                    s.ConstructUsing(name => new KeyBoxEmulator(settings));
                    s.WhenStarted(async c => await c.StartAsync());
                    s.WhenStopped(c => c.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("KeyBox Emulator Service");
                x.SetDisplayName("KeyBox Emulator Service");
                x.SetServiceName("KeyBox Emulator Service");
            });
        }
    }
}
