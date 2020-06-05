namespace MG.KSI.Client.Extensions
{
	public static class StringExtensions
	{
		public static string KsiNormalize(this string str) => str.Substring(0, 20).Replace(" ", "+");
	}
}
