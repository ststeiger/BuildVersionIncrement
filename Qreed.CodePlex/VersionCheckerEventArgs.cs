using System;
namespace Qreed.CodePlex
{
	public class VersionCheckerEventArgs : EventArgs
	{
		private Exception _error;
		private bool _newVersionAvailable;
		public Exception Error
		{
			get
			{
				return this._error;
			}
		}
		public bool NewVersionAvailable
		{
			get
			{
				return this._newVersionAvailable;
			}
		}
		internal VersionCheckerEventArgs(Exception error, bool newVersionAvailable)
		{
			this._error = error;
			this._newVersionAvailable = newVersionAvailable;
		}
	}
}
