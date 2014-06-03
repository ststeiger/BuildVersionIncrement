using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace BuildVersionIncrement
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0"), CompilerGenerated]
	internal sealed class GlobalAddinSettings : ApplicationSettingsBase
	{
		private static GlobalAddinSettings defaultInstance = (GlobalAddinSettings)SettingsBase.Synchronized(new GlobalAddinSettings());
		public static GlobalAddinSettings Default
		{
			get
			{
				return GlobalAddinSettings.defaultInstance;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool IsEnabled
		{
			get
			{
				return (bool)this["IsEnabled"];
			}
			set
			{
				this["IsEnabled"] = value;
			}
		}
		[DefaultSettingValue("1975-10-21"), UserScopedSetting, DebuggerNonUserCode]
		public DateTime LastVersionCheck
		{
			get
			{
				return (DateTime)this["LastVersionCheck"];
			}
			set
			{
				this["LastVersionCheck"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool GlobalAutoUpdateAssemblyVersion
		{
			get
			{
				return (bool)this["GlobalAutoUpdateAssemblyVersion"];
			}
			set
			{
				this["GlobalAutoUpdateAssemblyVersion"] = value;
			}
		}
		[DefaultSettingValue("1975-10-21"), UserScopedSetting, DebuggerNonUserCode]
		public DateTime GlobalStartDate
		{
			get
			{
				return (DateTime)this["GlobalStartDate"];
			}
			set
			{
				this["GlobalStartDate"] = value;
			}
		}
		[DefaultSettingValue("Both"), UserScopedSetting, DebuggerNonUserCode]
		public string GlobalBuildAction
		{
			get
			{
				return (string)this["GlobalBuildAction"];
			}
			set
			{
				this["GlobalBuildAction"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool GlobalAutoUpdateFileVersion
		{
			get
			{
				return (bool)this["GlobalAutoUpdateFileVersion"];
			}
			set
			{
				this["GlobalAutoUpdateFileVersion"] = value;
			}
		}
		[DefaultSettingValue("None"), UserScopedSetting, DebuggerNonUserCode]
		public string GlobalMajor
		{
			get
			{
				return (string)this["GlobalMajor"];
			}
			set
			{
				this["GlobalMajor"] = value;
			}
		}
		[DefaultSettingValue("None"), UserScopedSetting, DebuggerNonUserCode]
		public string GlobalMinor
		{
			get
			{
				return (string)this["GlobalMinor"];
			}
			set
			{
				this["GlobalMinor"] = value;
			}
		}
		[DefaultSettingValue("None"), UserScopedSetting, DebuggerNonUserCode]
		public string GlobalBuild
		{
			get
			{
				return (string)this["GlobalBuild"];
			}
			set
			{
				this["GlobalBuild"] = value;
			}
		}
		[DefaultSettingValue("None"), UserScopedSetting, DebuggerNonUserCode]
		public string GlobalRevision
		{
			get
			{
				return (string)this["GlobalRevision"];
			}
			set
			{
				this["GlobalRevision"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool GlobalReplaceNonNumeric
		{
			get
			{
				return (bool)this["GlobalReplaceNonNumeric"];
			}
			set
			{
				this["GlobalReplaceNonNumeric"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool GlobalUseUniversalClock
		{
			get
			{
				return (bool)this["GlobalUseUniversalClock"];
			}
			set
			{
				this["GlobalUseUniversalClock"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool GlobalIncrementBeforeBuild
		{
			get
			{
				return (bool)this["GlobalIncrementBeforeBuild"];
			}
			set
			{
				this["GlobalIncrementBeforeBuild"] = value;
			}
		}
		[DefaultSettingValue("OnlyWhenChosen"), UserScopedSetting, DebuggerNonUserCode]
		public string GlobalApply
		{
			get
			{
				return (string)this["GlobalApply"];
			}
			set
			{
				this["GlobalApply"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool IsVerboseLogEnabled
		{
			get
			{
				return (bool)this["IsVerboseLogEnabled"];
			}
			set
			{
				this["IsVerboseLogEnabled"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool DetectChanges
		{
			get
			{
				return (bool)this["DetectChanges"];
			}
			set
			{
				this["DetectChanges"] = value;
			}
		}
	}
}
