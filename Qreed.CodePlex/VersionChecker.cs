using Qreed.Reflection;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
namespace Qreed.CodePlex
{
	public class VersionChecker : Component
	{
		private string _versionInfoUrl;
		private string _projectHomePage;
		private Assembly _assembly;
		private bool _useAssemblyFileVersion;
		private string _pattern = "<div\\ class=\"wikidoc\">.*?Current\\ version.+?\\ (?<Version>.+?)</div>";
		private RegexOptions _patternOptions = RegexOptions.IgnoreCase;
		private Version _onlineVersion;
		private IContainer components = null;
		private BackgroundWorker backgroundWorker;
		public event EventHandler<VersionCheckerEventArgs> CheckForNewVersionComplete;
		public string VersionInfoUrl
		{
			get
			{
				string result;
				if (string.IsNullOrEmpty(this._versionInfoUrl) && !string.IsNullOrEmpty(this.ProjectHomePage))
				{
					string text = this.ProjectHomePage.TrimEnd("/".ToCharArray());
					text += "/Release/ProjectReleases.aspx?ReleaseName=VersionInfo";
					result = text;
				}
				else
				{
					result = this._versionInfoUrl;
				}
				return result;
			}
			set
			{
				this._versionInfoUrl = value;
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public string ProjectHomePage
		{
			get
			{
				return this._projectHomePage;
			}
			set
			{
				this._projectHomePage = value;
			}
		}
		[Browsable(false)]
		public Assembly Assembly
		{
			get
			{
				Assembly result;
				if (this._assembly == null)
				{
					result = Assembly.GetEntryAssembly();
				}
				else
				{
					result = this._assembly;
				}
				return result;
			}
			set
			{
				this._assembly = value;
			}
		}
		public bool UseAssemblyFileVersion
		{
			get
			{
				return this._useAssemblyFileVersion;
			}
			set
			{
				this._useAssemblyFileVersion = value;
			}
		}
		public string Pattern
		{
			get
			{
				return this._pattern;
			}
			set
			{
				this._pattern = value;
			}
		}
		public RegexOptions PatternOptions
		{
			get
			{
				return this._patternOptions;
			}
			set
			{
				this._patternOptions = value;
			}
		}
		[Browsable(false)]
		public Version LocalVersion
		{
			get
			{
				Version result;
				if (this.UseAssemblyFileVersion)
				{
					AssemblyFileVersionAttribute assemblyAttribute = ReflectionHelper.GetAssemblyAttribute<AssemblyFileVersionAttribute>(this.Assembly);
					result = new Version(assemblyAttribute.Version);
				}
				else
				{
					result = this.Assembly.GetName().Version;
				}
				return result;
			}
		}
		[Browsable(false), DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Version OnlineVersion
		{
			get
			{
				if (this._onlineVersion == null)
				{
					this._onlineVersion = this.GetOnlineVersion();
				}
				return this._onlineVersion;
			}
		}
		public VersionChecker()
		{
			this.InitializeComponent();
		}
		public VersionChecker(IContainer container)
		{
			container.Add(this);
			this.InitializeComponent();
		}
		public bool CheckForNewVersion()
		{
			return this.OnlineVersion > this.LocalVersion;
		}
		public void CheckForNewVersionASync()
		{
			this.backgroundWorker.RunWorkerAsync();
		}
		private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			e.Result = this.CheckForNewVersion();
		}
		private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			bool newVersionAvailable = false;
			try
			{
				if (e.Result != null)
				{
					newVersionAvailable = (bool)e.Result;
				}
			}
			catch
			{
			}
			if (this.CheckForNewVersionComplete != null)
			{
				VersionCheckerEventArgs e2 = new VersionCheckerEventArgs(e.Error, newVersionAvailable);
				this.CheckForNewVersionComplete(this, e2);
			}
		}
		private Version GetOnlineVersion()
		{
			WebClient webClient = new WebClient();
			if (string.IsNullOrEmpty(this.VersionInfoUrl))
			{
				throw new Exception("VersionInfoUrl not set.");
			}
			string text = webClient.DownloadString(this.VersionInfoUrl);
			if (string.IsNullOrEmpty(text))
			{
				throw new Exception("Empty response from \"" + this.VersionInfoUrl + "\".");
			}
			Match match = Regex.Match(text, this.Pattern, this.PatternOptions);
			if (!match.Success)
			{
				throw new Exception("Could not locate online version information from \"" + this.VersionInfoUrl + "\".");
			}
			return new Version(match.Groups["Version"].Value);
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			this.backgroundWorker = new BackgroundWorker();
			this.backgroundWorker.DoWork += new DoWorkEventHandler(this.BackgroundWorkerDoWork);
			this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BackgroundWorkerRunWorkerCompleted);
		}
	}
}
