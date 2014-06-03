using Qreed.Reflection;
using System;
namespace BuildVersionIncrement.Incrementors
{
	internal abstract class BuiltInBaseIncrementor : BaseIncrementor
	{
		internal class NoneIncrementor : BuiltInBaseIncrementor
		{
			public static readonly BuiltInBaseIncrementor.NoneIncrementor Instance = new BuiltInBaseIncrementor.NoneIncrementor();
		}
		private class DayStampIncrementor : BuiltInBaseIncrementor
		{
		}
		private class DeltaBaseDateIncrementor : BuiltInBaseIncrementor
		{
		}
		private class DeltaBaseDateInDaysIncrementor : BuiltInBaseIncrementor
		{
		}
		private class DeltaBaseYearDayOfYearIncrementor : BuiltInBaseIncrementor
		{
		}
		private class DeltaBaseYearIncrementor : BuiltInBaseIncrementor
		{
		}
		private class IncrementIncrementor : BuiltInBaseIncrementor
		{
		}
		private class MonthStampIncrementor : BuiltInBaseIncrementor
		{
		}
		private class TimeStampIncrementor : BuiltInBaseIncrementor
		{
		}
		private class YearStampIncrementor : BuiltInBaseIncrementor
		{
		}
		private class YearDayOfYearIncrementor : BuiltInBaseIncrementor
		{
		}
		private class YearDecadeStampIncrementor : BuiltInBaseIncrementor
		{
		}
		private class MonthAndDayStampIncrementor : BuiltInBaseIncrementor
		{
		}
		public override string Name
		{
			get
			{
				return this.IncrementStyle.ToString();
			}
		}
		public override string Description
		{
			get
			{
				return EnumHelper.GetDescription(this.IncrementStyle);
			}
		}
		private OLD_IncrementStyle IncrementStyle
		{
			get
			{
				string text = base.GetType().Name;
				text = text.Substring(0, text.Length - "Incrementor".Length);
				return (OLD_IncrementStyle)Enum.Parse(typeof(OLD_IncrementStyle), text);
			}
		}
		public override void Increment(IncrementContext context, VersionComponent versionComponent)
		{
			string currentVersionComponentValue = context.GetCurrentVersionComponentValue(versionComponent);
			string value = this.Increment(currentVersionComponentValue, context.BuildStartDate, context.ProjectStartDate, context.ProjectFilename);
			context.SetNewVersionComponentValue(versionComponent, value);
		}
		private string Increment(string value, DateTime buildStart, DateTime projectStart, string projectFilePath)
		{
			string arg = buildStart.DayOfYear.ToString("000");
			int num = buildStart.Year - projectStart.Year;
			string text = buildStart.ToString("yy");
			int num2 = 0;
			int.TryParse(value, out num2);
			if (num2 < 0)
			{
				num2 = 0;
			}
			string result;
			switch (this.IncrementStyle)
			{
			case OLD_IncrementStyle.None:
				result = value;
				break;
			case OLD_IncrementStyle.DayStamp:
				result = buildStart.Day.ToString();
				break;
			case OLD_IncrementStyle.DeltaBaseDate:
			{
				DateSpan dateDifference = DateSpan.GetDateDifference(buildStart, projectStart);
				result = string.Format("{0}{1:00}", dateDifference.Years * 12 + dateDifference.Months, dateDifference.Days);
				break;
			}
			case OLD_IncrementStyle.DeltaBaseYearDayOfYear:
				result = string.Format("{0}{1:000}", num, arg);
				break;
			case OLD_IncrementStyle.DeltaBaseYear:
				result = num.ToString();
				break;
			case OLD_IncrementStyle.Increment:
				result = (num2 + 1).ToString();
				break;
			case OLD_IncrementStyle.MonthStamp:
				result = buildStart.Month.ToString();
				break;
			case OLD_IncrementStyle.TimeStamp:
				result = string.Format("{0:00}{1:00}", buildStart.Hour, buildStart.Minute);
				break;
			case OLD_IncrementStyle.YearStamp:
				result = buildStart.Year.ToString();
				break;
			case OLD_IncrementStyle.YearDayOfYear:
				result = string.Format("{0}{1:000}", text, arg);
				break;
			case OLD_IncrementStyle.YearDecadeStamp:
				result = text;
				break;
			case OLD_IncrementStyle.MonthAndDayStamp:
				result = string.Format("{0:00}{1:00}", buildStart.Month, buildStart.Day);
				break;
			case OLD_IncrementStyle.DeltaBaseDateInDays:
				result = ((int)buildStart.Subtract(projectStart).TotalDays).ToString();
				break;
			default:
				throw new ApplicationException("Unknown increment style: " + this.IncrementStyle.ToString());
			}
			return result;
		}
	}
}
