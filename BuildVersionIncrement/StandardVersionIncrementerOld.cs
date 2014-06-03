using System;
namespace BuildVersionIncrement
{
	internal class StandardVersionIncrementerOld : IVersionIncrementerOld
	{
		private DateTime _buildStartDate;
		private DateTime _projectStartDate = new DateTime(1975, 10, 21);
		public DateTime BuildStartDate
		{
			get
			{
				return this._buildStartDate;
			}
			set
			{
				this._buildStartDate = value;
			}
		}
		public DateTime ProjectStartDate
		{
			get
			{
				return this._projectStartDate;
			}
			set
			{
				this._projectStartDate = value;
			}
		}
		public virtual string Increment(int current, OLD_IncrementStyle incrementStyle)
		{
			string arg = this.BuildStartDate.DayOfYear.ToString("000");
			int num = this.BuildStartDate.Year - this.ProjectStartDate.Year;
			string text = this.BuildStartDate.ToString("yy");
			if (current < 0)
			{
				current = 0;
			}
			string result;
			switch (incrementStyle)
			{
			case OLD_IncrementStyle.None:
				result = current.ToString();
				break;
			case OLD_IncrementStyle.DayStamp:
				result = this.BuildStartDate.Day.ToString();
				break;
			case OLD_IncrementStyle.DeltaBaseDate:
			{
				DateSpan dateDifference = DateSpan.GetDateDifference(this.BuildStartDate, this.ProjectStartDate);
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
				result = (current + 1).ToString();
				break;
			case OLD_IncrementStyle.MonthStamp:
				result = this.BuildStartDate.Month.ToString();
				break;
			case OLD_IncrementStyle.TimeStamp:
				result = string.Format("{0:00}{1:00}", this.BuildStartDate.Hour, this.BuildStartDate.Minute);
				break;
			case OLD_IncrementStyle.YearStamp:
				result = this.BuildStartDate.Year.ToString();
				break;
			case OLD_IncrementStyle.YearDayOfYear:
				result = string.Format("{0}{1:000}", text, arg);
				break;
			case OLD_IncrementStyle.YearDecadeStamp:
				result = text;
				break;
			case OLD_IncrementStyle.MonthAndDayStamp:
				result = string.Format("{0:00}{1:00}", this.BuildStartDate.Month, this.BuildStartDate.Day);
				break;
			case OLD_IncrementStyle.DeltaBaseDateInDays:
				result = ((int)this.BuildStartDate.Subtract(this.ProjectStartDate).TotalDays).ToString();
				break;
			default:
				throw new ApplicationException("Unknown increment style: " + incrementStyle.ToString());
			}
			return result;
		}
	}
}
