using BuildVersionIncrement.Incrementors;
using System;
using System.ComponentModel;
using System.Diagnostics;
namespace BuildVersionIncrement
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	internal class VersioningStyle
	{
		private BaseIncrementor _major = BuiltInBaseIncrementor.NoneIncrementor.Instance;
		private BaseIncrementor _minor = BuiltInBaseIncrementor.NoneIncrementor.Instance;
		private BaseIncrementor _build = BuiltInBaseIncrementor.NoneIncrementor.Instance;
		private BaseIncrementor _revision = BuiltInBaseIncrementor.NoneIncrementor.Instance;
		[Description("Major update style"), NotifyParentProperty(true)]
		public BaseIncrementor Major
		{
			get
			{
				return this._major ?? BuiltInBaseIncrementor.NoneIncrementor.Instance;
			}
			set
			{
				Debug.Assert(value != null);
				this._major = value;
			}
		}
		[Description("Minor update style"), NotifyParentProperty(true)]
		public BaseIncrementor Minor
		{
			get
			{
				return this._minor ?? BuiltInBaseIncrementor.NoneIncrementor.Instance;
			}
			set
			{
				Debug.Assert(value != null);
				this._minor = value;
			}
		}
		[Description("Build update style"), NotifyParentProperty(true)]
		public BaseIncrementor Build
		{
			get
			{
				return this._build ?? BuiltInBaseIncrementor.NoneIncrementor.Instance;
			}
			set
			{
				Debug.Assert(value != null);
				this._build = value;
			}
		}
		[Description("Revision update style"), NotifyParentProperty(true)]
		public BaseIncrementor Revision
		{
			get
			{
				return this._revision ?? BuiltInBaseIncrementor.NoneIncrementor.Instance;
			}
			set
			{
				Debug.Assert(value != null);
				this._revision = value;
			}
		}
		public VersioningStyle()
		{
		}
		public VersioningStyle(VersioningStyle other)
		{
			this.Major = other.Major;
			this.Minor = other.Minor;
			this.Build = other.Build;
			this.Revision = other.Revision;
		}
		internal string ToGlobalVariable()
		{
			return this.ToString();
		}
		internal void FromOldIncrementStyle(OLD_IncrementStyle major, OLD_IncrementStyle minor, OLD_IncrementStyle build, OLD_IncrementStyle revision)
		{
			this.Major = BuildVersionIncrementor.Instance.Incrementors[major];
			this.Minor = BuildVersionIncrementor.Instance.Incrementors[minor];
			this.Build = BuildVersionIncrementor.Instance.Incrementors[build];
			this.Revision = BuildVersionIncrementor.Instance.Incrementors[revision];
		}
		internal void FromGlobalVariable(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (value.Contains("."))
				{
					string[] array = value.Split(".".ToCharArray());
					if (array.Length != 4)
					{
						throw new ApplicationException("Invalid versioning style \"" + value + "\".");
					}
					this.Major = BuildVersionIncrementor.Instance.Incrementors[array[0]];
					this.Minor = BuildVersionIncrementor.Instance.Incrementors[array[1]];
					this.Build = BuildVersionIncrementor.Instance.Incrementors[array[2]];
					this.Revision = BuildVersionIncrementor.Instance.Incrementors[array[3]];
				}
				else
				{
					OLD_BuildVersioningStyleType oLD_BuildVersioningStyleType = (OLD_BuildVersioningStyleType)Enum.Parse(typeof(OLD_BuildVersioningStyleType), value);
					OLD_IncrementStyle major = OLD_IncrementStyle.None;
					OLD_IncrementStyle minor = OLD_IncrementStyle.None;
					OLD_IncrementStyle revision = OLD_IncrementStyle.None;
					OLD_IncrementStyle build;
					switch (oLD_BuildVersioningStyleType)
					{
					case OLD_BuildVersioningStyleType.DeltaBaseDate:
						build = OLD_IncrementStyle.DeltaBaseDate;
						revision = OLD_IncrementStyle.TimeStamp;
						break;
					case OLD_BuildVersioningStyleType.YearDayOfYear_Timestamp:
						build = OLD_IncrementStyle.YearDayOfYear;
						revision = OLD_IncrementStyle.TimeStamp;
						break;
					case OLD_BuildVersioningStyleType.DeltaBaseYear:
						build = OLD_IncrementStyle.DeltaBaseYearDayOfYear;
						revision = OLD_IncrementStyle.TimeStamp;
						break;
					case OLD_BuildVersioningStyleType.YearDayOfYear_AutoIncrement:
						build = OLD_IncrementStyle.YearDayOfYear;
						revision = OLD_IncrementStyle.Increment;
						break;
					case OLD_BuildVersioningStyleType.AutoIncrementBuildVersion:
						build = OLD_IncrementStyle.Increment;
						break;
					default:
						throw new ApplicationException("Unknown (old) versioning type: " + oLD_BuildVersioningStyleType.ToString());
					}
					this.FromOldIncrementStyle(major, minor, build, revision);
				}
			}
			else
			{
				this.Major = (this.Minor = (this.Build = (this.Revision = null)));
			}
		}
		internal StringVersion Increment(StringVersion currentVersion, DateTime buildStartDate, DateTime projectStartDate, SolutionItem solutionItem)
		{
			IncrementContext incrementContext = new IncrementContext(currentVersion, buildStartDate, projectStartDate, solutionItem.Filename);
			BaseIncrementor[] array = new BaseIncrementor[]
			{
				this.Major,
				this.Minor,
				this.Build,
				this.Revision
			};
			for (int i = 0; i < 4; i++)
			{
				BaseIncrementor baseIncrementor = array[i];
				if (baseIncrementor != null)
				{
					VersionComponent versionComponent = (VersionComponent)i;
					baseIncrementor.Increment(incrementContext, versionComponent);
					if (!incrementContext.Continue)
					{
						break;
					}
				}
			}
			return incrementContext.NewVersion;
		}
		internal static string GetDefaultGlobalVariable()
		{
			return new VersioningStyle().ToGlobalVariable();
		}
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				this.Major.Name,
				this.Minor.Name,
				this.Build.Name,
				this.Revision.Name
			});
		}
		private bool ShouldSerializeMajor()
		{
			return this._major != BuiltInBaseIncrementor.NoneIncrementor.Instance;
		}
		private bool ShouldSerializeMinor()
		{
			return this._minor != BuiltInBaseIncrementor.NoneIncrementor.Instance;
		}
		private bool ShouldSerializeBuild()
		{
			return this._build != BuiltInBaseIncrementor.NoneIncrementor.Instance;
		}
		private bool ShouldSerializeRevision()
		{
			return this._revision != BuiltInBaseIncrementor.NoneIncrementor.Instance;
		}
	}
}
