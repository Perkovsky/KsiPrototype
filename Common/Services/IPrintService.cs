namespace Common.Services
{
	public interface IPrintService
	{
		void PrintInfo(string text, bool useTimestamp = true);
		void PrintError(string text);
	}
}
