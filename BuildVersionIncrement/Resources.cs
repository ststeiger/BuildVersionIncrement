using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
namespace BuildVersionIncrement
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;
		private static CultureInfo resourceCulture;
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Resources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("BuildVersionIncrement.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}
		internal static string GlobalMessage_alwaysApplyGlobalSettings
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalMessage_alwaysApplyGlobalSettings", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_assemblyInfoFilename
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_assemblyInfoFilename", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_buildAction
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_buildAction", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_buildVersioningStyle
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_buildVersioningStyle", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_configurationName
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_configurationName", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_detectChanges
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_detectChanges", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_incrementBeforeBuild
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_incrementBeforeBuild", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_replaceNonNumerics
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_replaceNonNumerics", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_startDate
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_startDate", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_updateAssemblyVersion
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_updateAssemblyVersion", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_updateFileVersion
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_updateFileVersion", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_useGlobalSettings
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_useGlobalSettings", Resources.resourceCulture);
			}
		}
		internal static string GlobalVarName_useUniversalClock
		{
			get
			{
				return Resources.ResourceManager.GetString("GlobalVarName_useUniversalClock", Resources.resourceCulture);
			}
		}
		internal Resources()
		{
		}
	}
}
