using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;
namespace BuildVersionIncrement
{
	internal class SolutionItemIncrementSettings : BaseIncrementSettings
	{
		private SolutionItem _solutionItem;
		private string _assemblyInfoFilename = string.Empty;
		private bool _useGlobalSettings;
		private string _configurationName = "Any";
		[Browsable(false)]
		public SolutionItem SolutionItem
		{
			get
			{
				return this._solutionItem;
			}
		}
		[Category("Project"), Description("The project file."), DisplayName("Filename"), ReadOnly(true)]
		public string Filename
		{
			get
			{
				return this.SolutionItem.Filename;
			}
		}
		[Category("Project"), DisplayName("Project Kind"), ReadOnly(true)]
		public string Guid
		{
			get
			{
				return this.SolutionItem.Guid;
			}
		}
		[Category("Project"), Description("The name of the project."), DisplayName("Project Name"), ReadOnly(true)]
		public string Name
		{
			get
			{
				return this.SolutionItem.Name;
			}
		}

        [Category("Increment Settings")]
        [Description("Use this value if the assembly attributes aren't saved in the default file. " +
                     "You can use this at solution level if you make use of file links in your projects.")]
        [DefaultValue("")]
        [DisplayName("Assembly Info Filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
		public string AssemblyInfoFilename
		{
			get
			{
				return this._assemblyInfoFilename;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					string directoryName = Path.GetDirectoryName(this.SolutionItem.Filename);
					this._assemblyInfoFilename = Common.MakeRelativePath(directoryName, value);
				}
				else
				{
					this._assemblyInfoFilename = string.Empty;
				}
			}
		}
		[Category("Increment Settings"), DefaultValue(false), Description("If the project should use the global settings instead of it's own."), DisplayName("Use Global Settings")]
		public bool UseGlobalSettings
		{
			get
			{
				return this._useGlobalSettings;
			}
			set
			{
				this._useGlobalSettings = value;
			}
		}
		[Category("Condition"), DefaultValue("Any"), Description("Set this to the name to of the configuration when the auto update should occur."), DisplayName("Configuration Name"), TypeConverter(typeof(ConfigurationStringConverter))]
		public string ConfigurationName
		{
			get
			{
				return this._configurationName;
			}
			set
			{
				this._configurationName = value;
			}
		}
		public override void Load()
		{
			try
			{
				string globalVariable = GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_buildVersioningStyle, VersioningStyle.GetDefaultGlobalVariable());
				base.VersioningStyle.FromGlobalVariable(globalVariable);
				base.AutoUpdateAssemblyVersion = bool.Parse(GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_updateAssemblyVersion, "false"));
				base.AutoUpdateFileVersion = bool.Parse(GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_updateFileVersion, "false"));
				try
				{
					base.BuildAction = (BuildActionType)Enum.Parse(typeof(BuildActionType), GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_buildAction, "Both"));
				}
				catch (ArgumentException)
				{
					base.BuildAction = BuildActionType.Both;
				}
				base.StartDate = DateTime.Parse(GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_startDate, "1975/10/21"));
				base.ReplaceNonNumerics = bool.Parse(GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_replaceNonNumerics, "true"));
				base.IncrementBeforeBuild = bool.Parse(GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_incrementBeforeBuild, "true"));
				this.AssemblyInfoFilename = GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_assemblyInfoFilename, "");
				this.ConfigurationName = GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_configurationName, "Any");
				this.UseGlobalSettings = bool.Parse(GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_useGlobalSettings, (GlobalIncrementSettings.ApplySettings == GlobalIncrementSettings.ApplyGlobalSettings.AsDefault).ToString()));
				base.IsUniversalTime = bool.Parse(GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_useUniversalClock, "false"));
				base.DetectChanges = bool.Parse(GlobalVariables.GetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_detectChanges, "true"));
			}
			catch (Exception ex)
			{
				Logger.Write("Error occured while reading BuildVersionIncrement settings from \"" + this.SolutionItem.Filename + "\"\n" + ex.ToString(), LogLevel.Error);
			}
		}
		public override void Save()
		{
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_buildVersioningStyle, base.VersioningStyle.ToGlobalVariable(), VersioningStyle.GetDefaultGlobalVariable());
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_updateAssemblyVersion, base.AutoUpdateAssemblyVersion.ToString(), "false");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_updateFileVersion, base.AutoUpdateFileVersion.ToString(), "false");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_buildAction, base.BuildAction.ToString(), "Both");
			string value = string.Format("{0}/{1}/{2}", base.StartDate.Year, base.StartDate.Month, base.StartDate.Day);
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_startDate, value, "1975/10/21");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_replaceNonNumerics, base.ReplaceNonNumerics.ToString(), "true");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_incrementBeforeBuild, base.IncrementBeforeBuild.ToString(), "true");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_assemblyInfoFilename, this.AssemblyInfoFilename, "");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_configurationName, this.ConfigurationName, "Any");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_useGlobalSettings, this.UseGlobalSettings.ToString(), "false");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_useUniversalClock, base.IsUniversalTime.ToString(), "false");
			GlobalVariables.SetGlobalVariable(this.SolutionItem.Globals, Resources.GlobalVarName_detectChanges, base.DetectChanges.ToString(), "true");
		}
		public override void Reset()
		{
			string defaultGlobalVariable = VersioningStyle.GetDefaultGlobalVariable();
			base.VersioningStyle.FromGlobalVariable(defaultGlobalVariable);
			base.AutoUpdateAssemblyVersion = false;
			base.AutoUpdateFileVersion = false;
			base.BuildAction = BuildActionType.Both;
			base.StartDate = new DateTime(1975, 10, 21);
			base.ReplaceNonNumerics = true;
			base.IncrementBeforeBuild = true;
			this.AssemblyInfoFilename = string.Empty;
			this.ConfigurationName = "Any";
			this.UseGlobalSettings = false;
			base.IsUniversalTime = false;
			base.DetectChanges = true;
		}
		public override void CopyFrom(BaseIncrementSettings source)
		{
			base.CopyFrom(source);
			if (source.GetType().IsAssignableFrom(typeof(SolutionItemIncrementSettings)))
			{
				SolutionItemIncrementSettings solutionItemIncrementSettings = (SolutionItemIncrementSettings)source;
				this.AssemblyInfoFilename = solutionItemIncrementSettings.AssemblyInfoFilename;
				this.ConfigurationName = solutionItemIncrementSettings.ConfigurationName;
				this.UseGlobalSettings = solutionItemIncrementSettings.UseGlobalSettings;
			}
		}
		public SolutionItemIncrementSettings(SolutionItem solutionItem)
		{
			this._solutionItem = solutionItem;
		}
	}
}
