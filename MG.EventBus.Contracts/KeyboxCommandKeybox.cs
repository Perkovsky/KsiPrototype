using System;

namespace MG.EventBus.Contracts
{
	public interface KeyboxCommandKeybox
	{
		string KeyBoxTcpClientId { get; }
		DateTime CreatedDate { get; }
		string KeyBoxCommand { get; }
	}
}
