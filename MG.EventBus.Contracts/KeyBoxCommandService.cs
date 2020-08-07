namespace MG.EventBus.Contracts
{
	public interface KeyboxCommandService
	{
		string KeyBoxTcpClientId { get; }
		string IPAddress { get; }
		ServiceCommandType Command { get; }
	}
}
