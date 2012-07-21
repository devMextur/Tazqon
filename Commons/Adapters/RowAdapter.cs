using System;
using System.Data;

namespace Tazqon.Commons.Adapters
{
    class RowAdapter : IDisposable
    {
        /// <summary>
        /// Row to get data from.
        /// </summary>
        public DataRow Row { get; private set; }

        /// <summary>
        /// Starts the streaming.
        /// </summary>
        /// <param name="Row"></param>
        public RowAdapter(DataRow Row)
        {
            this.Row = Row;
        }

        /// <summary>
        /// Tries to get an integer from row.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public int PopInt32(string Key)
        {
            int Output;
            int.TryParse(PopString(Key), out Output);
            return Output;
        }

        /// <summary>
        /// Tries to get an double from row.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public double PopDouble(string Key)
        {
            double Output;
            double.TryParse(PopString(Key), out Output);
            return Output;
        }

        /// <summary>
        /// Tries to get an string from row.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string PopString(string Key)
        {
            string Output = default(string);
            try { Output = Row[Key].ToString(); }
            catch { }
            return Output;
        }

        /// <summary>
        /// Tries to get an boolean fron row.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool PopBoolean(string Key)
        {
            return PopInt32(Key) > 0;
        }

        /// <summary>
        /// Tries to get an Enum from the row.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        public TEnum PopEnum<TEnum>(string Key)
        {
            TEnum Output = default(TEnum);

            try
            {
                Output = (TEnum)Enum.Parse(typeof(TEnum), PopString(Key));
            }
            catch { }

            return Output;
        }

        /// <summary>
        /// End of stream
        /// </summary>
        public void Dispose()
        {
            this.Row = null;
        }
    }
}