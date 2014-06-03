using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
namespace Etier.IconHelper
{
	public class IconListManager
	{
		private Hashtable _extensionList = new Hashtable();
		private ArrayList _imageLists = new ArrayList();
		private IconSize _iconSize;
		private bool _manageBothSizes;
		public IconListManager(ImageList imageList, IconSize iconSize)
		{
			this._imageLists.Add(imageList);
			this._iconSize = iconSize;
		}
		public IconListManager(ImageList smallImageList, ImageList largeImageList)
		{
			this._imageLists.Add(smallImageList);
			this._imageLists.Add(largeImageList);
			this._manageBothSizes = true;
		}
		private void AddExtension(string Extension, int ImageListPosition)
		{
			this._extensionList.Add(Extension, ImageListPosition);
		}
		public int AddFileIcon(string filePath)
		{
			string text = Path.GetExtension(filePath).TrimStart(".".ToCharArray());
			int result;
			if (this._extensionList.ContainsKey(text.ToUpper()))
			{
				result = (int)this._extensionList[text.ToUpper()];
			}
			else
			{
				int count = ((ImageList)this._imageLists[0]).Images.Count;
				if (this._manageBothSizes)
				{
					((ImageList)this._imageLists[0]).Images.Add(IconReader.GetFileIcon(filePath, IconSize.Small, false));
					((ImageList)this._imageLists[1]).Images.Add(IconReader.GetFileIcon(filePath, IconSize.Large, false));
				}
				else
				{
					((ImageList)this._imageLists[0]).Images.Add(IconReader.GetFileIcon(filePath, this._iconSize, false));
				}
				this.AddExtension(text.ToUpper(), count);
				result = count;
			}
			return result;
		}
		public int AddFolderIcon(FolderType type)
		{
			string text = "folder_" + type.ToString();
			int result;
			if (this._extensionList.ContainsKey(text))
			{
				result = (int)this._extensionList[text];
			}
			else
			{
				int count = ((ImageList)this._imageLists[0]).Images.Count;
				if (this._manageBothSizes)
				{
					((ImageList)this._imageLists[0]).Images.Add(IconReader.GetFolderIcon(IconSize.Small, type));
					((ImageList)this._imageLists[1]).Images.Add(IconReader.GetFolderIcon(IconSize.Large, type));
				}
				else
				{
					((ImageList)this._imageLists[0]).Images.Add(IconReader.GetFolderIcon(this._iconSize, type));
				}
				this.AddExtension(text, count);
				result = count;
			}
			return result;
		}
		public void ClearLists()
		{
			foreach (ImageList imageList in this._imageLists)
			{
				imageList.Images.Clear();
			}
			this._extensionList.Clear();
		}
	}
}
