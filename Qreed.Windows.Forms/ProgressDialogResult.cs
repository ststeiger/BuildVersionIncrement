using System;
namespace Qreed.Windows.Forms
{
	public class ProgressDialogResult
	{
		private object _value;
		private bool _isCancelled;
		private Exception _exception;
		public object Value
		{
			get
			{
				return this._value;
			}
		}
		public bool IsCancelled
		{
			get
			{
				return this._isCancelled;
			}
		}
		public Exception Exception
		{
			get
			{
				return this._exception;
			}
		}
		internal ProgressDialogResult(object value, bool isCancelled, Exception ex)
		{
			this._value = value;
			this._isCancelled = isCancelled;
			this._exception = ex;
		}
	}
}
