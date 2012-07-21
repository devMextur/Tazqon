using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Tazqon.Commons.Components
{
    class XMLStreamer : IDisposable
    {
        /// <summary>
        /// XMLFile to process.
        /// </summary>
        private string FilePath { get; set; }

        public XMLStreamer(string Path)
        {
            this.FilePath = Path;
        }

        /// <summary>
        /// Returns an IDictionary with properties.
        /// </summary>
        /// <returns>XML Properties</returns>
        public IDictionary<string, object> PopItems()
        {
            IDictionary<string, object> Out = new Dictionary<string, object>();

            foreach (var Line in File.ReadAllLines(FilePath))
            {
                if (Regex.IsMatch(Line, "XML"))
                {
                    continue;
                }

                if (Line.Trim().StartsWith("<") && Line.Trim().EndsWith(">"))
                {
                    int KeyIndex =Array.IndexOf(Line.ToCharArray(),'<') + 1;

                    string Key = Line.Substring(KeyIndex , Array.IndexOf(Line.ToArray(), '>') - KeyIndex);

                    if (Key.Contains('!'))
                    {
                        continue;
                    }

                    object Value = Line.Replace(Key, string.Empty).Split('<','>')[2];

                    Out.Add(Key, Value);
                }
            }

            return Out;
        }

        public void Dispose() {}
    }
}
