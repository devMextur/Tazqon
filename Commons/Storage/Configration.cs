using System.Collections.Generic;
using Tazqon.Commons.Components;

namespace Tazqon.Commons.Storage
{
    class Configuration
    {
        /// <summary>
        /// Settings read en from an XML file.
        /// </summary>
        public IDictionary<string, object> Settings;

        public Configuration(string Filename)
        {
            using (var Stream = new XMLStreamer(Filename))
            {
                this.Settings = Stream.PopItems();
            }
        }

        /// <summary>
        /// Checks the settings if they do link or not.
        /// </summary>
        public void CheckSettings()
        {
            Dictionary<string, object> ToUpdate = new Dictionary<string, object>(); 

            foreach (var kvp in Settings)
            {
                if (System.HabboSystem.CheckValueIfSQLSetting(PopString(kvp.Key)))
                {
                    ToUpdate.Add(kvp.Key,System.HabboSystem.GetMySQLSetting<object>(PopString(kvp.Key)));
                }
            }

            foreach (var kvp in ToUpdate)
            {
                this.Settings[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// Returns an value from the key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string PopString(string Key)
        {
            object specOutput;
            Settings.TryGetValue(Key, out specOutput);
            string Output = specOutput as string;
            return CheckResult<string>(Output);
        }

        /// <summary>
        /// Returns an value from the key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public int PopInt32(string Key)
        {
            int Output;
            int.TryParse(PopString(Key), out Output);
            return CheckResult<int>(Output);
        }

        /// <summary>
        /// Returns an value from the key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public uint PopUInt32(string Key)
        {
            return (uint)PopInt32(Key);
        }

        /// <summary>
        /// Returns an value from the key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool PopBoolean(string Key)
        {
            bool Output;

            bool.TryParse(PopString(Key), out Output);

            return Output;
        }

        /// <summary>
        /// Check against null reference-exceptions
        /// </summary>
        /// <param name="Result"></param>
        private T CheckResult<T>(object Result)
        {
            if (Result != null)
            {
                return (T)Result;
            }

            return default(T);
        }
    }
}
