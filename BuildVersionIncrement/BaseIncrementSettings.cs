using System;
using System.ComponentModel;
namespace BuildVersionIncrement
{
	[DefaultProperty("VersioningStyle")]
	internal abstract class BaseIncrementSettings
	{
		private bool _replaceNonNumerics;
		private DateTime _projectStartDate;
		private bool _autoUpdateFileVersion;
		private bool _autoUpdateAssemblyVersion;
		private bool _isUniversalTime;
		private VersioningStyle _versioningStyle = new VersioningStyle();
		private BuildActionType _buildAction;
		private bool _incrementBeforeBuild;
		private bool _DetectChanges = true;
		[Category("Increment Settings"), DefaultValue(true), Description("If non-numeric values within the version should be replaced with a zero."), DisplayName("Replace Non-Numerics")]
		public bool ReplaceNonNumerics
		{
			get
			{
				return this._replaceNonNumerics;
			}
			set
			{
				this._replaceNonNumerics = value;
			}
		}
		[Category("Increment Settings"), DefaultValue(typeof(DateTime), "1975/10/21"), Description("The start date to use."), DisplayName("Start Date")]
		public DateTime StartDate
		{
			get
			{
				return this._projectStartDate;
			}
			set
			{
				this._projectStartDate = value;
			}
		}
		[Category("Increment Settings"), DefaultValue(false), Description("Auto update the file version. Note that setting this to true on solution level will have no effect on building individual projects."), DisplayName("Update AssemblyFileVersion")]
		public bool AutoUpdateFileVersion
		{
			get
			{
				return this._autoUpdateFileVersion;
			}
			set
			{
				this._autoUpdateFileVersion = value;
			}
		}
		[Category("Increment Settings"), DefaultValue(false), Description("Auto update the assembly version. Note that setting this to true on solution level will have no effect on building individual projects."), DisplayName("Update AssemblyVersion")]
		public bool AutoUpdateAssemblyVersion
		{
			get
			{
				return this._autoUpdateAssemblyVersion;
			}
			set
			{
				this._autoUpdateAssemblyVersion = value;
			}
		}
		[Category("Increment Settings"), DefaultValue(false), Description("Indicates wheter to use Coordinated Universal Time (UTC) time stamps."), DisplayName("Use Coordinated Universal Time")]
		public bool IsUniversalTime
		{
			get
			{
				return this._isUniversalTime;
			}
			set
			{
				this._isUniversalTime = value;
			}
		}
		[Browsable(true), Category("Increment Settings"), Description("The version increment style settings."), DisplayName("Versioning Style")]
		public VersioningStyle VersioningStyle
		{
			get
			{
				return this._versioningStyle;
			}
			set
			{
				this._versioningStyle = value;
			}
		}
		[Category("Condition"), DefaultValue(BuildActionType.Both), Description("Set this to the desired build action when the auto update should occur."), DisplayName("Build Action")]
		public BuildActionType BuildAction
		{
			get
			{
				return this._buildAction;
			}
			set
			{
				this._buildAction = value;
			}
		}
		[Category("Condition"), DefaultValue(true), Description("If the increment should be executed before the build."), DisplayName("Increment Before Build")]
		public bool IncrementBeforeBuild
		{
			get
			{
				return this._incrementBeforeBuild;
			}
			set
			{
				this._incrementBeforeBuild = value;
			}
		}
		[Category("Condition"), DefaultValue(true), Description("Set this to true if you want to detect item changes in order to make version increment."), DisplayName("Detect changes")]
		public bool DetectChanges
		{
			get
			{
				return this._DetectChanges;
			}
			set
			{
				this._DetectChanges = value;
			}
		}
		public abstract void Load();
		public abstract void Save();
		public abstract void Reset();
		public virtual void CopyFrom(BaseIncrementSettings source)
		{
			try
			{
				this.VersioningStyle = new VersioningStyle(source.VersioningStyle);
				this.AutoUpdateAssemblyVersion = source.AutoUpdateAssemblyVersion;
				this.AutoUpdateFileVersion = source.AutoUpdateFileVersion;
				this.BuildAction = source.BuildAction;
				this.StartDate = source.StartDate;
				this.IsUniversalTime = source.IsUniversalTime;
				this.ReplaceNonNumerics = source.ReplaceNonNumerics;
				this.IncrementBeforeBuild = source.IncrementBeforeBuild;
				this.DetectChanges = source.DetectChanges;
			}
			catch (Exception ex)
			{
				Logger.Write("Exception occured while copying settings: " + ex.ToString(), LogLevel.Error);
			}
		}
		private bool ShouldSerializeVersioningStyle()
		{
			return this._versioningStyle.ToString() != "None.None.None.None";
		}
	}
}
