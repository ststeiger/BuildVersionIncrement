using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
namespace BuildVersionIncrement
{
	internal class SolutionItem
	{
		private Connect _connect;
		private SolutionItemType _itemType;
		private object _item;
		private string _uniqueName;
		private List<SolutionItem> _subItems;
		private SolutionItemIncrementSettings _incrementSetting;
		private string _name;
		private string _filename;
		private LanguageType _projectType;
		[Browsable(false)]
		public Connect Connect
		{
			get
			{
				return this._connect;
			}
		}
		[Browsable(false)]
		public SolutionItemType ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				this._itemType = value;
			}
		}
		[Browsable(false)]
		public Solution Solution
		{
			get
			{
				return (Solution)this._item;
			}
		}
		[Browsable(false)]
		public Project Project
		{
			get
			{
				return (Project)this._item;
			}
		}
		[Browsable(false)]
		public DTE DTE
		{
			get
			{
				DTE result;
				switch (this.ItemType)
				{
				case SolutionItemType.Project:
					result = this.Project.DTE;
					break;
				case SolutionItemType.Solution:
					result = this.Solution.DTE;
					break;
				default:
					result = null;
					break;
				}
				return result;
			}
		}
		[Browsable(false)]
		public Globals Globals
		{
			get
			{
				Globals result;
				switch (this.ItemType)
				{
				case SolutionItemType.Project:
					result = this.Project.Globals;
					break;
				case SolutionItemType.Solution:
					result = this.Solution.Globals;
					break;
				default:
					result = null;
					break;
				}
				return result;
			}
		}
		[Browsable(false)]
		public string UniqueName
		{
			get
			{
				return this._uniqueName;
			}
			set
			{
				this._uniqueName = value;
			}
		}
		[Browsable(false)]
		public List<SolutionItem> SubItems
		{
			get
			{
				return this._subItems;
			}
		}
		[Browsable(false)]
		public BuildDependency BuildDependency
		{
			get
			{
				return this.DTE.Solution.SolutionBuild.BuildDependencies.Item(this.UniqueName);
			}
		}
		[Browsable(false)]
		public SolutionItemIncrementSettings IncrementSettings
		{
			get
			{
				return this._incrementSetting;
			}
		}
		public string Guid
		{
			get
			{
				string result;
				if (this.ItemType != SolutionItemType.Solution)
				{
					result = this.Project.Kind;
				}
				else
				{
					result = string.Empty;
				}
				return result;
			}
		}
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}
		public string Filename
		{
			get
			{
				return this._filename;
			}
		}
		public LanguageType ProjectType
		{
			get
			{
				return this._projectType;
			}
			set
			{
				this._projectType = value;
			}
		}
		public SolutionItem(Connect connect, Solution solution) : this(connect, solution, true)
		{
		}
		public SolutionItem(Connect connect, Solution solution, bool recursive) 
		{
			this._subItems = new List<SolutionItem>();
			this._projectType = LanguageType.None;

			if (connect == null)
			{
				throw new ArgumentNullException("connect");
			}

			if (solution == null)
			{
				throw new ArgumentNullException("solution");
			}

			this._incrementSetting = new SolutionItemIncrementSettings(this);
			this._connect = connect;
			this._item = solution;
			this._itemType = SolutionItemType.Solution;
			this._name = Path.GetFileNameWithoutExtension(solution.FileName);
			this._filename = solution.FileName;
			this._uniqueName = this._name;
			this.GetGlobalVariables();
			if (recursive)
			{
				SolutionItem.FillSolutionTree(connect, this, solution.Projects);
			}
		}
		private SolutionItem(Connect connect, Project project) : this(connect, project, true)
		{
		}
		private SolutionItem(Connect connect, Project project, bool recursive) 
		{
			this._subItems = new List<SolutionItem>();
			this._projectType = LanguageType.None;

			if (project == null)
			{
				throw new ArgumentNullException("project");
			}

			if (connect == null)
			{
				throw new ArgumentNullException("connect");
			}

			this._incrementSetting = new SolutionItemIncrementSettings(this);
			this._connect = connect;
			this._item = project;
			this._name = project.Name;
			this._filename = project.FileName;
			this._uniqueName = project.UniqueName;
			if (!string.IsNullOrEmpty(this._filename) && string.IsNullOrEmpty(Path.GetExtension(this._filename)))
			{
				this._filename += Path.GetExtension(project.UniqueName);
			}
			if (string.IsNullOrEmpty(project.FullName))
			{
				this._itemType = SolutionItemType.Folder;
				if (recursive)
				{
					SolutionItem.FillSolutionTree(connect, this, project.ProjectItems);
				}
			}
			else
			{
				this._itemType = SolutionItemType.Project;
				this.GetGlobalVariables();
			}
		}
		public static SolutionItem ConstructSolutionItem(Connect connect, Project project)
		{
			return SolutionItem.ConstructSolutionItem(connect, project, true);
		}
		public static SolutionItem ConstructSolutionItem(Connect connect, Project project, bool recursive)
		{
			SolutionItem result = null;
			if (SolutionItem.IsValidSolutionItem(project))
			{
				result = new SolutionItem(connect, project, recursive);
			}
			return result;
		}
		private static bool IsValidSolutionItem(Project p)
		{
			bool flag = false;
			bool result;
			try
			{
				if (p != null && p.Object != null && !string.IsNullOrEmpty(p.Kind) && (p.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}" || p.Kind == "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}" || p.Kind == "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}" || p.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}"))
				{
					result = true;
					return result;
				}
			}
			catch (Exception ex)
			{
				Logger.Write("Exception occured while checking project type \"" + p.UniqueName + "\".\n" + ex.ToString(), LogLevel.Error);
			}
			result = flag;
			return result;
		}
		private static void FillSolutionTree(Connect connect, SolutionItem solutionItem, Projects projects)
		{
			if (projects != null)
			{
				foreach (Project project in projects)
				{
					SolutionItem solutionItem2 = SolutionItem.ConstructSolutionItem(connect, project);
					if (solutionItem2 != null)
					{
						solutionItem.SubItems.Add(solutionItem2);
					}
				}
			}
		}
		private static void FillSolutionTree(Connect connect, SolutionItem solutionItem, ProjectItems projectItems)
		{
			if (projectItems != null)
			{
				foreach (ProjectItem projectItem in projectItems)
				{
					SolutionItem solutionItem2 = SolutionItem.ConstructSolutionItem(connect, projectItem.SubProject);
					if (solutionItem2 != null)
					{
						solutionItem.SubItems.Add(solutionItem2);
					}
				}
			}
		}
		public ProjectItem FindProjectItem(string name)
		{
			ProjectItem result = null;
			if (this.ItemType == SolutionItemType.Project)
			{
				result = SolutionItem.FindProjectItem(this.Project.ProjectItems, name);
			}
			return result;
		}
		private static ProjectItem FindProjectItem(ProjectItems projectItems, string name)
		{
			ProjectItem result;
			if (projectItems == null)
			{
				result = null;
			}
			else
			{
				foreach (ProjectItem projectItem in projectItems)
				{
					if (string.Compare(projectItem.Name, name, true) == 0)
					{
						result = projectItem;
						return result;
					}
					if (projectItem.ProjectItems != null && projectItem.ProjectItems.Count > 0)
					{
						ProjectItem projectItem2 = SolutionItem.FindProjectItem(projectItem.ProjectItems, name);
						if (projectItem2 != null)
						{
							result = projectItem2;
							return result;
						}
					}
				}
				result = null;
			}
			return result;
		}
		public void ApplyGlobalSettings()
		{
			GlobalIncrementSettings globalIncrementSettings = new GlobalIncrementSettings();
			try
			{
				globalIncrementSettings.Load();
			}
			catch (Exception innerException)
			{
				throw new ApplicationException("Exception occured while applying global settings to the solution item (" + this.UniqueName + ").", innerException);
			}
			this.IncrementSettings.CopyFrom(globalIncrementSettings);
		}
		private void GetGlobalVariables()
		{
			this.IncrementSettings.Load();
		}
		private string GetGlobalVariable(string varName, string defaultValue)
		{
			return GlobalVariables.GetGlobalVariable(this.Globals, varName, defaultValue);
		}
		public void SetGlobalVariables()
		{
			this.IncrementSettings.Save();
			foreach (SolutionItem current in this.SubItems)
			{
				current.SetGlobalVariables();
			}
		}
		public void SetGlobalVariable(string varName, string value)
		{
			GlobalVariables.SetGlobalVariable(this.Globals, varName, value);
		}
	}
}
