using System.Collections.Generic;

namespace Tazqon.Commons.Storage
{
    class MiniConfigration
    {
        /// <summary>
        /// Distionary to store data on.
        /// </summary>
        public IDictionary<string, object> Settings { get; private set; }

        /// <summary>
        /// Updates property
        /// </summary>
        /// <param name="Items"></param>
        public void Parse(Dictionary<string, object> Items)
        {
            this.Settings = Items;
        }

        /// <summary>
        /// Returns an value from the key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string PopString(string Key)
        {
            object Output;

            Settings.TryGetValue(Key, out Output);

            return Output as string;
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

            return Output;
        }

        /// <summary>
        /// Returns an value from the key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public uint PopUInt32(string Key)
        {
            uint Output;

            uint.TryParse(PopString(Key), out Output);

            return Output;
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
    }
}
