﻿namespace MG.KSI.DAO.Extensions
{
	public static class StringExtensions
	{
		public static string KsiNormalize(this string str)
		{
			int length = str.Length > KsiConatants.DISPLAY_LINE_MAX_LENGTH ? KsiConatants.DISPLAY_LINE_MAX_LENGTH : str.Length;
			return str.Substring(0, length).Replace(" ", "+");
		}
	}
}