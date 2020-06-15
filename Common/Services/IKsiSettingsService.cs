using Common.Models;
using MG.EventBus.Startup.Models;
using System.Collections.Generic;

namespace Common.Services
{
	public interface IKsiSettingsService
	{
		IEnumerable<KsiSettings> GetKsiSettings();
		EventBusSettings GetEventBusSetting();
	}
}
