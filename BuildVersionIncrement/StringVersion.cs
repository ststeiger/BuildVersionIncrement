using System;
using System.Text;
namespace BuildVersionIncrement
{
	public class StringVersion
	{
		public string Major
		{
			get;
			set;
		}
		public string Minor
		{
			get;
			set;
		}
		public string Build
		{
			get;
			set;
		}
		public string Revision
		{
			get;
			set;
		}
		public StringVersion(string fullVersion)
		{
			string[] array = fullVersion.Split(new char[]
			{
				'.'
			});
			int num = array.Length;
			if (num < 2 || num > 4)
			{
				throw new ArgumentException();
			}
			this.Major = array[0];
			this.Minor = array[1];
			if (num > 2)
			{
				this.Build = array[2];
			}
			if (num > 3)
			{
				this.Revision = array[3];
			}
		}
		public StringVersion(string major, string minor, string build, string revision)
		{
			this.Major = major;
			this.Minor = minor;
			this.Build = build;
			this.Revision = revision;
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Major + "." + this.Minor);
			if (this.Build != null)
			{
				stringBuilder.Append("." + this.Build);
			}
			if (this.Revision != null)
			{
				stringBuilder.Append("." + this.Revision);
			}
			return stringBuilder.ToString();
		}
		public string ToString(int componentCount)
		{
			if (componentCount < 0 || componentCount > 4)
			{
				throw new ArgumentException();
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (componentCount > 0)
			{
				stringBuilder.Append(this.Major);
			}
			if (componentCount > 1)
			{
				stringBuilder.Append("." + this.Minor);
			}
			if (componentCount > 2)
			{
				stringBuilder.Append("." + (this.Build ?? "0"));
			}
			if (componentCount > 3)
			{
				stringBuilder.Append("." + (this.Revision ?? "0"));
			}
			return stringBuilder.ToString();
		}
		public static bool operator ==(StringVersion a, StringVersion b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && (a.Major == b.Major && a.Minor == b.Minor && a.Build == b.Build) && a.Revision == b.Revision);
		}
		public static bool operator !=(StringVersion a, StringVersion b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			StringVersion stringVersion = obj as StringVersion;
			return stringVersion != null && this == stringVersion;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
