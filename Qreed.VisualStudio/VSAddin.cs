using EnvDTE;
using EnvDTE80;
using Extensibility;
using System;
using System.Diagnostics;
namespace Qreed.VisualStudio
{
	public abstract class VSAddin : IVSAddin, IDTExtensibility2, IDTCommandTarget
	{
		private VSMenu _rootMenu;
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
		private ext_ConnectMode _connectMode;
		protected VSMenu RootMenu
		{
			get
			{
				return this._rootMenu;
			}
		}
		public virtual DTE2 ApplicationObject
		{
			get
			{
				return this._applicationObject;
			}
		}
		public virtual AddIn AddInInstance
		{
			get
			{
				return this._addInInstance;
			}
		}
		public virtual string CommandBarResourceName
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public ext_ConnectMode ConnectMode
		{
			get
			{
				return this._connectMode;
			}
		}
		public bool IsCommandLineBuild
		{
			get
			{
				return this._connectMode == ext_ConnectMode.ext_cm_CommandLine;
			}
		}
		protected virtual VSMenu SetupMenuItems()
		{
			return null;
		}
		public virtual void OnAddInsUpdate(ref Array custom)
		{
		}
		public virtual void OnBeginShutdown(ref Array custom)
		{
		}
		public virtual void OnConnection(object Application, ext_ConnectMode connectMode, object AddInInst, ref Array custom)
		{
			this._applicationObject = (DTE2)Application;
			this._addInInstance = (AddIn)AddInInst;
			this._connectMode = connectMode;
			if (this.ConnectMode == ext_ConnectMode.ext_cm_Startup)
			{
				this._rootMenu = this.SetupMenuItems();
			}
		}
		public virtual void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}
		public virtual void OnStartupComplete(ref Array custom)
		{
		}
		public virtual void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			Trace.WriteLine(string.Concat(new object[]
			{
				"Exec ",
				commandName,
				" ",
				executeOption
			}));
			if (this.RootMenu != null)
			{
				this.RootMenu.Exec(commandName, executeOption, ref varIn, ref varOut, ref handled);
			}
		}
		public virtual void QueryStatus(string CmdName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object CommandText)
		{
			Trace.WriteLine(string.Concat(new object[]
			{
				"QueryStatus ",
				CmdName,
				" ",
				neededText
			}));
			if (this.RootMenu != null)
			{
				this.RootMenu.QueryStatus(CmdName, neededText, ref status, ref CommandText);
			}
		}
	}
}
