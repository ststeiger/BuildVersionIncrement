using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
namespace Qreed.Windows.Forms
{
	[DebuggerDisplay("ProgressDialog")]
	public class ProgressDialog : Form
	{
		private object _argument;
		private ProgressDialogResult _result;
		private volatile bool _autoClose;
		private IContainer components = null;
		private Button cancelButton;
		protected BackgroundWorker backgroundWorker;
		private Label statusLabel;
		public ProgressBar ProgressBar;
		public event DoWorkEventHandler DoWork;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public object Argument
		{
			get
			{
				return this._argument;
			}
			set
			{
				this._argument = value;
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public ProgressDialogResult Result
		{
			get
			{
				return this._result;
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool CancellationPending
		{
			get
			{
				return this.backgroundWorker.CancellationPending;
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool AutoClose
		{
			get
			{
				return this._autoClose;
			}
			set
			{
				this._autoClose = value;
			}
		}
		public ProgressDialog()
		{
			this.InitializeComponent();
			this.statusLabel.Text = "";
		}
		protected override void OnShown(EventArgs e)
		{
			this.backgroundWorker.RunWorkerAsync(this.Argument);
			base.OnShown(e);
		}
		public void ReportProgress(int percent, string statusText)
		{
			this.backgroundWorker.ReportProgress(percent, statusText);
		}
		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Debug.WriteLine("BackgroundWorker_DoWork " + Thread.CurrentThread.ManagedThreadId);
			if (this.DoWork != null)
			{
				this.DoWork(this, e);
			}
		}
		private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Debug.WriteLine("BackgroundWorker_ProgressChanged " + Thread.CurrentThread.ManagedThreadId);
			this.statusLabel.Text = (string)e.UserState;
			this.ProgressBar.Value = e.ProgressPercentage;
		}
		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Debug.WriteLine("BackgroundWorker_RunWorkerCompleted " + Thread.CurrentThread.ManagedThreadId);
			bool cancelled = e.Cancelled;
			Exception error = e.Error;
			object value = null;
			if (error == null && !cancelled)
			{
				value = e.Result;
			}
			this._result = new ProgressDialogResult(value, cancelled, error);
			if (this.AutoClose || this._result.Exception != null)
			{
				base.Close();
			}
			else
			{
				this.cancelButton.Text = "&Close";
				this.cancelButton.Enabled = true;
				this.cancelButton.Focus();
			}
		}
		private void cancelButton_Click(object sender, EventArgs e)
		{
			if (this.EndBackgroundWorker())
			{
				base.Close();
			}
		}
		private bool EndBackgroundWorker()
		{
			bool result;
			if (this.backgroundWorker.IsBusy)
			{
				this.backgroundWorker.CancelAsync();
				Cursor.Current = Cursors.WaitCursor;
				this.cancelButton.Enabled = false;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
		private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.EndBackgroundWorker())
			{
				e.Cancel = true;
			}
			else
			{
				if (this.Result.IsCancelled || this.Result.Exception != null)
				{
					base.DialogResult = DialogResult.Cancel;
				}
				else
				{
					base.DialogResult = DialogResult.OK;
				}
			}
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
			this.ProgressBar = new ProgressBar();
			this.cancelButton = new Button();
			this.statusLabel = new Label();
			base.SuspendLayout();
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += new DoWorkEventHandler(this.BackgroundWorker_DoWork);
			this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
			this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.BackgroundWorker_ProgressChanged);
			this.ProgressBar.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.ProgressBar.Location = new Point(12, 28);
			this.ProgressBar.Name = "ProgressBar";
			this.ProgressBar.Size = new Size(318, 16);
			this.ProgressBar.TabIndex = 0;
			this.cancelButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(255, 48);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
			this.statusLabel.AutoSize = true;
			this.statusLabel.Location = new Point(12, 9);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new Size(61, 13);
			this.statusLabel.TabIndex = 2;
			this.statusLabel.Text = "Status Text";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new Size(342, 83);
			base.ControlBox = false;
			base.Controls.Add(this.statusLabel);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.ProgressBar);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.KeyPreview = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ProgressDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.Text = "ProgressDialog";
			base.FormClosing += new FormClosingEventHandler(this.ProgressDialog_FormClosing);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
