using System;
using System.Windows.Forms;
namespace BuildVersionIncrement
{
	internal class WinWrapper : IWin32Window
	{
		private Connect _c;
		public IntPtr Handle
		{
			get
			{
				IntPtr result = new IntPtr(this._c.ApplicationObject.MainWindow.HWnd);
				return result;
			}
		}
		public WinWrapper(Connect connect)
		{
			this._c = connect;
		}
	}
}
