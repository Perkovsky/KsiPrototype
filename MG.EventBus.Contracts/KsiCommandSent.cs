using System;

namespace MG.EventBus.Contracts
{
	public interface KsiCommandSent
	{
		string KsiTcpClientId { get; }
		DateTime CreatedDate { get; }
		string KsiCommand { get; }
	}
}
