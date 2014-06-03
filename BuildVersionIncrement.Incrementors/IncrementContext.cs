using System;
namespace BuildVersionIncrement.Incrementors
{
	public class IncrementContext
	{
		private StringVersion _newVersion;
		public StringVersion CurrentVersion
		{
			get;
			private set;
		}
		public DateTime BuildStartDate
		{
			get;
			private set;
		}
		public DateTime ProjectStartDate
		{
			get;
			private set;
		}
		public string ProjectFilename
		{
			get;
			private set;
		}
		public StringVersion NewVersion
		{
			get
			{
				return this._newVersion;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this._newVersion = value;
			}
		}
		public bool Continue
		{
			get;
			set;
		}
		internal IncrementContext(StringVersion currentVersion, DateTime buildStartDate, DateTime projectStartDate, string projectFilename)
		{
			this.CurrentVersion = currentVersion;
			this.BuildStartDate = buildStartDate;
			this.ProjectStartDate = projectStartDate;
			this.ProjectFilename = projectFilename;
			this.NewVersion = new StringVersion(currentVersion.Major, currentVersion.Minor, currentVersion.Build, currentVersion.Revision);
			this.Continue = true;
		}
		public string GetCurrentVersionComponentValue(VersionComponent component)
		{
			string result;
			switch (component)
			{
			case VersionComponent.Major:
				result = this.CurrentVersion.Major;
				break;
			case VersionComponent.Minor:
				result = this.CurrentVersion.Minor;
				break;
			case VersionComponent.Build:
				result = this.CurrentVersion.Build;
				break;
			case VersionComponent.Revision:
				result = this.CurrentVersion.Revision;
				break;
			default:
				result = "0";
				break;
			}
			return result;
		}
		public void SetNewVersionComponentValue(VersionComponent component, string value)
		{
			switch (component)
			{
			case VersionComponent.Major:
				this.NewVersion = new StringVersion(value, this.NewVersion.Minor, this.NewVersion.Build, this.NewVersion.Revision);
				break;
			case VersionComponent.Minor:
				this.NewVersion = new StringVersion(this.NewVersion.Major, value, this.NewVersion.Build, this.NewVersion.Revision);
				break;
			case VersionComponent.Build:
				this.NewVersion = new StringVersion(this.NewVersion.Major, this.NewVersion.Minor, value, this.NewVersion.Revision);
				break;
			case VersionComponent.Revision:
				this.NewVersion = new StringVersion(this.NewVersion.Major, this.NewVersion.Minor, this.NewVersion.Build, value);
				break;
			}
		}
	}
}
