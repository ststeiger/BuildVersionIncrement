using System;
using System.Runtime.InteropServices;
namespace Etier.IconHelper
{
	public class User32
	{
		[DllImport("User32.dll")]
		public static extern int DestroyIcon(IntPtr hIcon);
	}
}
