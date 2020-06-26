using System;

namespace MG.EventBus.Contracts
{
	public interface SendKsiCommand
	{
		string KsiTcpClientId { get; }
		DateTime CreatedDate { get; }
		string KsiCommand { get; }
	}
}
