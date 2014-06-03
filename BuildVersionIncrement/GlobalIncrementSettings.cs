using System;
using System.ComponentModel;
namespace BuildVersionIncrement
{
	internal class GlobalIncrementSettings : BaseIncrementSettings
	{
		public enum ApplyGlobalSettings
		{
			OnlyWhenChosen,
			AsDefault,
			Always
		}
		private GlobalIncrementSettings.ApplyGlobalSettings _apply;
		[Browsable(true), Category("Global"), DefaultValue(typeof(GlobalIncrementSettings.ApplyGlobalSettings), "OnlyWhenChosen"), Description("The setting when to use the Global Settings"), DisplayName("Apply Global Settings")]
		public GlobalIncrementSettings.ApplyGlobalSettings Apply
		{
			get
			{
				return this._apply;
			}
			set
			{
				this._apply = value;
			}
		}
		public static GlobalIncrementSettings.ApplyGlobalSettings ApplySettings
		{
			get
			{
				return (GlobalIncrementSettings.ApplyGlobalSettings)Enum.Parse(typeof(GlobalIncrementSettings.ApplyGlobalSettings), GlobalAddinSettings.Default.GlobalApply);
			}
		}
		public override void Load()
		{
			VersioningStyle versioningStyle = new VersioningStyle();
			versioningStyle.FromGlobalVariable(string.Concat(new string[]
			{
				GlobalAddinSettings.Default.GlobalMajor,
				".",
				GlobalAddinSettings.Default.GlobalMinor,
				".",
				GlobalAddinSettings.Default.GlobalBuild,
				".",
				GlobalAddinSettings.Default.GlobalRevision
			}));
			base.VersioningStyle = versioningStyle;
			base.BuildAction = (BuildActionType)Enum.Parse(typeof(BuildActionType), GlobalAddinSettings.Default.GlobalBuildAction);
			base.AutoUpdateAssemblyVersion = GlobalAddinSettings.Default.GlobalAutoUpdateAssemblyVersion;
			base.AutoUpdateFileVersion = GlobalAddinSettings.Default.GlobalAutoUpdateFileVersion;
			base.ReplaceNonNumerics = GlobalAddinSettings.Default.GlobalReplaceNonNumeric;
			base.IsUniversalTime = GlobalAddinSettings.Default.GlobalUseUniversalClock;
			base.IncrementBeforeBuild = GlobalAddinSettings.Default.GlobalIncrementBeforeBuild;
			base.StartDate = GlobalAddinSettings.Default.GlobalStartDate;
			base.DetectChanges = GlobalAddinSettings.Default.DetectChanges;
			this.Apply = (GlobalIncrementSettings.ApplyGlobalSettings)Enum.Parse(typeof(GlobalIncrementSettings.ApplyGlobalSettings), GlobalAddinSettings.Default.GlobalApply);
		}
		public override void Save()
		{
			GlobalAddinSettings.Default.GlobalMajor = base.VersioningStyle.Major.Name;
			GlobalAddinSettings.Default.GlobalMinor = base.VersioningStyle.Minor.Name;
			GlobalAddinSettings.Default.GlobalBuild = base.VersioningStyle.Build.Name;
			GlobalAddinSettings.Default.GlobalRevision = base.VersioningStyle.Revision.Name;
			GlobalAddinSettings.Default.GlobalBuildAction = base.BuildAction.ToString();
			GlobalAddinSettings.Default.GlobalAutoUpdateAssemblyVersion = base.AutoUpdateAssemblyVersion;
			GlobalAddinSettings.Default.GlobalAutoUpdateFileVersion = base.AutoUpdateFileVersion;
			GlobalAddinSettings.Default.GlobalStartDate = base.StartDate;
			GlobalAddinSettings.Default.GlobalReplaceNonNumeric = base.ReplaceNonNumerics;
			GlobalAddinSettings.Default.GlobalUseUniversalClock = base.IsUniversalTime;
			GlobalAddinSettings.Default.GlobalIncrementBeforeBuild = base.IncrementBeforeBuild;
			GlobalAddinSettings.Default.DetectChanges = base.DetectChanges;
			GlobalAddinSettings.Default.GlobalApply = this.Apply.ToString();
			GlobalAddinSettings.Default.Save();
		}
		public override void Reset()
		{
			base.AutoUpdateAssemblyVersion = false;
			base.AutoUpdateFileVersion = false;
			base.BuildAction = BuildActionType.Both;
			string defaultGlobalVariable = VersioningStyle.GetDefaultGlobalVariable();
			base.VersioningStyle.FromGlobalVariable(defaultGlobalVariable);
			base.IsUniversalTime = false;
			base.StartDate = new DateTime(1975, 10, 21);
			base.ReplaceNonNumerics = true;
			base.IncrementBeforeBuild = true;
			base.DetectChanges = true;
			this.Apply = GlobalIncrementSettings.ApplyGlobalSettings.OnlyWhenChosen;
		}
	}
}
