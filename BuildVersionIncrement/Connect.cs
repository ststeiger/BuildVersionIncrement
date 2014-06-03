using EnvDTE;
using Extensibility;
using Qreed.CodePlex;
using Qreed.Reflection;
using Qreed.VisualStudio;
using Qreed.Windows.Forms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace BuildVersionIncrement
{
	public class Connect : VSAddin
	{
		private OutputWindow _outputWindow;
		private OutputWindowPane _outputBuildWindow;
		private TaskList _taskList;
		private Logger _logger;
		private BuildVersionIncrementor _incrementor;
		private BuildEvents _buildEvents;
		public override string CommandBarResourceName
		{
			get
			{
				return "BuildVersionIncrement.CommandBar";
			}
		}
		private OutputWindow OutputWindow
		{
			get
			{
				if (this._outputWindow == null)
				{
					this._outputWindow = (OutputWindow)this.ApplicationObject.Windows.Item("{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}").Object;
				}
				return this._outputWindow;
			}
		}
		internal OutputWindowPane OutputBuildWindow
		{
			get
			{
				if (this._outputBuildWindow == null)
				{
					try
					{
						this._outputBuildWindow = this.OutputWindow.OutputWindowPanes.Item("{1BD8A850-02D1-11D1-BEE7-00A0C913D1F8}");
					}
					catch (ArgumentException ex)
					{
						Console.Write(ex.Message);
					}
				}
				return this._outputBuildWindow;
			}
		}
		private TaskList TaskList
		{
			get
			{
				if (this._taskList == null)
				{
					this._taskList = (TaskList)this.ApplicationObject.Windows.Item("{4A9B7E51-AA16-11D0-A8C5-00A0C921A4D2}").Object;
				}
				return this._taskList;
			}
		}
		public Connect()
		{
			this._logger = new Logger(this);
			this._incrementor = new BuildVersionIncrementor(this);
		}
		protected override VSMenu SetupMenuItems()
		{
			VSMenu vSMenu = new VSMenu(this, "Build Version Increment");
			VSMenuCommand vSMenuCommand = vSMenu.AddCommand("OnlineVersionCheck", "Check for a new version.", "Check online for a new version of this addin.");
			vSMenuCommand.Execute += new EventHandler(this.MenuVersionCheck_Execute);
			VSMenuCommand vSMenuCommand2 = vSMenu.AddCommand("BuildVersionIncrementSettings", "Settings", "Configure BuildVersionIncrement.");
			vSMenuCommand2.Execute += new EventHandler(this.DisplayAddinSettings);
			vSMenuCommand2.QueryStatus += new EventHandler<VSMenuQueryStatusEventArgs>(this.MenuSettingsQueryStatus);
			return vSMenu;
		}
		private void MenuSettingsQueryStatus(object sender, VSMenuQueryStatusEventArgs e)
		{
			if (this.ApplicationObject.Solution.IsOpen)
			{
				e.Status = (vsCommandStatus)3;
			}
			else
			{
				e.Status = (vsCommandStatus)17;
			}
		}
		private void MenuVersionCheck_Execute(object sender, EventArgs e)
		{
			WinWrapper owner = new WinWrapper(this);
			try
			{
				ProgressDialog progressDialog = new ProgressDialog();
				progressDialog.DoWork += new DoWorkEventHandler(this.MenuVersionCheckDoWork);
				progressDialog.Text = "Version Check ...";
				progressDialog.ProgressBar.Style = ProgressBarStyle.Marquee;
				progressDialog.AutoClose = true;
				progressDialog.ShowDialog(owner);
				if (progressDialog.Result.Exception != null)
				{
					throw progressDialog.Result.Exception;
				}
				if (!progressDialog.Result.IsCancelled)
				{
					VersionChecker versionChecker = (VersionChecker)progressDialog.Result.Value;
					if (versionChecker.OnlineVersion > versionChecker.LocalVersion)
					{
						this.DisplayVersionCheckerResult(versionChecker);
					}
					else
					{
						MessageBox.Show(owner, "Your local version is up to date.", "Version Check", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(owner, "Exception occured while checking for a new version:\n\n" + ex.ToString(), "Check for new version.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		private void DisplayVersionCheckerResult(VersionChecker vc)
		{
			string text = string.Concat(new string[]
			{
				"There's a new version available of this addin.\n\nLocal version: ",
				vc.LocalVersion.ToString(),
				"\nOnline version: ",
				vc.OnlineVersion.ToString(),
				"\n\nWould you like to open the project page on CodePlex to review the changes?"
			});
			WinWrapper owner = new WinWrapper(this);
			if (MessageBox.Show(owner, text, "BuildVersionIncrement", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				System.Diagnostics.Process.Start(vc.ProjectHomePage);
			}
		}
		private void MenuVersionCheckDoWork(object sender, DoWorkEventArgs e)
		{
			VersionChecker versionChecker = this.GetVersionChecker();
			e.Result = versionChecker;
			versionChecker.CheckForNewVersion();
		}
		private void DisplayAddinSettings(object sender, EventArgs e)
		{
			AddInSettingsForm addInSettingsForm = new AddInSettingsForm();
			WinWrapper owner = new WinWrapper(this);
			addInSettingsForm.Connect = this;
			addInSettingsForm.ShowDialog(owner);
		}
		public override void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			base.OnConnection(application, connectMode, addInInst, ref custom);
			this._buildEvents = this.ApplicationObject.Events.BuildEvents;
            // this._buildEvents.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(this._incrementor, (UIntPtr)ldftn(OnBuildBegin));
			// this._buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(this._incrementor, (UIntPtr)ldftn(OnBuildDone));


            _buildEvents.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(_incrementor.OnBuildBegin);
            _buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(_incrementor.OnBuildDone);


			if (connectMode == ext_ConnectMode.ext_cm_Startup)
			{
				Logger.Write("Checking online for a new version of BuildVersionIncrement.", LogLevel.Debug);
				try
				{
					VersionChecker versionChecker = this.GetVersionChecker();
					versionChecker.CheckForNewVersionComplete += new EventHandler<VersionCheckerEventArgs>(this.VersionChecker_CheckForNewVersionComplete);
					versionChecker.CheckForNewVersionASync();
				}
				catch (Exception)
				{
				}
				this._incrementor.InitializeIncrementors();
			}
		}
		private VersionChecker GetVersionChecker()
		{
			AssemblyConfigurationAttribute assemblyAttribute = ReflectionHelper.GetAssemblyAttribute<AssemblyConfigurationAttribute>(Assembly.GetExecutingAssembly());
			return this.GetVersionChecker(assemblyAttribute.Configuration);
		}
		private VersionChecker GetVersionChecker(string configuration)
		{
			return new VersionChecker
			{
				ProjectHomePage = "http://autobuildversion.codeplex.com/",
				VersionInfoUrl = "http://autobuildversion.codeplex.com/",
				Assembly = Assembly.GetExecutingAssembly(),
				Pattern = "Version\\ Info.+<table>.+?<tr><td>\\ " + configuration + "\\ </td><td>(?<Version>.+?)</td>",
				PatternOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace,
				UseAssemblyFileVersion = true
			};
		}
		private void VersionChecker_CheckForNewVersionComplete(object sender, VersionCheckerEventArgs e)
		{
			try
			{
				if (e.NewVersionAvailable)
				{
					VersionChecker versionChecker = (VersionChecker)sender;
					Logger.Write("New online version located: " + versionChecker.OnlineVersion, LogLevel.Info);
					this.DisplayVersionCheckerResult(versionChecker);
				}
				GlobalAddinSettings.Default.LastVersionCheck = DateTime.Now;
				GlobalAddinSettings.Default.Save();
			}
			catch (Exception)
			{
			}
		}
		public override void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
			//this._buildEvents.OnBuildBegin -= new _dispBuildEvents_OnBuildBeginEventHandler(this._incrementor, (UIntPtr)ldftn(OnBuildBegin));

            this._buildEvents.OnBuildBegin -= _incrementor.OnBuildBegin;

			base.OnDisconnection(disconnectMode, ref custom);
		}
	}
}
