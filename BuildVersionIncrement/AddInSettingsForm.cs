using EnvDTE;
using Etier.IconHelper;
using Qreed.Reflection;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
namespace BuildVersionIncrement
{
	internal class AddInSettingsForm : Form
	{
		private class TreeNodeSort : IComparer
		{
			public int Compare(object x, object y)
			{
				SolutionItem solutionItem = (SolutionItem)((TreeNode)x).Tag;
				SolutionItem solutionItem2 = (SolutionItem)((TreeNode)y).Tag;
				int result;
				if (solutionItem != null && solutionItem2 != null)
				{
					if (solutionItem.ItemType == SolutionItemType.Folder)
					{
						if (solutionItem2.ItemType == SolutionItemType.Folder)
						{
							result = string.Compare(solutionItem.Name, solutionItem2.Name);
							return result;
						}
						result = -1;
						return result;
					}
					else
					{
						if (solutionItem2.ItemType == SolutionItemType.Folder)
						{
							result = 1;
							return result;
						}
					}
				}
				else
				{
					if (solutionItem2 == null)
					{
						result = 1;
						return result;
					}
					if (solutionItem2 == null)
					{
						result = -1;
						return result;
					}
				}
				result = string.Compare(solutionItem.Name, solutionItem2.Name);
				return result;
			}
		}
		private SolutionItem _solution;
		private GlobalIncrementSettings _globalSettings;
		private IconListManager _iconListManager;
		private Connect _connect;
		private IContainer components = null;
		private TreeView solutionTreeView;
		private SplitContainer splitContainer1;
		private Button buttonCancel;
		private Button buttonOk;
		private PropertyGrid propertyGrid;
		private ImageList imageList;
		private CheckBox chkVersionIncrementEnabled;
		private ContextMenuStrip contextMenu;
		private ToolStripMenuItem copyToAllProjectsToolStripMenuItem;
		private ToolStripMenuItem copyToGlobalSettingsToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem clearToolStripMenuItem;
		private ToolStripMenuItem undoToolStripMenuItem;
		private TabControl tabControl1;
		private TabPage tabPageSolution;
		private TabPage tabPageGlobalSettings;
		private PropertyGrid propertyGridGlobalSettings;
		private TabPage tabPageLog;
		private TextBox textBoxLog;
		private CheckBox chkVerboseLogEnabled;
		public Connect Connect
		{
			get
			{
				return this._connect;
			}
			set
			{
				this._connect = value;
			}
		}
		public BaseIncrementSettings SelectedIncrementSettings
		{
			get
			{
				BaseIncrementSettings result = null;
				if (this.solutionTreeView.SelectedNode != null)
				{
					if (this.solutionTreeView.SelectedNode.Tag is GlobalIncrementSettings)
					{
						result = (this.solutionTreeView.SelectedNode.Tag as BaseIncrementSettings);
					}
					else
					{
						if (this.solutionTreeView.SelectedNode.Tag is SolutionItem)
						{
							SolutionItem solutionItem = this.solutionTreeView.SelectedNode.Tag as SolutionItem;
							if (solutionItem.ItemType == SolutionItemType.Project || solutionItem.ItemType == SolutionItemType.Solution)
							{
								result = solutionItem.IncrementSettings;
							}
						}
					}
				}
				return result;
			}
		}
		public AddInSettingsForm()
		{
			this.InitializeComponent();
			Logger.WriteEvent += new EventHandler<Logger.WriteEventArgs>(this.Logger_WriteEvent);
			this._iconListManager = new IconListManager(this.imageList, IconSize.Small);
			this._globalSettings = new GlobalIncrementSettings();
			this.propertyGrid.PropertySort = PropertySort.Categorized;
			this.propertyGridGlobalSettings.PropertySort = PropertySort.Categorized;
			try
			{
				string version = ReflectionHelper.GetAssemblyAttribute<AssemblyFileVersionAttribute>(Assembly.GetExecutingAssembly()).Version;
				string configuration = ReflectionHelper.GetAssemblyAttribute<AssemblyConfigurationAttribute>(Assembly.GetExecutingAssembly()).Configuration;
				this.Text = string.Format("{0} v{1} [{2}]", this.Text, version, configuration);
			}
			catch
			{
			}
		}
		protected override void OnLoad(EventArgs e)
		{
			Window mainWindow = this.Connect.ApplicationObject.MainWindow;
			int x = mainWindow.Left + Convert.ToInt32(mainWindow.Width / 2) - Convert.ToInt32(base.Width / 2);
			int y = mainWindow.Top + Convert.ToInt32(mainWindow.Height / 2) - Convert.ToInt32(base.Height / 2);
			base.Location = new Point(x, y);
			try
			{
				this._globalSettings.Load();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error occured while loading the global settings:\n" + ex.ToString(), "Global Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				this._globalSettings = new GlobalIncrementSettings();
			}
			this.BuildTree();
			this.propertyGridGlobalSettings.SelectedObject = this._globalSettings;
			this.textBoxLog.AppendText(Logger.Instance.Contents);
			base.OnLoad(e);
		}
		private void Logger_WriteEvent(object sender, Logger.WriteEventArgs e)
		{
			this.textBoxLog.AppendText(e.Message);
		}
		private void BuildTree()
		{
			try
			{
				this._solution = new SolutionItem(this.Connect, this.Connect.ApplicationObject.Solution);
				TreeNode treeNode = this.solutionTreeView.Nodes.Add(this._solution.Name);
				treeNode.SelectedImageIndex = (treeNode.ImageIndex = this._iconListManager.AddFileIcon(this._solution.Filename));
				treeNode.Tag = this._solution;
				this.BuildTree(treeNode, this._solution);
				treeNode.Expand();
				this.solutionTreeView.SelectedNode = treeNode;
				this.solutionTreeView.TreeViewNodeSorter = new AddInSettingsForm.TreeNodeSort();
				this.solutionTreeView.Sort();
				this.solutionTreeView.AfterCollapse += new TreeViewEventHandler(this.solutionTreeView_AfterCollapse);
				this.solutionTreeView.AfterExpand += new TreeViewEventHandler(this.solutionTreeView_AfterExpand);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error occored while building solution tree.\n" + ex.ToString(), "Error");
			}
		}
		private void solutionTreeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
			e.Node.ImageIndex = this._iconListManager.AddFolderIcon(FolderType.Open);
		}
		private void solutionTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			e.Node.ImageIndex = this._iconListManager.AddFolderIcon(FolderType.Closed);
		}
		private void BuildTree(TreeNode currentNode, SolutionItem currentItem)
		{
			foreach (SolutionItem current in currentItem.SubItems)
			{
				TreeNode treeNode = currentNode.Nodes.Add(current.Name);
				if (current.ItemType == SolutionItemType.Folder)
				{
					treeNode.ImageIndex = this._iconListManager.AddFolderIcon(FolderType.Closed);
					treeNode.SelectedImageIndex = this._iconListManager.AddFolderIcon(FolderType.Open);
				}
				else
				{
					treeNode.SelectedImageIndex = (treeNode.ImageIndex = this._iconListManager.AddFileIcon(current.Filename));
				}
				treeNode.Tag = current;
				this.BuildTree(treeNode, current);
			}
			if (currentItem.ItemType == SolutionItemType.Folder && currentNode.Nodes.Count == 0)
			{
				this.solutionTreeView.Nodes.Remove(currentNode);
			}
		}
		private void solutionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Name == "Global Settings")
			{
				this.propertyGrid.Enabled = true;
				this.propertyGrid.SelectedObject = e.Node.Tag;
			}
			else
			{
				SolutionItem solutionItem = (SolutionItem)e.Node.Tag;
				if ((solutionItem.ItemType == SolutionItemType.Project || solutionItem.ItemType == SolutionItemType.Solution) && solutionItem.Globals != null)
				{
					this.propertyGrid.Enabled = true;
					this.propertyGrid.SelectedObject = solutionItem.IncrementSettings;
				}
				else
				{
					this.propertyGrid.Enabled = false;
					this.propertyGrid.SelectedObject = null;
				}
			}
		}
		private void buttonOk_Click(object sender, EventArgs e)
		{
			if (this._globalSettings.Apply == GlobalIncrementSettings.ApplyGlobalSettings.Always)
			{
				MessageBox.Show(Resources.GlobalMessage_alwaysApplyGlobalSettings, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			try
			{
				this._globalSettings.Save();
			}
			catch (Exception ex)
			{
				string text = "Failed saving default settings:\n" + ex.ToString();
				Logger.Write(text, LogLevel.Error);
				MessageBox.Show(this, text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			try
			{
				GlobalAddinSettings.Default.Save();
			}
			catch (Exception ex)
			{
				string text = "Failed saving global settings:\n" + ex.ToString();
				Logger.Write(text, LogLevel.Error);
				MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			try
			{
				if (this._solution != null)
				{
					this._solution.SetGlobalVariables();
				}
				base.Close();
			}
			catch (Exception ex)
			{
				string text = "Failed storing global variables:\n" + ex.ToString();
				Logger.Write(text, LogLevel.Error);
				MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		private void contextMenu_Opening(object sender, CancelEventArgs e)
		{
			BaseIncrementSettings selectedIncrementSettings = this.SelectedIncrementSettings;
			if (selectedIncrementSettings == null)
			{
				e.Cancel = true;
			}
			this.copyToGlobalSettingsToolStripMenuItem.Enabled = (selectedIncrementSettings is SolutionItemIncrementSettings);
		}
		private void copyToAllProjectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			BaseIncrementSettings selectedIncrementSettings = this.SelectedIncrementSettings;
			if (selectedIncrementSettings != null)
			{
				string str = "Global Settings";
				if (selectedIncrementSettings is SolutionItemIncrementSettings)
				{
					str = ((SolutionItemIncrementSettings)selectedIncrementSettings).Name;
				}
				DialogResult dialogResult = MessageBox.Show(this, "Copy the increment settings of \"" + str + "\" to all other items?", "Copy to all", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.Yes)
				{
					AddInSettingsForm.CopySettingsToAll(this._solution, selectedIncrementSettings);
				}
			}
		}
		private static void CopySettingsToAll(SolutionItem item, BaseIncrementSettings settings)
		{
			if (item.ItemType == SolutionItemType.Solution || item.ItemType == SolutionItemType.Project)
			{
				Logger.Write("Copying IncrementSettings to \"" + item.Name + "\"", LogLevel.Debug);
				item.IncrementSettings.CopyFrom(settings);
			}
			foreach (SolutionItem current in item.SubItems)
			{
				AddInSettingsForm.CopySettingsToAll(current, settings);
			}
		}
		private void copyToGlobalSettingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SolutionItemIncrementSettings solutionItemIncrementSettings = this.SelectedIncrementSettings as SolutionItemIncrementSettings;
			if (solutionItemIncrementSettings != null)
			{
				DialogResult dialogResult = MessageBox.Show(this, "Set the increment settings of \"" + solutionItemIncrementSettings.Name + "\" as global settings?", "Set as global settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.Yes)
				{
					Logger.Write("Copying from \"" + this.solutionTreeView.SelectedNode.Text + "\" to global settings", LogLevel.Debug);
					this._globalSettings.CopyFrom(solutionItemIncrementSettings);
				}
			}
		}
		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			BaseIncrementSettings selectedIncrementSettings = this.SelectedIncrementSettings;
			if (selectedIncrementSettings != null)
			{
				DialogResult dialogResult = MessageBox.Show(this, "Reset the increment settings of \"" + this.solutionTreeView.SelectedNode.Text + "\" to the defaults?", "Reset settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.Yes)
				{
					Logger.Write("Resetting increment settings  of \"" + this.solutionTreeView.SelectedNode.Text + "\"", LogLevel.Debug);
					selectedIncrementSettings.Reset();
					this.propertyGrid.Refresh();
				}
			}
		}
		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			BaseIncrementSettings selectedIncrementSettings = this.SelectedIncrementSettings;
			if (selectedIncrementSettings != null)
			{
				DialogResult dialogResult = MessageBox.Show(this, "Discard changes to \"" + this.solutionTreeView.SelectedNode.Text + "\"?", "Undo changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.Yes)
				{
					Logger.Write("Discard changes to \"" + this.solutionTreeView.SelectedNode.Text + "\"", LogLevel.Debug);
					selectedIncrementSettings.Load();
					this.propertyGrid.Refresh();
				}
			}
		}
		private void solutionTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			this.solutionTreeView.SelectedNode = e.Node;
		}
		private void AddInSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Logger.WriteEvent -= new EventHandler<Logger.WriteEventArgs>(this.Logger_WriteEvent);
		}
		private void textBoxLog_VisibleChanged(object sender, EventArgs e)
		{
			this.textBoxLog.Select(this.textBoxLog.Text.Length, 1);
			this.textBoxLog.ScrollToCaret();
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
			this.components = new Container();
			this.solutionTreeView = new TreeView();
			this.contextMenu = new ContextMenuStrip(this.components);
			this.copyToAllProjectsToolStripMenuItem = new ToolStripMenuItem();
			this.copyToGlobalSettingsToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.clearToolStripMenuItem = new ToolStripMenuItem();
			this.undoToolStripMenuItem = new ToolStripMenuItem();
			this.imageList = new ImageList(this.components);
			this.splitContainer1 = new SplitContainer();
			this.propertyGrid = new PropertyGrid();
			this.tabControl1 = new TabControl();
			this.tabPageSolution = new TabPage();
			this.tabPageGlobalSettings = new TabPage();
			this.propertyGridGlobalSettings = new PropertyGrid();
			this.tabPageLog = new TabPage();
			this.textBoxLog = new TextBox();
			this.buttonCancel = new Button();
			this.buttonOk = new Button();
			this.chkVersionIncrementEnabled = new CheckBox();
			this.chkVerboseLogEnabled = new CheckBox();
			this.contextMenu.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPageSolution.SuspendLayout();
			this.tabPageGlobalSettings.SuspendLayout();
			this.tabPageLog.SuspendLayout();
			base.SuspendLayout();
			this.solutionTreeView.ContextMenuStrip = this.contextMenu;
			this.solutionTreeView.Dock = DockStyle.Fill;
			this.solutionTreeView.HideSelection = false;
			this.solutionTreeView.ImageIndex = 0;
			this.solutionTreeView.ImageList = this.imageList;
			this.solutionTreeView.Indent = 20;
			this.solutionTreeView.ItemHeight = 18;
			this.solutionTreeView.Location = new Point(0, 0);
			this.solutionTreeView.Name = "solutionTreeView";
			this.solutionTreeView.SelectedImageIndex = 0;
			this.solutionTreeView.Size = new Size(255, 355);
			this.solutionTreeView.TabIndex = 0;
			this.solutionTreeView.AfterSelect += new TreeViewEventHandler(this.solutionTreeView_AfterSelect);
			this.solutionTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.solutionTreeView_NodeMouseClick);
			this.contextMenu.Items.AddRange(new ToolStripItem[]
			{
				this.copyToAllProjectsToolStripMenuItem,
				this.copyToGlobalSettingsToolStripMenuItem,
				this.toolStripSeparator1,
				this.clearToolStripMenuItem,
				this.undoToolStripMenuItem
			});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new Size(187, 98);
			this.contextMenu.Opening += new CancelEventHandler(this.contextMenu_Opening);
			this.copyToAllProjectsToolStripMenuItem.Name = "copyToAllProjectsToolStripMenuItem";
			this.copyToAllProjectsToolStripMenuItem.Size = new Size(186, 22);
			this.copyToAllProjectsToolStripMenuItem.Text = "Copy to all projects";
			this.copyToAllProjectsToolStripMenuItem.ToolTipText = "Copies the current settings to all projects";
			this.copyToAllProjectsToolStripMenuItem.Click += new EventHandler(this.copyToAllProjectsToolStripMenuItem_Click);
			this.copyToGlobalSettingsToolStripMenuItem.Name = "copyToGlobalSettingsToolStripMenuItem";
			this.copyToGlobalSettingsToolStripMenuItem.Size = new Size(186, 22);
			this.copyToGlobalSettingsToolStripMenuItem.Text = "Set as Global Settings";
			this.copyToGlobalSettingsToolStripMenuItem.ToolTipText = "Copies the current settings to the Global Settings";
			this.copyToGlobalSettingsToolStripMenuItem.Click += new EventHandler(this.copyToGlobalSettingsToolStripMenuItem_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(183, 6);
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new Size(186, 22);
			this.clearToolStripMenuItem.Text = "Reset to defaults";
			this.clearToolStripMenuItem.ToolTipText = "Resets the current settings to the defaults";
			this.clearToolStripMenuItem.Click += new EventHandler(this.clearToolStripMenuItem_Click);
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new Size(186, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			this.undoToolStripMenuItem.Click += new EventHandler(this.undoToolStripMenuItem_Click);
			this.imageList.ColorDepth = ColorDepth.Depth32Bit;
			this.imageList.ImageSize = new Size(16, 16);
			this.imageList.TransparentColor = Color.Transparent;
			this.splitContainer1.Dock = DockStyle.Fill;
			this.splitContainer1.Location = new Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Panel1.Controls.Add(this.solutionTreeView);
			this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
			this.splitContainer1.Size = new Size(617, 355);
			this.splitContainer1.SplitterDistance = 255;
			this.splitContainer1.TabIndex = 1;
			this.propertyGrid.Dock = DockStyle.Fill;
			this.propertyGrid.Enabled = false;
			this.propertyGrid.Location = new Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new Size(358, 355);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.ToolbarVisible = false;
			this.tabControl1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.tabControl1.Controls.Add(this.tabPageSolution);
			this.tabControl1.Controls.Add(this.tabPageGlobalSettings);
			this.tabControl1.Controls.Add(this.tabPageLog);
			this.tabControl1.Location = new Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new Size(631, 387);
			this.tabControl1.TabIndex = 1;
			this.tabPageSolution.Controls.Add(this.splitContainer1);
			this.tabPageSolution.Location = new Point(4, 22);
			this.tabPageSolution.Name = "tabPageSolution";
			this.tabPageSolution.Padding = new Padding(3);
			this.tabPageSolution.Size = new Size(623, 361);
			this.tabPageSolution.TabIndex = 0;
			this.tabPageSolution.Text = "Solution";
			this.tabPageSolution.UseVisualStyleBackColor = true;
			this.tabPageGlobalSettings.Controls.Add(this.propertyGridGlobalSettings);
			this.tabPageGlobalSettings.Location = new Point(4, 22);
			this.tabPageGlobalSettings.Name = "tabPageGlobalSettings";
			this.tabPageGlobalSettings.Padding = new Padding(3);
			this.tabPageGlobalSettings.Size = new Size(623, 361);
			this.tabPageGlobalSettings.TabIndex = 1;
			this.tabPageGlobalSettings.Text = "Global Settings";
			this.tabPageGlobalSettings.UseVisualStyleBackColor = true;
			this.propertyGridGlobalSettings.Dock = DockStyle.Fill;
			this.propertyGridGlobalSettings.Location = new Point(3, 3);
			this.propertyGridGlobalSettings.Name = "propertyGridGlobalSettings";
			this.propertyGridGlobalSettings.Size = new Size(617, 355);
			this.propertyGridGlobalSettings.TabIndex = 1;
			this.propertyGridGlobalSettings.ToolbarVisible = false;
			this.tabPageLog.Controls.Add(this.textBoxLog);
			this.tabPageLog.Location = new Point(4, 22);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Padding = new Padding(3);
			this.tabPageLog.Size = new Size(623, 361);
			this.tabPageLog.TabIndex = 2;
			this.tabPageLog.Text = "Log";
			this.tabPageLog.UseVisualStyleBackColor = true;
			this.textBoxLog.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxLog.BackColor = SystemColors.Window;
			this.textBoxLog.Location = new Point(6, 6);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = ScrollBars.Both;
			this.textBoxLog.Size = new Size(611, 349);
			this.textBoxLog.TabIndex = 0;
			this.textBoxLog.VisibleChanged += new EventHandler(this.textBoxLog_VisibleChanged);
			this.buttonCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonCancel.DialogResult = DialogResult.Cancel;
			this.buttonCancel.Location = new Point(568, 405);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonOk.Location = new Point(487, 405);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new Size(75, 23);
			this.buttonOk.TabIndex = 0;
			this.buttonOk.Text = "&Ok";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new EventHandler(this.buttonOk_Click);
			this.chkVersionIncrementEnabled.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.chkVersionIncrementEnabled.AutoSize = true;
			this.chkVersionIncrementEnabled.Checked = GlobalAddinSettings.Default.IsEnabled;
			this.chkVersionIncrementEnabled.CheckState = CheckState.Checked;
			this.chkVersionIncrementEnabled.DataBindings.Add(new Binding("Checked", GlobalAddinSettings.Default, "IsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
			this.chkVersionIncrementEnabled.Location = new Point(12, 410);
			this.chkVersionIncrementEnabled.Name = "chkVersionIncrementEnabled";
			this.chkVersionIncrementEnabled.Size = new Size(153, 17);
			this.chkVersionIncrementEnabled.TabIndex = 2;
			this.chkVersionIncrementEnabled.Text = "Version Increment Enabled";
			this.chkVersionIncrementEnabled.UseVisualStyleBackColor = true;
			this.chkVerboseLogEnabled.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.chkVerboseLogEnabled.AutoSize = true;
			this.chkVerboseLogEnabled.Checked = GlobalAddinSettings.Default.IsVerboseLogEnabled;
			this.chkVerboseLogEnabled.CheckState = CheckState.Unchecked;
			this.chkVerboseLogEnabled.DataBindings.Add(new Binding("Checked", GlobalAddinSettings.Default, "IsVerboseLogEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
			this.chkVerboseLogEnabled.Location = new Point(171, 410);
			this.chkVerboseLogEnabled.Name = "chkVerboseLogEnabled";
			this.chkVerboseLogEnabled.Size = new Size(139, 17);
			this.chkVerboseLogEnabled.TabIndex = 3;
			this.chkVerboseLogEnabled.Text = "Is Verbose Log Enabled";
			this.chkVerboseLogEnabled.UseVisualStyleBackColor = true;
			base.AcceptButton = this.buttonOk;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.buttonCancel;
			base.ClientSize = new Size(655, 440);
			base.Controls.Add(this.chkVerboseLogEnabled);
			base.Controls.Add(this.tabControl1);
			base.Controls.Add(this.chkVersionIncrementEnabled);
			base.Controls.Add(this.buttonOk);
			base.Controls.Add(this.buttonCancel);
			base.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			base.Name = "AddInSettingsForm";
			base.ShowIcon = false;
			this.Text = "Build Version Increment Settings";
			base.FormClosed += new FormClosedEventHandler(this.AddInSettingsForm_FormClosed);
			this.contextMenu.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPageSolution.ResumeLayout(false);
			this.tabPageGlobalSettings.ResumeLayout(false);
			this.tabPageLog.ResumeLayout(false);
			this.tabPageLog.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
