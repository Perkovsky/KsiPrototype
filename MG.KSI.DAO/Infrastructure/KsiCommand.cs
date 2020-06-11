using MG.KSI.DAO.Extensions;
using MG.KSI.DAO.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MG.KSI.DAO.Infrastructure
{
	public static class KsiCommand
	{
		/// <summary>
		/// The panelping command may be used by the host application to verify that the GFMS XML parser
		/// is up and running.
		/// </summary>
		/// <returns>KSI string command</returns>
		public static string PanelPing() => "<panelping></panelping>";

		/// <summary>
		/// The opendoor command instructs the Keybox to energize the door solenoid thereby releasing the
		/// door latch. Some Keybox(s) are mechanically designed to open the door automatically via a spring
		/// other designs require the user to pull on the door handle.
		/// </summary>
		/// <remarks>
		/// Note: the appropriate door is opened automatically when sending a lightkey type=on command. It
		/// is recommended that opendoor is not used prior to releasing a key since an opendoor command
		/// will create a 5 second user session that prevents a lightkey command from being executed during
		/// that time interval (hence the lightkey command will be nacked).
		/// </remarks>
		/// <param name="door">Door number</param>
		/// <returns>KSI string command</returns>
		public static string OpenDoor(int door) => $"<opendoor door={door}></opendoor>";

		/// <summary>
		/// The display command allows the PC application to display text on the Keybox LCD screen.
		/// </summary>
		/// <remarks>
		/// Note: a "+" character should be used to display "space" characters within text to be displayed.
		/// 
		/// Note: alignment will be the responsibility of the Host application(i.e.pad with spaces accordingly)
		///			"+++++++++++++Hello+World"
		///			"++++++++Hello+World+++++"
		///			"Hello+World+++++++++++++"
		///
		/// Note: future implementation may allow the ability to ask for display size(assume 4x20 for now).
		/// </remarks>
		/// <param name="text">Text to be displayed</param>
		/// <param name="line">LCD line number to display text</param>
		/// <returns>KSI string command</returns>
		public static string Display(string text, int line = 1)
		{
			if (line <= 0 || line > KsiConatants.DISPLAY_MAX_LINES)
				throw new ArgumentOutOfRangeException(nameof(line), $"The line should be 1-{KsiConatants.DISPLAY_MAX_LINES}.");

			return $"<display line={line}>{text.KsiNormalize()}</display>";
		}

		/// <summary>
		/// For the ScanIT application type the lightkey command will light the key fob or key position
		/// associated with specified key serial number or position. Most common usage is to provide user
		/// feedback on key selection options. The ability to turn a key fob or position light off is also provided.
		/// </summary>
		/// <param name="type">on|off</param>
		/// <param name="pos">Position number of asset</param>
		/// <param name="userId">User ID of user to be assigned to removed assets (SKD application only)</param>
		/// <param name="keyList">"-" range operator and "," delimiter for key or range list</param>
		/// <param name="log">on|off</param>
		/// <param name="keySerialNumber">Key serial number of key fob or position to be lit</param>
		/// <returns>KSI string command</returns>
		public static string LightKey(int pos
			, KsiSwitchType type = KsiSwitchType.On
			, string userId = null
			, string keyList = null
			, KsiSwitchType? log = null
			, string keySerialNumber = null
		)
		{
			var sb = new StringBuilder();
			sb.Append($"<lightkey type={type.ToString().ToLower()} pos={pos}");

			if (!string.IsNullOrWhiteSpace(userId))
				sb.Append($" userid={userId}");

			if (!string.IsNullOrWhiteSpace(keyList))
				sb.Append($" keylist={keyList}");

			if (log != null)
				sb.Append($" log={log.ToString().ToLower()}");

			if (!string.IsNullOrWhiteSpace(keySerialNumber))
				sb.Append($">{keySerialNumber}</lightkey>");
			else
				sb.Append($"></lightkey>");

			return sb.ToString();
		}
	}
}
