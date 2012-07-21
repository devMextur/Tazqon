using System;
using System.IO;

namespace Tazqon.Commons
{
    class IOStreamer
    {
        /// <summary>
        /// Gets or sets the title of the console.
        /// </summary>
        public string Title
        {
            get { return Console.Title; }
            set { Console.Title = value; }
        }

        /// <summary>
        /// An stream that replaces the console's output member.
        /// </summary>
        public TextWriter Output { get; private set; }

        /// <summary>
        /// An stream that replaces the console's input member.
        /// </summary>
        public TextReader Input { get; private set; }

        /// <summary>
        /// Gets the default color of the stream's.
        /// </summary>
        public ConsoleColor DefaultForecolor { get; private set; }

        /// <summary>
        /// Gets or sets the next color the stream will use.
        /// </summary>
        private ConsoleColor MirorForecolor { get; set; }

        /// <summary>
        /// Avtivates when the user is going to see another color.
        /// </summary>
        private bool Painting { get; set; }

        public IOStreamer()
        {
            this.Output = TextWriter.Synchronized(Console.Out);
            this.Input = TextReader.Synchronized(Console.In);

            this.DefaultForecolor = ConsoleColor.DarkGray;
            this.Painting = false;
        }

        #region Writing
        /// <summary>
        /// Writes a line on the console using double-writting.
        /// </summary>
        /// <param name="String">Line to write with char-parameters</param>
        /// <param name="Params">Parameters for the line.</param>
        public void AppendLine(string String, params object[] Params)
        {
            {
                Console.ForegroundColor = ConsoleColor.White;
                this.Output.Write(" ({0}) ", DateTime.Now);
            }

            this.Append(String, Params);
        }

        /// <summary>
        /// Writes a line on the console using single-writting.
        /// </summary>
        /// <param name="String">Line to write with char-parameters</param>
        /// <param name="Params">Parameters for the line.</param>
        public void Append(string String, params object[] Params)
        {
            {
                if (Painting)
                {
                    Console.ForegroundColor = MirorForecolor;
                    this.Painting = false;
                }
                else
                {
                    Console.ForegroundColor = DefaultForecolor;
                }

                this.Output.WriteLine(String, Params);
            }
        }

        /// <summary>
        /// Set another color to the stream to write the next line.
        /// </summary>
        /// <param name="Color">Color to write the next line.</param>
        public void AppendColor(ConsoleColor Color)
        {
            this.MirorForecolor = Color;
            this.Painting = true;
        }
        #endregion

        #region Reading
        /// <summary>
        /// Returns  the current line of the input stream.
        /// </summary>
        /// <returns>Current line of input steam.</returns>
        public string PopLine()
        {
            return this.Input.ReadLine();
        }

        /// <summary>
        /// Returns the next character what will be typed in.
        /// </summary>
        /// <returns>The next character what will be typed in.</returns>
        public char PopChar()
        {
            return (char)this.Input.Read();
        }

        /// <summary>
        /// Returns an dymanic string that will read till application closes.
        /// </summary>
        /// <returns>An dymanic string that will read till application closes.</returns>
        public string PopToEnd()
        {
            return this.Input.ReadToEnd();
        }
        #endregion
    }
}
