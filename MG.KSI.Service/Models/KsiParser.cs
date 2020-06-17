using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MG.KSI.Service.Models
{
	public class KsiParser
	{
		public string Tag { get; private set; }
		public string Value { get; private set; }

		public IReadOnlyDictionary<string, string> Attributes { get; private set; }

		public KsiParser(string reply)
		{
			if (string.IsNullOrWhiteSpace(reply))
				throw new ArgumentException("Value cannot be null or empty.", nameof(reply));

			Parse(reply);
		}

		#region Private Methods

		private string GetRegexResult(string regexPattern, string parsedString)
		{
			var regex = new Regex(regexPattern);
			var match = regex.Match(parsedString);

			if (!match.Success)
				return string.Empty;

			var result = match.Groups.GetValueOrDefault("value").Value;
			return result;
		}

		private void FillAttributes(string parsedString)
		{
			var regex = new Regex(@"(?<key>\S*)=(?<value>\S*(?=\s|>\w*</\w*>$))");
			var matches = regex.Matches(parsedString);

			var result = new Dictionary<string, string>();
			foreach (Match match in matches)
			{
				var key = match.Groups.GetValueOrDefault("key").Value;
				var value = match.Groups.GetValueOrDefault("value").Value;

				if (string.IsNullOrWhiteSpace(key))
					continue;

				result.Add(key, value);
			}

			Attributes = result;
		}

		private void Parse(string reply)
		{
			Tag = GetRegexResult(@"<(?<value>\w*)", reply);
			Value = GetRegexResult(@">(?<value>\w*)<", reply);
			FillAttributes(reply);
		}

		#endregion

		//TODO: add method Map<T>()
	}
}
