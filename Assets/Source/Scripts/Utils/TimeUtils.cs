using System;
using UnityEngine;

namespace Utility
{
    public class TimeUtils
    {
        #region Static Fields

        public static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static TimeSpan ServerDeltaTime = new TimeSpan();
        public static GetTimeMethod CurrentGetTimeMethod = GetNow;

        public static int DeltaTime = 0; //for cheats

        #endregion

        #region Delegates

        public delegate DateTime GetTimeMethod();

        #endregion

        #region Public Methods and Operators

        public static DateTime ClearTime(DateTime time)
        {
            time = time.AddHours(-time.Hour);
            time = time.AddMinutes(-time.Minute);
            time = time.AddSeconds(-time.Second);
            return time;
        }

        public static DateTime GetDateTime(int timestamp)
        {
            return Epoch.AddSeconds(Convert.ToDouble(timestamp)).ToLocalTime();
        }

        public static DateTime GetDateTime(double timestamp)
        {
            return Epoch.AddSeconds(timestamp);
        }


        public static DateTime GetDateTimeUTC(int timestamp)
        {
            return Epoch.AddSeconds(Convert.ToDouble(timestamp)).ToUniversalTime();
        }

        public static float GetDelta(double timestamp)
        {
            return Convert.ToSingle((UtcNow() - Epoch).TotalSeconds - timestamp + DeltaTime);
        }

        //not used
        public static DateTime GetNowWithAnticheat()
        {
            return DateTime.Now;
        }


        public static int GetServerTimestamp(string type = "s")
        {
            if (type == "ms")
                return Convert.ToInt32((UtcNow() + ServerDeltaTime - Epoch).TotalMilliseconds) + DeltaTime * 1000;
            return Convert.ToInt32((UtcNow() + ServerDeltaTime - Epoch).TotalSeconds) + DeltaTime;
        }

        public static long GetServerTimestampLong(string type = "s")
        {
            if (type == "ms")
                return Convert.ToInt64((UtcNow() + ServerDeltaTime - Epoch).TotalMilliseconds) + (long) DeltaTime * 1000;
            return Convert.ToInt64((UtcNow() + ServerDeltaTime - Epoch).TotalSeconds) + DeltaTime;
        }

        public static int GetTimestamp(string type = "s")
        {
            if (type == "ms")
                return Convert.ToInt32((UtcNow() - Epoch).TotalMilliseconds) + DeltaTime * 1000;
            return Convert.ToInt32((UtcNow() - Epoch).TotalSeconds) + DeltaTime;
        }

        public static int GetTimestamp(DateTime date)
        {
            return Convert.ToInt32((date.ToUniversalTime() - Epoch).TotalSeconds) + DeltaTime;
        }

        public static double GetTimestampDouble(string type = "s")
        {
            if (type == "ms")
                return Convert.ToDouble((UtcNow() - Epoch).TotalMilliseconds) + (double) DeltaTime * 1000;
            return Convert.ToDouble((UtcNow() - Epoch).TotalSeconds) + DeltaTime;
        }


        public static bool IsSameDayLocal(DateTime date0, DateTime date1)
        {
            var date0UTC = date0.ToLocalTime();
            var date1UTC = date1.ToLocalTime();
            if (date0UTC.DayOfYear != date1UTC.DayOfYear)
                return false;

            if (date0UTC.Year != date1UTC.Year)
                return false;

            return true;
        }

        public static bool IsSameDayLocal(int timestamp, DateTime date1)
        {
            return IsSameDayLocal(GetDateTime(timestamp), date1);
        }

        public static bool IsSameDayUTC(DateTime date0, DateTime date1)
        {
            var date0UTC = date0.ToUniversalTime();
            var date1UTC = date1.ToUniversalTime();
            if (date0UTC.DayOfYear != date1UTC.DayOfYear)
                return false;

            if (date0UTC.Year != date1UTC.Year)
                return false;

            return true;
        }


        public static bool IsSameDayUTC(int timestamp, DateTime date1)
        {
            return IsSameDayUTC(GetDateTimeUTC(timestamp), date1);
        }

        public static DateTime Now()
        {
            if (CurrentGetTimeMethod != null)
                return CurrentGetTimeMethod().ToLocalTime();
            return GetNow();
        }

        public static string OrdinalNumber(int num)
        {
            string suff;
            var ones = num % 10;
            var tens = Mathf.FloorToInt(num / 10) % 10;
            if (tens == 1)
                suff = "th";
            else
                switch (ones)
                {
                    case 1:
                        suff = "st";
                        break;
                    case 2:
                        suff = "nd";
                        break;
                    case 3:
                        suff = "rd";
                        break;
                    default:
                        suff = "th";
                        break;
                }
            return num + suff;
        }

        public static DateTime UtcNow()
        {
            if (CurrentGetTimeMethod != null)
                return CurrentGetTimeMethod().ToUniversalTime();
            return DateTime.UtcNow;
        }

        #endregion

        #region Methods

        private static DateTime GetNow()
        {
            return DateTime.UtcNow;
        }

        #endregion
    }
}