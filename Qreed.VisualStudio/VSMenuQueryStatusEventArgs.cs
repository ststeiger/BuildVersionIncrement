using EnvDTE;
using System;
namespace Qreed.VisualStudio
{
	public class VSMenuQueryStatusEventArgs : EventArgs
	{
		private vsCommandStatusTextWanted _neededText;
		private vsCommandStatus _status;
		private object _commandText;
		public vsCommandStatusTextWanted NeededText
		{
			get
			{
				return this._neededText;
			}
			set
			{
				this._neededText = value;
			}
		}
		public vsCommandStatus Status
		{
			get
			{
				return this._status;
			}
			set
			{
				this._status = value;
			}
		}
		public object CommandText
		{
			get
			{
				return this._commandText;
			}
			set
			{
				this._commandText = value;
			}
		}
	}
}
