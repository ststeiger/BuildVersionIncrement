using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
namespace Qreed.VisualStudio
{
	public class VSMenu
	{
		private string _menuName;
		private IVSAddin _vsAddin;
		private string _toolsMenuName;
		private List<VSMenuCommand> _menuCommands = new List<VSMenuCommand>();
		private CommandBarPopup _popup;
		public IVSAddin VSAddin
		{
			get
			{
				return this._vsAddin;
			}
		}
		private string ToolsMenuName
		{
			get
			{
				if (this._toolsMenuName == null)
				{
					try
					{
						Assembly executingAssembly = Assembly.GetExecutingAssembly();
						ResourceManager resourceManager = new ResourceManager(this.VSAddin.CommandBarResourceName, executingAssembly);
						int localeID = this.VSAddin.ApplicationObject.LocaleID;
						CultureInfo cultureInfo = new CultureInfo(localeID);
                        string name = cultureInfo.TwoLetterISOLanguageName + "Tools";
						this._toolsMenuName = resourceManager.GetString(name);
					}
					catch
					{
                        this._toolsMenuName = "Tools";
					}
				}

                // this._toolsMenuName = "Extras";

				return this._toolsMenuName;
			}
		}
		public List<VSMenuCommand> MenuCommands
		{
			get
			{
				return this._menuCommands;
			}
		}
		public CommandBarPopup Popup
		{
			get
			{
				if (this._popup == null)
				{
					CommandBar commandBar = ((CommandBars)this.VSAddin.ApplicationObject.CommandBars)["MenuBar"];
					CommandBarControl commandBarControl = commandBar.Controls[this.ToolsMenuName];
					CommandBarPopup commandBarPopup = (CommandBarPopup)commandBarControl;
					this._popup = (CommandBarPopup)commandBarPopup.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, 1, true);
					this._popup.Caption = this._menuName;
				}
				return this._popup;
			}
		}
		public VSMenuCommand this[string commandName]
		{
			get
			{
				VSMenuCommand result;
				foreach (VSMenuCommand current in this._menuCommands)
				{
					if (current.CommandName == commandName)
					{
						result = current;
						return result;
					}
				}
				result = null;
				return result;
			}
		}
		public VSMenu(IVSAddin vsAddin, string menuName)
		{
			this._vsAddin = vsAddin;
			this._menuName = menuName;
		}
		public VSMenuCommand AddCommand(string commandName, string displayName, string description)
		{
			VSMenuCommand vSMenuCommand = new VSMenuCommand(this, commandName, displayName, description);
			this.MenuCommands.Add(vSMenuCommand);
			return vSMenuCommand;
		}
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				VSMenuCommand vSMenuCommand = this[commandName];
				if (vSMenuCommand != null)
				{
					vSMenuCommand.OnExecute();
					handled = true;
				}
			}
		}
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			VSMenuCommand vSMenuCommand = this[commandName];
			if (vSMenuCommand != null)
			{
				vSMenuCommand.OnQueryStatus(neededText, ref status, ref commandText);
			}
		}
	}
}
