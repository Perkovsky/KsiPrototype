using Common.Models;
using MG.EventBus.Startup.Models;

namespace Common.Services
{
	public interface IKsiSettingsService
	{
		KsiSettings GetKsiSettings();
		EventBusSettings GetEventBusSetting();
	}
}
