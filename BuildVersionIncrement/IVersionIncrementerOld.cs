using System;
namespace BuildVersionIncrement
{
	internal interface IVersionIncrementerOld
	{
		DateTime BuildStartDate
		{
			get;
			set;
		}
		DateTime ProjectStartDate
		{
			get;
			set;
		}
		string Increment(int current, OLD_IncrementStyle incrementStyle);
	}
}
