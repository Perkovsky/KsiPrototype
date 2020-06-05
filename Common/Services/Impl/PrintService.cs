using System;

namespace Common.Services.Impl
{
	public class PrintService : IPrintService
	{
		public void PrintInfo(string text, bool useTimestamp = true)
		{
			if (useTimestamp)
				Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fffff}] - {text}");
			else
				Console.WriteLine(text);
		}

		public void PrintError(string text)
		{
			var defaultTextColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(text);
			Console.ForegroundColor = defaultTextColor;
		}
	}
}
