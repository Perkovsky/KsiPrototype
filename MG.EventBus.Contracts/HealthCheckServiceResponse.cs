using System;
using System.Collections.Generic;

namespace MG.EventBus.Contracts
{
	public interface HealthCheckServiceResponse
	{
		string ServerId { get; }
		DateTime LastActivity { get; }
		Dictionary<string, bool> KeyBoxTcpClients { get; }
	}
}
