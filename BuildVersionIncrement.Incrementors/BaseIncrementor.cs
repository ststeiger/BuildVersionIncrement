using System;
using System.ComponentModel;
namespace BuildVersionIncrement.Incrementors
{
	[TypeConverter(typeof(BaseIncrementorConverter))]
	public abstract class BaseIncrementor
	{
		public abstract string Name
		{
			get;
		}
		public abstract string Description
		{
			get;
		}
		public BaseIncrementor()
		{
			if (string.IsNullOrEmpty(this.Name) || this.Name.Contains("."))
			{
				throw new FormatException("The Name property of the class " + base.GetType().FullName + " is invalid.");
			}
		}
		public abstract void Increment(IncrementContext context, VersionComponent versionComponent);
	}
}
