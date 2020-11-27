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
			if (line <= 0 || line > KsiConstants.DISPLAY_MAX_LINES)
				throw new ArgumentOutOfRangeException(nameof(line), $"The line should be 1-{KsiConstants.DISPLAY_MAX_LINES}.");

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
		/// <summary>
		/// Upload panel log data with the option to specify last log entry or all. Most common use for this
		/// command is to retrieve panel log information from Keybox. The delete attribute if present with a
		/// value of yes will result in the deletion of all panel log entries. Note: this is true even if only
		/// uploading the last log entry only. If the delete attribute is not present or if the value is no then
		/// the panel log entries will not be deleted from the Keybox. This usage is much like a view only option.
		/// </summary>
		/// <param name="control">last|all</param>
		/// <param name="pos">yes|no</param>
		/// <returns>KeyBox string command</returns>
		public static string UploadPanel(KeyBoxUploadControlType control = KeyBoxUploadControlType.Last, KeyBoxYesNoType delete = KeyBoxYesNoType.No)
		{
			var _type = KeyBoxUploadType.Panel.ToString().ToLower();
			var _control = control.ToString().ToLower();
			var _delete = delete.ToString().ToLower();
			return $"<upload type={_type} control={_control} delete={_delete}></upload>";
		}

		/// <summary>
		/// Upload XML events with the option to specify starting and ending event range. Most common use for
		/// this command is to retrieve XML events from panel that may have been lost due to loss of network
		/// connectivity or other failures.The starting and ending event range is specified by using the eventid
		/// value. Note: the eventid value is specified in each event and is unique to that event. The panel
		/// increments the eventid for subsequent events. The host application should store the eventid in order
		/// to detect missing events.
		/// </summary>
		/// <param name="start">Starting eventid value</param>
		/// <param name="end">Ending eventid value</param>
		/// <returns>KeyBox string command</returns>
		public static string UploadEvent(int start, int end)
		{
			if (start < 0 || end < 0 || start > end)
				throw new ArgumentException("You must specify the correct start and end.");

			return $"<upload type={KeyBoxUploadType.Event.ToString().ToLower()} start={start} end={end}></upload>";
		}


		// ------------------------
		public static string UploadEvent(int start)
		{
			if (start < 0)
				throw new ArgumentException("You must specify the correct start and end.");

			return $"<upload type={KeyBoxUploadType.Event.ToString().ToLower()} start={start}></upload>";
		}
		public static string UploadEvent()
		{
			return $"<upload type={KeyBoxUploadType.Event.ToString().ToLower()}></upload>";
		}
		// ------------------------



		public static string Panel(string type, string name)
		{
			return $"<panel type={type} name={name}></panel>";
		}

		/// <summary>
		/// This command may be sent to retrieve asset status. The keyaudit command behaves somewhat different in the case 
		/// of assets that have serial numbers i.e. iButton or RFID and are not position dependent (meaning that they can be
		/// returned to any available position in the Keybox). Rather than in the case of simple presence detection type assets
		/// which must be returned to a specific position. In the non-position dependent case the keyaudit command requires
		/// the serial number of the asset to determine presence. While in the position dependent case the position number must
		/// be passed in the 'keyaudit' command to determine presence.
		/// </summary>
		/// <remarks>
		/// Note: keyaudit all option for position based assets will be supported in later releases.
		/// Note: the number of entries returned in a keyaudit reply is dependent on the Keybox size. Future design options
		/// include returning the number of records to be returned in a multipart XML command reply sequence
		/// </remarks>
		/// <param name="keyId">Key ID of key to be checked i.e. iButton or RFID serial number</param>
		/// <param name="type">all</param>
		/// <param name="pos">Position number of asset</param>
		/// <param name="poll">0|1 if set to 1 Keybox performs a low level poll of row board hardware</param>
		/// <returns>KeyBox string command</returns>
		public static string KeyAudit(string keyId = null, KeyBoxKeyAuditType? type = null, int? pos = null, KeyBoxKeyAuditPoll? poll = null)
		{
			var sb = new StringBuilder();
			sb.Append("<keyaudit");

			if (type.HasValue)
				sb.Append($" type={type.ToString().ToLower()}");

			if (pos.HasValue)
				sb.Append($" pos={pos.Value}");

			if (poll.HasValue)
				sb.Append($" poll={(int)poll.Value}");

			if (!string.IsNullOrWhiteSpace(keyId))
				sb.Append($">{keyId}</keyaudit>");
			else
				sb.Append($"></keyaudit>");

			return sb.ToString();
		}
	}
}
