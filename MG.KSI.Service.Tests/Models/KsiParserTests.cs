using MG.KSI.Service.Models;
using System;
using System.Linq;
using Xunit;

namespace MG.KSI.Service.Tests.Models
{
	[Trait("MG.KSI.Service", "KsiParser")]
	public class KsiParserTests
	{
		[Fact]
		public void CreateKsiParser_Panelping_ReturnsKsiParser()
		{
			// Arrange
			var reply = @"<panelping></panelping>";

			// Act
			var result = new KsiParser(reply);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("panelping", result.Tag);
			Assert.Equal(string.Empty, result.Value);
			Assert.False(result.Attributes.Any());
		}

		[Fact]
		public void CreateKsiParser_PanelpingReply_ReturnsKsiParser()
		{
			// Arrange
			var reply = @"<cmdreply panelid=0050c23d8000 status=ack>panelping</cmdreply>";

			// Act
			var result = new KsiParser(reply);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("cmdreply", result.Tag);
			Assert.Equal("panelping", result.Value);
			Assert.Equal(2, result.Attributes.Count());
			Assert.Equal("0050c23d8000", result.Attributes["panelid"]);
			Assert.Equal("ack", result.Attributes["status"]);
		}

		[Fact]
		public void CreateKsiParser_OpendoorReply_ReturnsKsiParser()
		{
			// Arrange
			var reply = @"<cmdreply panelid=0050c23d8000 status=ack>opendoor</cmdreply>";

			// Act
			var result = new KsiParser(reply);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("cmdreply", result.Tag);
			Assert.Equal("opendoor", result.Value);
			Assert.Equal(2, result.Attributes.Count());
			Assert.Equal("0050c23d8000", result.Attributes["panelid"]);
			Assert.Equal("ack", result.Attributes["status"]);
		}

		[Fact]
		public void CreateKsiParser_Alarmevent_ReturnsKsiParser()
		{
			// Arrange
			string reply = @"<alarmevent panelid=0050c23d8000 eventid=3341 date=2004/12/14 time=15:44:30 logtype=72 type=set condition=timer door=1>door</alarmevent>";

			// Act
			var result = new KsiParser(reply);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("alarmevent", result.Tag);
			Assert.Equal("door", result.Value);
			Assert.Equal(8, result.Attributes.Count());
			Assert.Equal("15:44:30", result.Attributes["time"]);
		}

		[Fact]
		public void CreateKsiParser_Doorevent_ReturnsKsiParser()
		{
			// Arrange
			string reply = @"<doorevent panelid=b827eb82438d eventid=717 date=2020/06/05 time=10:27:10 logtype=40 userid= door=1>opened</doorevent>";

			// Act
			var result = new KsiParser(reply);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("doorevent", result.Tag);
			Assert.Equal("opened", result.Value);
			Assert.Equal(7, result.Attributes.Count());
			Assert.Equal(string.Empty, result.Attributes["userid"]);
			Assert.Equal("2020/06/05", result.Attributes["date"]);
		}

		[Fact]
		public void CreateKsiParser_Keyevent_ReturnsKsiParser()
		{
			// Arrange
			string reply = @"<keyevent panelid=b827eb82438d eventid=722 date=2020/06/05 time=10:27:39 logtype=32 pos=34 ppos=34 tagid=b400001b78c9cb01 door=1 userid=>removed</keyevent>";

			// Act
			var result = new KsiParser(reply);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("keyevent", result.Tag);
			Assert.Equal("removed", result.Value);
			Assert.Equal(10, result.Attributes.Count());
			Assert.Equal(string.Empty, result.Attributes["userid"]);
			Assert.Equal("722", result.Attributes["eventid"]);
		}

		[Fact]
		public void CreateKsiParser_Panelevent_ReturnsKsiParser()
		{
			// Arrange
			string reply = @"<panelevent panelid=b827eb82438d eventid=716 date=2020/06/05 time=10:26:18 logtype=90 assets=40 doors=1 xmlconfig=00 version=0.20190919a cpu=rpi>connected</panelevent>";

			// Act
			var result = new KsiParser(reply);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("panelevent", result.Tag);
			Assert.Equal("connected", result.Value);
			Assert.Equal(10, result.Attributes.Count());
			Assert.Equal("0.20190919a", result.Attributes["version"]);
		}
	}
}
