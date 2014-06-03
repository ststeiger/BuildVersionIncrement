using System;
using System.Drawing;
using System.Runtime.InteropServices;
namespace Etier.IconHelper
{
	public sealed class IconReader
	{
		private IconReader()
		{
		}
		public static Icon GetFileIcon(string name, IconSize size, bool linkOverlay)
		{
			Shell32.SHFILEINFO sHFILEINFO = default(Shell32.SHFILEINFO);
			uint num = 272u;
			if (linkOverlay)
			{
				num |= 32768u;
			}
			if (IconSize.Small == size)
			{
				num |= 1u;
			}
			else
			{
				num = num;
			}
			Shell32.SHGetFileInfo(name, 128u, ref sHFILEINFO, (uint)Marshal.SizeOf(sHFILEINFO), num);
			Icon result = (Icon)Icon.FromHandle(sHFILEINFO.hIcon).Clone();
			User32.DestroyIcon(sHFILEINFO.hIcon);
			return result;
		}
		public static Icon GetFolderIcon(IconSize size, FolderType folderType)
		{
			uint num = 272u;
			if (FolderType.Open == folderType)
			{
				num |= 2u;
			}
			if (IconSize.Small == size)
			{
				num |= 1u;
			}
			else
			{
				num = num;
			}
			Shell32.SHFILEINFO sHFILEINFO = default(Shell32.SHFILEINFO);
			Shell32.SHGetFileInfo(Environment.CurrentDirectory, 16u, ref sHFILEINFO, (uint)Marshal.SizeOf(sHFILEINFO), num);
			Icon.FromHandle(sHFILEINFO.hIcon);
			Icon result = (Icon)Icon.FromHandle(sHFILEINFO.hIcon).Clone();
			User32.DestroyIcon(sHFILEINFO.hIcon);
			return result;
		}
	}
}
