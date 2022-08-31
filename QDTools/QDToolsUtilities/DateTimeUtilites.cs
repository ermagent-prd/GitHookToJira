using System;
using System.Collections.Generic;
using System.Linq;

namespace QDToolsUtilities
{
    public interface IDateTimeUtilities
    {
        /// <summary>
        /// Add a lag to a starting date
        /// If starting date is last day of month then result date will be last day of month.
        /// </summary>
        /// <param name="sourceDate">starting date</param>
        /// <param name="years">years lag</param>
        /// <param name="months">months lag</param>
        /// <param name="days">days lag</param>
        /// <returns>The result date</returns>
        DateTime ToDateCheckingEnd(DateTime sourceDate, int years, int months, int days);

        Int32 DayDiff(DateTime startDate, DateTime endDate);
    }

    public class DateTimeUtilities : IDateTimeUtilities
	{
		#region Private methods

		private static long Fix(double number)
		{
			return
				number >= 0 ?
				(long)Math.Floor(number) :
				(long)Math.Ceiling(number);
		}

		private static DateTime LastDayOfMonthPeriod(DateTime startingDate, int monthPeriod)
		{
			int quarter = (startingDate.Month - 1) / monthPeriod;
			int lastMonthInPeriod = (quarter + 1) * monthPeriod;
			int lastDayInMonth = DateTime.DaysInMonth(startingDate.Year, lastMonthInPeriod);
			return new DateTime(startingDate.Year, lastMonthInPeriod, lastDayInMonth);
		}

		#endregion Private methods

		#region Public methods

		public static DateTime AddToDate(
			DateTime startingDate,
			Int32 years,
			Int32 months,
			Int32 days,
			Boolean takeEndOfMonth)
		{
			DateTime resultDate = startingDate;
			resultDate = resultDate.AddYears(years);

			if (months != 0)
			{
				resultDate = resultDate.AddMonths(months);
				if (takeEndOfMonth &&
					DateTimeUtilities.IsEndOfMonth(startingDate) &&
					!DateTimeUtilities.IsEndOfMonth(resultDate))
				{
					resultDate = DateTimeUtilities.EndOfMonth(resultDate);
				}

			}
			resultDate = resultDate.AddDays(days);
			return resultDate;
		}


		public static DateTime Max(DateTime d1, DateTime d2)
		{
			return DateTime.Compare(d1, d2) < 0 ?
				d2 :
				d1;
		}

		public static DateTime MaxDate(params DateTime[] dates)
		{
			return dates.Max();
		}

		public static DateTime? Max(DateTime? d1, DateTime? d2)
		{
			if (Nullable.Compare<DateTime>(d1, d2) < 0)
			{
				return d2;
			}
			return d1;
		}

		public static DateTime? Max(params DateTime?[] dates)
		{
			if (dates == null || !dates.Any())
				return null;

			DateTime? max = dates[0];

			for (int i = 1; i < dates.Count(); i++)
			{
				if (Nullable.Compare<DateTime>(max, dates[i]) < 0)
				{
					max = dates[i];
				}
			}
			return max;
		}

		public static DateTime Min(DateTime d1, DateTime d2)
		{
			if (DateTime.Compare(d1, d2) > 0)
			{
				return d2;
			}
			return d1;
		}

		/// <summary>
		/// Find minimum value between two dates. Null is less then every date.
		/// </summary>
		public static DateTime? Min(DateTime? d1, DateTime? d2)
		{
			if (Nullable.Compare<DateTime>(d1, d2) > 0)
			{
				return d2;
			}
			return d1;
		}

		public static Boolean IsEndOfMonth(DateTime date)
		{
			return (DateTime.DaysInMonth(date.Year, date.Month) == date.Day);
		}

        public static Boolean IsEndOfYear(DateTime date)
        {
            return (date.Month == 12 && date.Day == 31);
        }

        /// <summary>
        /// Gets the date if is end of month, else gets the previous end of month
        /// </summary>
        /// <param name="date">The starting date</param>
        /// <returns>The previous end of month</returns>
        public static DateTime PreviousEndOfMonth(DateTime date)
		{
			if (IsEndOfMonth(date))
				return date;

			DateTime startOfMonth =
				new DateTime(date.Year, date.Month, 1);

			return startOfMonth.AddDays(-1);
		}

		/// <summary>
		/// Gets next end of month of input date
		/// If input date is end of month, gets the next end of month
		/// </summary>
		public static DateTime NextEndOfMonth(DateTime date)
		{
			if (IsEndOfMonth(date))
				return IncMonthCheckingEnd(date, 1);

			return EndOfMonth(date);
		}

		/// <summary>
		/// Gets the previous month end of month
		/// </summary>
		/// <param name="date">Starting date</param>
		/// <returns>The previous month end of month </returns>
		public static DateTime PreviousMonthEndOfMonth(DateTime date)
		{
			DateTime thisMonth = new DateTime(date.Year, date.Month, 1);
			return thisMonth.AddDays(-1);
		}

		public static DateTime IncMonthCheckingEnd(DateTime fromDate, int months)
		{
			DateTime dtDate = fromDate.AddMonths(months);
			if (IsEndOfMonth(fromDate))
				dtDate = EndOfMonth(dtDate);
			return dtDate;
		}

		public static DateTime IncSolarMonths(DateTime fromDate, int months)
		{
			if (DateTimeUtilities.IsEndOfMonth(fromDate))
				return DateTimeUtilities.IncMonthCheckingEnd(fromDate, months);
			return DateTimeUtilities.IncMonthCheckingEnd(DateTimeUtilities.EndOfMonth(fromDate), months - 1);
		}

		public static DateTime IncDaysCheckingEnd(DateTime fromDate, int days)
		{
			DateTime dtDate = fromDate.AddDays(days);
			if (IsEndOfMonth(fromDate))
				dtDate = EndOfMonth(dtDate);
			return dtDate;
		}

		public static DateTime IncCheckingEnd(DateTime sourceDate, int yy, int mm, int dd)
		{
			return IncMonthCheckingEnd(sourceDate, yy * 12 + mm).AddDays(dd);
		}

		/// <summary>
		/// Only for test.  Use IncCheckingEnd that is the same method.
		/// </summary>
		public DateTime ToDateCheckingEnd(DateTime sourceDate, int years, int months, int days)
		{
			return IncCheckingEnd(sourceDate, years, months, days);
		}

		public static DateTime IncCheckingEnd(DateTime sourceDate, int yy, int mm, int dd, int times)
		{
			return IncMonthCheckingEnd(sourceDate, times * yy * 12 + times * mm).AddDays(times * dd);
		}

		public static long DateDiffByYear(
			DateTime gDate,
			DateTime lDate)
		{
			return gDate.Year - lDate.Year;
		}

		public static long DateDiffByMonth(
			DateTime gDate,
			DateTime lDate)
		{
			return (gDate.Month - lDate.Month) + (12 * (gDate.Year - lDate.Year));
		}

		public static long DateDiffByDay(
			DateTime gDate,
			DateTime lDate)
		{
			TimeSpan ts = gDate.Date - lDate.Date;
			return Fix(ts.TotalDays);
		}

		public static long DateDiffByHour(
			DateTime gDate,
			DateTime lDate)
		{
			TimeSpan ts = gDate - lDate;
			return Fix(ts.TotalHours);
		}

		public static long DateDiffByMinute(
			DateTime gDate,
			DateTime lDate)
		{
			TimeSpan ts = gDate - lDate;
			return Fix(ts.TotalMinutes);
		}

		public static long DateDiffBySeconds(
			DateTime gDate,
			DateTime lDate)
		{
			TimeSpan ts = gDate - lDate;
			return Fix(ts.TotalSeconds);
		}

		/// <summary>
		/// Dates difference by days
		/// </summary>
		/// <returns>The difference in days as integer</returns>
		public static Int32 DateDiffByDayAsInt(DateTime gDate, DateTime lDate)
		{
			long days = DateTimeUtilities.DateDiffByDay(gDate, lDate);
			if (days >= int.MinValue && days <= int.MaxValue)
			{
				return (Int32)days;
			}
			else
				throw new OverflowException("DateDiffByDays");
		}


		public Int32 DayDiff(DateTime startDate, DateTime endDate)
		{
			return endDate.Subtract(startDate).Days;
		}

		public static Int32 AbsDayDiff(DateTime endDate, DateTime startDate)
		{
			return Math.Abs(endDate.Subtract(startDate).Days);
		}


		public static String GetFormattedTimeSpan(TimeSpan timeSpan)
		{
			String strTimeSpan = timeSpan.Days > 0 ? timeSpan.Days.ToString("00") + "." : String.Empty;
			strTimeSpan +=
				timeSpan.Hours.ToString("00") + ":" +
				timeSpan.Minutes.ToString("00") + ":" +
				timeSpan.Seconds.ToString("00");

			if (timeSpan.Days == 0 &&
				timeSpan.Hours == 0 &&
				timeSpan.Minutes == 0 &&
				timeSpan.Seconds == 0)
				strTimeSpan += "." + timeSpan.Milliseconds.ToString("000");

			return strTimeSpan;
		}

		public static DateTime StartOfMonth(DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, 1);
		}

		public static DateTime EndOfMonth(DateTime dateTime)
		{
			return EndOfMonth(dateTime.Year, dateTime.Month);
		}

		public static DateTime EndOfMonth(Int32 year, Int32 month)
		{
			return new DateTime(year, month, DateTime.DaysInMonth(year, month));
		}

		public static DateTime EndOfYear(DateTime dateTime)
		{
			return new DateTime(dateTime.Year, 12, 31);
		}

		public static Int32 GetMonthDifference(
			DateTime startDate,
			DateTime endDate)
		{
			return
				(endDate.Year - startDate.Year - 1) * 12 +
				(12 - startDate.Month) + endDate.Month;
		}

		public static DateTime EndOfWeek(DateTime dateTime)
		{
			DateTime start = StartOfWeek(dateTime);
			return start.AddDays(6);
		}

		public static DateTime StartOfWeek(DateTime dateTime)
		{
			Int32 days = dateTime.DayOfWeek - DayOfWeek.Monday;
			if (days < 0)
				days += 7;
			return dateTime.AddDays(-1 * days).Date;
		}

		public static Int32 TotalMonths(Int32 years, Int32 months)
		{
			if (years < 0)
				throw new ArgumentException("years < 0", "years");
			if (months < 0)
				throw new ArgumentException("months < 0", "months");
			return years * 12 + months;
		}

		#region Solar functions

		public static DateTime FindSolarQuarterEndDate(DateTime startingDate, int nQuarters)
		{
			DateTime lastQDay = LastDayOfMonthPeriod(startingDate, 3); //LastDayOfQuarter
			return IncMonthCheckingEnd(lastQDay, 3 * (nQuarters - 1));
		}

		public static DateTime FindSolarHalfEndDate(DateTime startingDate, int nHalf)
		{
			DateTime lastHalfDay = LastDayOfMonthPeriod(startingDate, 6); //LastDayOfHalf
			return IncMonthCheckingEnd(lastHalfDay, 6 * (nHalf - 1));
		}

		public static DateTime FindSolarYearEndDate(DateTime startingDate, int nYears)
		{
			DateTime lastHalfDay = LastDayOfMonthPeriod(startingDate, 12); //LastDayOfHalf
			return IncMonthCheckingEnd(lastHalfDay, 12 * (nYears - 1));
		}

		#endregion Solar functions

		/// <summary>
		/// Gets total days from yyMMDD with formula years * 360 + months * 30 + days;
		/// </summary>
		/// <param name="years">years</param>
		/// <param name="months">months</param>
		/// <param name="days">days</param>
		/// <returns>total days</returns>
		public static Int32 GetDays360(Int32 years, Int32 months, Int32 days)
		{
			return years * 360 + months * 30 + days;
		}

		/// <summary>
		/// Gets total days from yyMMDD with formula years * 365 + months * 30 + days;
		/// </summary>
		/// <param name="years">years</param>
		/// <param name="months">months</param>
		/// <param name="days">days</param>
		/// <returns>total days</returns>
		public static Int32 GetDays365(Int32 years, Int32 months, Int32 days)
		{
			return years * 365 + months * 30 + days;
		}

		public static bool IsMaxOrMinDate(DateTime date)
		{
			return date.Equals(DateTime.MinValue) ||
				date.Equals(DateTime.MaxValue);
		}

		#endregion Public methods
	}
}