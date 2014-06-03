using BuildVersionIncrement.Incrementors;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
namespace BuildVersionIncrement
{
	internal class BuildVersionIncrementor
	{
		private Connect _connect;
		private DateTime _buildStartDate = DateTime.MinValue;
		private IncrementorCollection _incrementors = new IncrementorCollection();
		private static BuildVersionIncrementor _instance;
		private static Dictionary<string, DateTime> _fileDateCache = new Dictionary<string, DateTime>();
		private static Dictionary<string, bool> _solutionItemCache = new Dictionary<string, bool>();
		private vsBuildState _currentBuildState = vsBuildState.vsBuildStateInProgress;
		private vsBuildAction _currentBuildAction = vsBuildAction.vsBuildActionClean;
		private vsBuildScope _currentBuildScope = vsBuildScope.vsBuildScopeBatch;
		private Dictionary<string, SolutionItem> _updatedItems = new Dictionary<string, SolutionItem>();
		public IncrementorCollection Incrementors
		{
			get
			{
				return this._incrementors;
			}
		}
		public static BuildVersionIncrementor Instance
		{
			get
			{
				return BuildVersionIncrementor._instance;
			}
		}
		public BuildVersionIncrementor(Connect connect)
		{
			this._connect = connect;
			BuildVersionIncrementor._instance = this;
		}
		public void InitializeIncrementors()
		{
			try
			{
				this._incrementors.AddFrom(Assembly.GetExecutingAssembly());
				string[] files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.Incrementor.dll");
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					Logger.Write("Loading incrementors from \"" + text + "\".", LogLevel.Debug);
					Assembly asm = Assembly.LoadFrom(text);
					this._incrementors.AddFrom(asm);
				}
			}
			catch (Exception ex)
			{
				Logger.Write("Exception occured while initializing incrementors.\n" + ex.ToString(), LogLevel.Error);
			}
		}
		private bool PropertyExists(Properties properties, string propertyName)
		{
			bool result;
			foreach (Property property in properties)
			{
				if (property.Name == propertyName)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private static DateTime GetCachedFileDate(string outputFileName, string fullPath)
		{
			string text = Path.Combine(fullPath, outputFileName);
			DateTime dateTime;
			if (BuildVersionIncrementor._fileDateCache.ContainsKey(text))
			{
				dateTime = BuildVersionIncrementor._fileDateCache[text];
			}
			else
			{
				dateTime = File.GetLastWriteTime(text);
				BuildVersionIncrementor._fileDateCache.Add(text, dateTime);
			}
			Logger.Write(string.Format("Last Build:{1} ({0})", text, dateTime), LogLevel.Debug);
			return dateTime;
		}
		private static void ClearSolutionItemAndFileDateCache()
		{
			Logger.Write("Clearing date and solution cache", LogLevel.Debug);
			BuildVersionIncrementor._fileDateCache.Clear();
			BuildVersionIncrementor._solutionItemCache.Clear();
		}
		private bool CheckFilesystemItem(string localPath, string itemName)
		{
			FileAttributes attributes = File.GetAttributes(localPath);
			bool result;
			if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				if (!Directory.Exists(localPath))
				{
					Logger.Write(string.Format(" Directory '{0}' was not found - assuming a clean build was made", itemName), LogLevel.Debug);
					result = true;
					return result;
				}
			}
			else
			{
				if (!File.Exists(localPath))
				{
					Logger.Write(string.Format(" File '{0}' was not found - assuming a clean build was made", itemName), LogLevel.Debug);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private bool CheckProjectItem(ProjectItem item, DateTime outputFileDate)
		{
			DateTime dateTime = DateTime.MinValue;
			bool result;
			if (this.PropertyExists(item.Properties, "LocalPath"))
			{
				Property property = item.Properties.Item("LocalPath");
				string text = property.Value.ToString();
				if (this.CheckFilesystemItem(text, item.Name))
				{
					result = true;
					return result;
				}
				if (this.PropertyExists(item.Properties, "DateModified"))
				{
					Property property2 = item.Properties.Item("DateModified");
					string s = property2.Value.ToString();
					try
					{
						dateTime = DateTime.Parse(s);
					}
					catch
					{
						try
						{
							dateTime = DateTime.Parse(s, CultureInfo.InvariantCulture);
						}
						catch
						{
							Logger.Write(string.Format("Cannot parse current item's date '{0}'", dateTime), LogLevel.Warning);
						}
					}
				}
			}
			else
			{
				if (this.PropertyExists(item.Properties, "FullPath"))
				{
					Property property = item.Properties.Item("FullPath");
					string text = property.Value.ToString();
					if (this.CheckFilesystemItem(text, item.Name))
					{
						result = true;
						return result;
					}
					dateTime = File.GetLastWriteTime(text);
				}
			}
			result = (dateTime > outputFileDate);
			return result;
		}
		private bool IsProjectModified(Project project, LanguageType language)
		{
			bool result;
			try
			{
				Logger.Write(string.Format("Checking project '{0}'...", project.Name), LogLevel.Debug);
				string outputFileName;
				string text;
				try
				{
					Configuration activeConfiguration = project.ConfigurationManager.ActiveConfiguration;
					outputFileName = project.Properties.Item("OutputFileName").Value.ToString();
					text = project.Properties.Item("FullPath").Value.ToString();
					text = Path.Combine(text, activeConfiguration.Properties.Item("OutputPath").Value.ToString());
				}
				catch
				{
					try
					{
						object @object = project.Properties.Item("project").Object;
						object obj = @object.GetType().InvokeMember("Configurations", BindingFlags.GetProperty, null, @object, null);
						object obj2 = obj.GetType().InvokeMember("Item", BindingFlags.InvokeMethod, null, obj, new object[]
						{
							1
						});
						string path = "";
						if (obj2 != null)
						{
							path = (string)obj2.GetType().InvokeMember("PrimaryOutput", BindingFlags.GetProperty, null, obj2, null);
						}
						outputFileName = Path.GetFileName(path);
						text = Path.GetDirectoryName(path);
						if (!text.EndsWith(Path.DirectorySeparatorChar.ToString()))
						{
							text += Path.DirectorySeparatorChar;
						}
					}
					catch (Exception ex)
					{
						Logger.Write(string.Format("Could not get project output file date: {0}. Assumming file is modified.", ex.Message), LogLevel.Warning);
						result = true;
						return result;
					}
				}
				DateTime cachedFileDate = BuildVersionIncrementor.GetCachedFileDate(outputFileName, text);
				foreach (ProjectItem projectItem in project.ProjectItems)
				{
					if (projectItem.Kind == "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}" || projectItem.Kind == "{6BB5F8F0-4483-11D3-8BCF-00C04F8EC28C}")
					{
						foreach (ProjectItem item in projectItem.ProjectItems)
						{
							if (this.CheckProjectItem(item, cachedFileDate))
							{
								Logger.Write(string.Format("Project's ('{0}') item '{1}' is modified. Version will be updated.", project.Name, projectItem.Name), LogLevel.Debug);
								result = true;
								return result;
							}
						}
					}
					else
					{
						if (this.CheckProjectItem(projectItem, cachedFileDate))
						{
							Logger.Write(string.Format("Project's ('{0}') item '{1}' is modified. Version will be updated.", project.Name, projectItem.Name), LogLevel.Debug);
							result = true;
							return result;
						}
					}
				}
				Logger.Write(string.Format("Project '{0}' is not modified", project.Name), LogLevel.Debug);
				result = false;
			}
			catch (Exception ex)
			{
				Logger.Write(string.Format("Could not check if project were modified because: {0}. Assumming file is modified.", ex.Message), LogLevel.Warning);
				Logger.Write(ex.ToString(), LogLevel.Debug);
				result = true;
			}
			return result;
		}
		private bool IsSolutionItemModified(SolutionItem solutionItem)
		{
			string key = string.Format("{0}:{1}", solutionItem.ItemType, solutionItem.Name);
			bool result;
			if (BuildVersionIncrementor._solutionItemCache.ContainsKey(key))
			{
				bool flag = BuildVersionIncrementor._solutionItemCache[key];
				result = flag;
			}
			else
			{
				if (!solutionItem.IncrementSettings.DetectChanges)
				{
					Logger.Write(string.Format("Detect changes disabled. Mark item '{0}' as modified.", solutionItem.Name), LogLevel.Debug);
					BuildVersionIncrementor._solutionItemCache.Add(key, true);
					result = true;
				}
				else
				{
					this.PrepareSolutionItem(solutionItem);
					switch (solutionItem.ItemType)
					{
					case SolutionItemType.Folder:
					case SolutionItemType.Solution:
					{
						bool flag = false;
						foreach (SolutionItem current in solutionItem.SubItems)
						{
							if (current.ItemType == SolutionItemType.Project)
							{
								flag = this.IsProjectModified(current.Project, current.ProjectType);
								BuildVersionIncrementor._solutionItemCache.Add(string.Format("{0}:{1}", current.ItemType, current.Name), flag);
							}
							else
							{
								if (current.ItemType == SolutionItemType.Folder)
								{
									flag = this.IsSolutionItemModified(current);
								}
							}
							if (flag)
							{
								break;
							}
						}
						Logger.Write(string.Format("Solution/Folder '{0}' is not modified", solutionItem.Name), LogLevel.Debug);
						BuildVersionIncrementor._solutionItemCache.Add(key, flag);
						result = flag;
						break;
					}
					case SolutionItemType.Project:
					{
						bool flag = this.IsProjectModified(solutionItem.Project, solutionItem.ProjectType);
						BuildVersionIncrementor._solutionItemCache.Add(string.Format("{0}:{1}", solutionItem.ItemType, solutionItem.Name), flag);
						result = flag;
						break;
					}
					default:
						Logger.Write(string.Format("Solution item '{0}' is not supported. Run standard behavior (is modified).", solutionItem.ItemType), LogLevel.Warning);
						BuildVersionIncrementor._solutionItemCache.Add(key, true);
						result = true;
						break;
					}
				}
			}
			return result;
		}
		private void ExecuteIncrement()
		{
			if (!GlobalAddinSettings.Default.IsEnabled)
			{
				Logger.Write("BuildVersionIncrement disabled.", LogLevel.Info);
			}
			else
			{
				try
				{
					if (this._currentBuildAction == vsBuildAction.vsBuildActionBuild || this._currentBuildAction == vsBuildAction.vsBuildActionRebuildAll)
					{
						if (this._currentBuildScope == vsBuildScope.vsBuildScopeSolution)
						{
							Solution solution = this._connect.ApplicationObject.Solution;
							SolutionItem solutionItem = new SolutionItem(this._connect, solution, true);
							this.UpdateRecursive(solutionItem);
						}
						else
						{
							Array array = (Array)this._connect.ApplicationObject.ActiveSolutionProjects;
							foreach (Project project in array)
							{
								SolutionItem solutionItem = SolutionItem.ConstructSolutionItem(this._connect, project, false);
								if (solutionItem != null && this.IsSolutionItemModified(solutionItem))
								{
									this.UpdateProject(solutionItem);
								}
							}
						}
						Logger.Write(string.Format("{0}-build process : Completed", (this._currentBuildState == vsBuildState.vsBuildStateInProgress) ? "Pre" : "Post"), LogLevel.Info);
					}
				}
				catch (Exception ex)
				{
					Logger.Write("Error occured while executing build version increment.\n" + ex.ToString(), LogLevel.Error);
				}
			}
		}
		private void UpdateRecursive(SolutionItem solutionItem)
		{
			try
			{
				if (!this.IsSolutionItemModified(solutionItem))
				{
					return;
				}
				if (solutionItem.IncrementSettings.UseGlobalSettings)
				{
					solutionItem.ApplyGlobalSettings();
				}
				if (this.ActiveConfigurationMatch(solutionItem))
				{
					if (solutionItem.IncrementSettings.AutoUpdateAssemblyVersion)
					{
						this.Update(solutionItem, "AssemblyVersion");
					}
					if (solutionItem.IncrementSettings.AutoUpdateFileVersion)
					{
						this.Update(solutionItem, "AssemblyFileVersion");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Write(ex.ToString(), LogLevel.Error);
			}
			foreach (SolutionItem current in solutionItem.SubItems)
			{
				this.UpdateRecursive(current);
			}
		}
		private bool ActiveConfigurationMatch(SolutionItem solutionItem)
		{
			bool result;
			try
			{
				if (solutionItem.ItemType == SolutionItemType.Folder)
				{
					result = false;
					return result;
				}
				string b;
				if (solutionItem.ItemType == SolutionItemType.Solution)
				{
					b = solutionItem.Solution.SolutionBuild.ActiveConfiguration.Name;
				}
				else
				{
					b = solutionItem.Project.ConfigurationManager.ActiveConfiguration.ConfigurationName;
				}
				if (solutionItem.IncrementSettings.ConfigurationName == "Any" || solutionItem.IncrementSettings.ConfigurationName == b)
				{
					result = true;
					return result;
				}
			}
			catch (Exception ex)
			{
				if (!solutionItem.UniqueName.EndsWith("contentproj"))
				{
					Logger.Write(string.Concat(new string[]
					{
						"Couldn't get the active configuration name for \"",
						solutionItem.UniqueName,
						"\": \"",
						ex.Message,
						"\"\nSkipping ..."
					}), LogLevel.Info);
				}
			}
			result = false;
			return result;
		}
		private void UpdateProject(SolutionItem solutionItem)
		{
			if (GlobalIncrementSettings.ApplySettings == GlobalIncrementSettings.ApplyGlobalSettings.Always || solutionItem.IncrementSettings.UseGlobalSettings)
			{
				solutionItem.ApplyGlobalSettings();
			}
			if (!this._updatedItems.ContainsKey(solutionItem.UniqueName))
			{
				if (this.ActiveConfigurationMatch(solutionItem))
				{
					if (solutionItem.IncrementSettings.AutoUpdateAssemblyVersion)
					{
						this.Update(solutionItem, "AssemblyVersion");
					}
					if (solutionItem.IncrementSettings.AutoUpdateFileVersion)
					{
						this.Update(solutionItem, "AssemblyFileVersion");
					}
				}
				try
				{
					if (solutionItem.BuildDependency != null)
					{
						object[] array = (object[])solutionItem.BuildDependency.RequiredProjects;
						object[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							object obj = array2[i];
							SolutionItem solutionItem2 = SolutionItem.ConstructSolutionItem(this._connect, (Project)obj, false);
							if (solutionItem2 != null)
							{
								try
								{
									this.UpdateProject(solutionItem2);
								}
								catch (Exception ex)
								{
									Logger.Write(string.Concat(new string[]
									{
										"Exception occured while updating project dependency \"",
										solutionItem2.UniqueName,
										"\" for \"",
										solutionItem.UniqueName,
										"\".\n",
										ex.Message
									}), LogLevel.Error);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Write("Failed updating dependencies for \"" + solutionItem.UniqueName + "\".\n" + ex.Message, LogLevel.Error);
				}
				this._updatedItems.Add(solutionItem.UniqueName, solutionItem);
			}
		}
		private void Update(SolutionItem solutionItem, string attribute)
		{
			if (solutionItem.IncrementSettings.BuildAction == BuildActionType.Both || (solutionItem.IncrementSettings.BuildAction == BuildActionType.Build && this._currentBuildAction == vsBuildAction.vsBuildActionBuild) || (solutionItem.IncrementSettings.BuildAction == BuildActionType.ReBuild && this._currentBuildAction == vsBuildAction.vsBuildActionRebuildAll))
			{
				if (solutionItem.IncrementSettings.IncrementBeforeBuild == (this._currentBuildState == vsBuildState.vsBuildStateInProgress))
				{
					Logger.Write("Updating attribute " + attribute + " of project " + solutionItem.Name, LogLevel.Debug);
					string assemblyInfoFilename = this.GetAssemblyInfoFilename(solutionItem);
					if (assemblyInfoFilename != null && File.Exists(assemblyInfoFilename))
					{
						switch (solutionItem.ProjectType)
						{
						case LanguageType.CSharp:
						case LanguageType.VisualBasic:
						case LanguageType.CPPManaged:
							this.UpdateVersion(solutionItem, "^[\\[<]assembly:\\s*" + attribute + "(Attribute)?\\s*\\(\\s*\"(?<FullVersion>\\S+\\.\\S+(\\.(?<Version>[^\"]+))?)\"\\s*\\)[\\]>]", assemblyInfoFilename, attribute);
							break;
						case LanguageType.CPPUnmanaged:
							if (attribute == "AssemblyVersion")
							{
								attribute = "ProductVersion";
							}
							if (attribute == "AssemblyFileVersion")
							{
								attribute = "FileVersion";
							}
							this.UpdateVersion(solutionItem, "^[\\s]*VALUE\\ \"" + attribute + "\",\\ \"(?<FullVersion>\\S+[.,\\s]+\\S+[.,\\s]+\\S+[.,\\s]+[^\\s\"]+)\"", assemblyInfoFilename, attribute);
							this.UpdateVersion(solutionItem, "^[\\s]*" + attribute.ToUpper() + "\\ (?<FullVersion>\\S+[.,]+\\S+[.,]+\\S+[.,]+\\S+)", assemblyInfoFilename, attribute.ToUpper());
							break;
						}
					}
				}
			}
		}
		private void UpdateVersion(SolutionItem solutionItem, string regexPattern, string assemblyFile, string debugAttribute)
		{
			string text = File.ReadAllText(assemblyFile);
			try
			{
				RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace;
				Match match = Regex.Match(text, regexPattern, options);
				if (!match.Success)
				{
					Logger.Write(string.Concat(new string[]
					{
						"Failed to locate attribute \"",
						debugAttribute,
						"\" in file \"",
						assemblyFile,
						"\"."
					}), LogLevel.Error, assemblyFile, 1);
				}
				else
				{
					Match match2 = Regex.Match(match.Groups["FullVersion"].Value, "(?<Separator>[\\s,.]+)", options);
					if (!match2.Success)
					{
						Logger.Write(string.Concat(new string[]
						{
							"Failed to fetch version separator on attribute \"",
							debugAttribute,
							"\" in file \"",
							assemblyFile,
							"\"."
						}), LogLevel.Error, assemblyFile, 2);
					}
					else
					{
						StringVersion stringVersion = null;
						try
						{
							stringVersion = new StringVersion(Regex.Replace(match.Groups["FullVersion"].Value, "[^\\d" + match2.Groups["Separator"].Value + "]+", "0").Replace(match2.Groups["Separator"].Value, "."));
						}
						catch (Exception ex)
						{
							string text2 = string.Format("Error occured while parsing value of {0} ({1}).\n{2}", debugAttribute, match.Groups["FullVersion"].Value, ex);
							throw new Exception(text2, ex);
						}
						StringVersion stringVersion2 = solutionItem.IncrementSettings.VersioningStyle.Increment(stringVersion, solutionItem.IncrementSettings.IsUniversalTime ? this._buildStartDate.ToUniversalTime() : this._buildStartDate, solutionItem.IncrementSettings.StartDate, solutionItem);
						if (stringVersion2 != stringVersion)
						{
							bool flag = false;
							if (this._connect.IsCommandLineBuild)
							{
								text = text.Remove(match.Groups["FullVersion"].Index, match.Groups["FullVersion"].Length);
								text = text.Insert(match.Groups["FullVersion"].Index, stringVersion2.ToString());
								try
								{
									File.WriteAllText(assemblyFile, text);
									flag = true;
								}
								catch (Exception ex)
								{
									Logger.Write(ex.Message, LogLevel.Warning);
									flag = false;
								}
							}
							else
							{
								bool flag2 = !solutionItem.DTE.ItemOperations.IsFileOpen(assemblyFile, null);
								string replaceText = string.Empty;
								if (!solutionItem.IncrementSettings.ReplaceNonNumerics && Regex.IsMatch(match.Groups["FullVersion"].Value, "[^\\d" + match2.Groups["Separator"].Value + "]+"))
								{
									string[] array = match.Groups["FullVersion"].Value.Replace(match2.Groups["Separator"].Value, ".").Split(new char[]
									{
										'.'
									});
									if (Regex.IsMatch(array[0], "[\\d]+"))
									{
										array[0] = stringVersion2.Major;
									}
									if (Regex.IsMatch(array[1], "[\\d]+"))
									{
										array[1] = stringVersion2.Minor;
									}
									if (Regex.IsMatch(array[2], "[\\d]+"))
									{
										array[2] = stringVersion2.Build;
									}
									if (Regex.IsMatch(array[3], "[\\d]+"))
									{
										array[3] = stringVersion2.Revision;
									}
									replaceText = match.Value.Replace(match.Groups["FullVersion"].Value, string.Format("{0}.{1}.{2}.{3}", array).Replace(".", match2.Groups["Separator"].Value));
								}
								else
								{
									replaceText = match.Value.Replace(match.Groups["FullVersion"].Value, stringVersion2.ToString(4).Replace(".", match2.Groups["Separator"].Value));
								}
								ProjectItem projectItem = this._connect.ApplicationObject.Solution.FindProjectItem(assemblyFile);
								if (projectItem == null)
								{
									throw new ApplicationException("Failed to find project item \"" + assemblyFile + "\".");
								}
								Window window = projectItem.Open("{7651A703-06E5-11D1-8EBD-00A0C90F26EA}");
								if (window == null)
								{
									throw new ApplicationException("Could not open project item.");
								}
								Document document = window.Document;
								if (document == null)
								{
									throw new ApplicationException("Located project item & window but no document.");
								}
								flag = document.ReplaceText(match.Value, replaceText, 0);
								if (flag2)
								{
									window.Close(vsSaveChanges.vsSaveChangesYes);
								}
								else
								{
									document.Save(assemblyFile);
								}
							}
							string text2 = string.Concat(new object[]
							{
								solutionItem.Name,
								" ",
								debugAttribute,
								": ",
								stringVersion2
							});
							if (flag)
							{
								text2 += " [SUCCESS]";
							}
							else
							{
								text2 += " [FAILED]";
							}
							Logger.Write(text2, LogLevel.Info);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Write("Error occured while updating version.\n" + ex.ToString(), LogLevel.Error, assemblyFile, 1);
			}
		}
		private LanguageType GetLanguageType(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			LanguageType result;
			if (extension != null)
			{
				if (extension == ".cs")
				{
					result = LanguageType.CSharp;
					return result;
				}
				if (extension == ".vb")
				{
					result = LanguageType.VisualBasic;
					return result;
				}
				if (extension == ".cpp")
				{
					result = LanguageType.CPPManaged;
					return result;
				}
			}
			result = LanguageType.None;
			return result;
		}
		private void PrepareSolutionItem(SolutionItem solutionItem)
		{
			if (solutionItem.ProjectType == LanguageType.None)
			{
				string extension = Path.GetExtension(solutionItem.Filename);
				string text = extension;
				if (text != null)
				{
					if (!(text == ".vbproj"))
					{
						if (!(text == ".vcproj") && !(text == ".vcxproj"))
						{
							if (text == ".csproj")
							{
								solutionItem.ProjectType = LanguageType.CSharp;
							}
						}
						else
						{
							solutionItem.ProjectType = LanguageType.CPPManaged;
						}
					}
					else
					{
						solutionItem.ProjectType = LanguageType.VisualBasic;
					}
				}
				ProjectItem projectItem = solutionItem.FindProjectItem("AssemblyInfo.cpp");
				if (projectItem == null)
				{
					if (extension == ".vcproj" || extension == ".vcxproj")
					{
						solutionItem.ProjectType = LanguageType.CPPUnmanaged;
					}
				}
			}
		}
		private string GetAssemblyInfoFilename(SolutionItem solutionItem)
		{
			string text = "AssemblyInfo";
			string extension = Path.GetExtension(solutionItem.Filename);
			solutionItem.ProjectType = LanguageType.None;
			string text2 = extension;
			string result;
			if (text2 != null)
			{
				if (!(text2 == ".vbproj"))
				{
					if (!(text2 == ".vcproj") && !(text2 == ".vcxproj"))
					{
						if (!(text2 == ".csproj"))
						{
							if (!(text2 == ".sln"))
							{
								goto IL_12F;
							}
							if (string.IsNullOrEmpty(solutionItem.IncrementSettings.AssemblyInfoFilename))
							{
								Logger.Write("Can't update build version for a solution without specifying an assembly info file.", LogLevel.Error, solutionItem.Filename, 1);
								result = null;
								return result;
							}
							solutionItem.ProjectType = this.GetLanguageType(solutionItem.IncrementSettings.AssemblyInfoFilename);
							if (solutionItem.ProjectType == LanguageType.None)
							{
								Logger.Write("Can't infer solution's assembly info file language. Please add extension to filename.", LogLevel.Error, solutionItem.Filename, 1);
							}
						}
						else
						{
							text += ".cs";
							solutionItem.ProjectType = LanguageType.CSharp;
						}
					}
					else
					{
						text += ".cpp";
						solutionItem.ProjectType = LanguageType.CPPManaged;
					}
				}
				else
				{
					text += ".vb";
					solutionItem.ProjectType = LanguageType.VisualBasic;
				}
				if (!string.IsNullOrEmpty(solutionItem.IncrementSettings.AssemblyInfoFilename))
				{
					string directoryName = Path.GetDirectoryName(solutionItem.Filename);
					result = Common.MakeAbsolutePath(directoryName, solutionItem.IncrementSettings.AssemblyInfoFilename);
					return result;
				}
				ProjectItem projectItem = solutionItem.FindProjectItem(text);
				if (projectItem == null)
				{
					if (extension == ".vcproj" || extension == ".vcxproj")
					{
						text = solutionItem.Name + ".rc";
						projectItem = solutionItem.FindProjectItem(text);
						solutionItem.ProjectType = LanguageType.CPPUnmanaged;
					}
					if (projectItem == null)
					{
						Logger.Write("Could not locate \"" + text + "\" in project.", LogLevel.Warning);
						result = null;
						return result;
					}
				}
				string text3 = projectItem.get_FileNames(0);
				if (string.IsNullOrEmpty(text3))
				{
					Logger.Write("Located \"" + text + "\" project item but failed to get filename.", LogLevel.Error);
					result = null;
					return result;
				}
				Logger.Write("Found \"" + text3 + "\"", LogLevel.Debug);
				result = text3;
				return result;
			}
			IL_12F:
			Logger.Write("Unknown project file type: \"" + extension + "\"", LogLevel.Error, solutionItem.Filename, 1);
			result = null;
			return result;
		}
		public void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
		{
			Logger.Write("BuildEvents_OnBuildBegin scope: " + scope.ToString() + " action " + action.ToString(), LogLevel.Debug);
			this._currentBuildState = vsBuildState.vsBuildStateInProgress;
			this._currentBuildAction = action;
			this._currentBuildScope = scope;
			this._buildStartDate = DateTime.Now;
			this.ExecuteIncrement();
			this._updatedItems.Clear();
		}
		public void OnBuildDone(vsBuildScope scope, vsBuildAction action)
		{
			Logger.Write("BuildEvents_OnBuildDone scope: " + scope.ToString() + " action " + action.ToString(), LogLevel.Debug);
			this._currentBuildState = vsBuildState.vsBuildStateDone;
			this.ExecuteIncrement();
			this._updatedItems.Clear();
			BuildVersionIncrementor.ClearSolutionItemAndFileDateCache();
		}
		public void OnBuildProjConfigBegin(string projectName, string projectConfig, string platform, string solutionConfig)
		{
			try
			{
				Project project = this._connect.ApplicationObject.Solution.Projects.Item(projectName);
				Logger.Write(this.DumpProperties(project.Properties), LogLevel.Debug);
			}
			catch (Exception ex)
			{
				Logger.Write("Error occured while updating build version of project " + projectName + "\n" + ex.ToString(), LogLevel.Error);
			}
		}
		private string DumpProperties(Properties props)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Property property in props)
			{
				try
				{
					stringBuilder.Append(string.Format("Name: \"{0}\" Value: \"{1}\"\r\n", property.Name, property.Value));
				}
				catch
				{
					stringBuilder.Append(string.Format("Name: \"{0}\" Value: \"(UNKNOWN)\"\r\n", property.Name));
				}
			}
			return stringBuilder.ToString();
		}
	}
}
