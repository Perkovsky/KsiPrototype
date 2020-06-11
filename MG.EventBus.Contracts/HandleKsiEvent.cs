using System;

namespace MG.EventBus.Contracts
{
	public interface HandleKsiEvent
	{
		string KsiTcpClientId { get; }
		DateTime CreatedDate { get; }
		string Event { get; } // Command Reply or Event
	}
}
