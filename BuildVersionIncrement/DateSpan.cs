using System;
namespace BuildVersionIncrement
{
	public class DateSpan
	{
		private int _days;
		private int _months;
		private int _years;
		public int Days
		{
			get
			{
				return this._days;
			}
			set
			{
				this._days = value;
			}
		}
		public int Months
		{
			get
			{
				return this._months;
			}
			set
			{
				this._months = value;
			}
		}
		public int Years
		{
			get
			{
				return this._years;
			}
			set
			{
				this._years = value;
			}
		}
		public static DateSpan GetDateDifference(DateTime date, DateTime dateToCompare)
		{
			int num = (date.Year - dateToCompare.Year) * 12 + date.Month - dateToCompare.Month;
			DateTime dateTime = new DateTime(date.Subtract(dateToCompare).Ticks);
			return new DateSpan
			{
				Years = dateTime.Year - 1,
				Months = dateTime.Month - 1,
				Days = dateTime.Day - 1
			};
		}
	}
}
