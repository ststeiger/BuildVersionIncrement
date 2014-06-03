using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using stdole;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace Qreed.VisualStudio
{
	public class VSMenuCommand
	{
		private class ImageConverter : AxHost
		{
			public ImageConverter() : base("59EE46BA-677D-4d20-BF10-8D8067CB8B33")
			{
			}
			public static StdPicture ImageToIPicture(Image image)
			{
				return (StdPicture)AxHost.GetIPictureDispFromPicture(image);
			}
		}
		private VSMenu _menu;
		private string _name;
		private string _displayName;
		private string _description;
		private Command _command;
		private CommandBarButton _button;
		private event EventHandler _execute;
		public event EventHandler Execute
		{
			add
			{
				this._execute += value;
			}
			remove
			{
				this._execute -= value;
			}
		}
		private event EventHandler<VSMenuQueryStatusEventArgs> _queryStatus;
		public event EventHandler<VSMenuQueryStatusEventArgs> QueryStatus
		{
			add
			{
				this._queryStatus += value;
			}
			remove
			{
				this._queryStatus -= value;
			}
		}
		public VSMenu Menu
		{
			get
			{
				return this._menu;
			}
		}
		public string Name
		{
			get
			{
				return this._name;
			}
		}
		public string DisplayName
		{
			get
			{
				return this._displayName;
			}
		}
		public string Description
		{
			get
			{
				return this._description;
			}
		}
		public Command Command
		{
			get
			{
				return this._command;
			}
		}
		public string CommandName
		{
			get
			{
				return this._command.Name;
			}
		}
		public CommandBarButton Button
		{
			get
			{
				return this._button;
			}
		}
		public VSMenuCommand(VSMenu menu, string name, string displayName, string description)
		{
			this._menu = menu;
			this._name = name;
			this._displayName = displayName;
			this._description = description;
			object[] array = new object[0];
			Commands2 commands = (Commands2)menu.VSAddin.ApplicationObject.Commands;
			try
			{
				this._command = commands.AddNamedCommand2(menu.VSAddin.AddInInstance, name, displayName, description, true, 0, ref array, 3, 3, vsCommandControlType.vsCommandControlTypeButton);
			}
			catch
			{
				this._command = commands.Item(menu.VSAddin.AddInInstance.ProgID + "." + name, 0);
			}
			this._button = (CommandBarButton)this._command.AddControl(this._menu.Popup.CommandBar, 1);
		}
		public virtual void OnExecute()
		{
			Trace.WriteLine("OnExecute: " + this.CommandName);
			if (this._execute != null)
			{
				this._execute(this, null);
			}
		}
		public void SetImage(Image image)
		{
			StdPicture picture = VSMenuCommand.ImageConverter.ImageToIPicture(image);
			this.Button.Picture = picture;
		}
		internal void OnQueryStatus(vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if (this._queryStatus != null)
			{
				VSMenuQueryStatusEventArgs vSMenuQueryStatusEventArgs = new VSMenuQueryStatusEventArgs();
				vSMenuQueryStatusEventArgs.NeededText = neededText;
				vSMenuQueryStatusEventArgs.Status = status;
				vSMenuQueryStatusEventArgs.CommandText = commandText;
				this._queryStatus(this, vSMenuQueryStatusEventArgs);
				status = vSMenuQueryStatusEventArgs.Status;
			}
			else
			{
				if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
				{
					status = (vsCommandStatus)3;
				}
			}
		}
	}
}
