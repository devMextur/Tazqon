using System;
using System.Globalization;

namespace Tazqon.Commons.Adapters
{
    class DateTimeAdapter : IDisposable
    {
        /// <summary>
        /// UnixTimestamp of the item.
        /// </summary>
        public string UnixTimestamp { get; private set; }

        /// <summary>
        /// DateTime of the item.
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Value of working.
        /// </summary>
        public bool DoneTimestamp { get; private set; }

        /// <summary>
        /// Value of working.
        /// </summary>
        public bool DoneDateTime { get; private set; }

        public DateTimeAdapter(string UnixTimestamp)
        {
            this.UnixTimestamp = UnixTimestamp;
        }

        public DateTimeAdapter(DateTime DateTime)
        {
            this.DateTime = DateTime;
        }

        /// <summary>
        /// Returns and Datetime of the item.
        /// </summary>
        /// <returns></returns>
        public DateTime PopDateTime()
        {
            if (!DoneDateTime)
            {
                DateTime Generator = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                try
                {
                    DateTime = Generator.AddSeconds(int.Parse(UnixTimestamp));
                }
                catch { DateTime = DateTime.Now; }

                DoneDateTime = true;
            }

            return DateTime;
        }

        /// <summary>
        /// Returns an Unix-Timestamp of the item.
        /// </summary>
        /// <returns></returns>
        public string PopUnixTimestamp()
        {
            if (!DoneTimestamp)
            {
                try
                {
                    UnixTimestamp = Convert.ToInt32((DateTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString(CultureInfo.InvariantCulture);
                }
                catch { UnixTimestamp = Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString(CultureInfo.InvariantCulture); }

                DoneTimestamp = true;
            }

            return UnixTimestamp;
        }

        /// <summary>
        /// Returns an Alternative stamp
        /// </summary>
        /// <returns></returns>
        public string GetAlternativestamp()
        {
            var MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(PopDateTime().Month);

            return string.Format("{0} {1} {2} {3}", PopDateTime().Day, MonthName, PopDateTime().Year, DateTime.ToShortTimeString());
        }

        public void Dispose() { }
    }
}