using System;

namespace MG.EventBus.Contracts
{
	public interface KeyboxEvent
	{
		string KeyBoxTcpClientId { get; }
		DateTime CreatedDate { get; }
		string Event { get; } // Command Reply or Event
	}
}
