using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MG.KSI.Tests.Models
{
	public class KsiParser
	{
		public string Tag { get; set; }
		public string Value { get; set; }

		public IDictionary<string, string> Attributes { get; set; }

		public KsiParser(string reply)
		{
			if (string.IsNullOrWhiteSpace(reply))
				throw new ArgumentException("Value cannot be null or empty.", nameof(reply));

			Parse(reply);
		}

		private void Parse(string reply)
		{
			
		}

		//TODO: add method Map<T>()
	}

	[Trait("MG.KSI", "KsiParser")]
	public class KsiParserTests
	{
		[Fact]
		public void CanTenantRegister_AllEnrollsDisabled_ReturnsTrue()
		{
			// Arrange
			string reply = @"<alarmevent panelid=0050c23d8000 eventid=3341 date=2004/12/14 time=15:44:30 logtype=72 type=set condition=timer door=1>door</alarmevent>";

			// Act
			KsiParser result = null;
			try
			{
				result = new KsiParser(reply);
			}
			catch (Exception e)
			{
				throw;
			}

			// Assert
			Assert.NotNull(result);
			Assert.Equal("alarmevent", result.Tag);
			Assert.Equal("door", result.Value);
			Assert.Equal(8, result.Tag.Count());
		}
	}
}
