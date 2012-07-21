using System;
using System.IO;
using System.Net;
using Tazqon.Commons;
using Tazqon.Commons.Adapters;
using Tazqon.Commons.Storage;
using Tazqon.Habbo;
using Tazqon.Network;
using Tazqon.Packets;
using Tazqon.Storage;

namespace Tazqon
{
    static class System
    {
        /// <summary>
        /// Gets the console-wrapper created by the application.
        /// </summary>
        public static IOStreamer IOStreamer { get; private set; }

        /// <summary>
        /// Gets the Configration with settings fro the settings file.
        /// </summary>
        public static Configuration Configuration { get; private set; }

        /// <summary>
        /// Gets the Handler of incoming connections.
        /// </summary>
        public static NetworkSocket NetworkSocket { get; private set; }

        /// <summary>
        /// Get the Handler of out environment.
        /// </summary>
        public static Garbage Garbage { get; private set; }

        /// <summary>
        /// Handler of all incoming & outgoing bytes.
        /// </summary>
        public static PacketManager PacketManager { get; private set; }

        /// <summary>
        /// Handler of our database.
        /// </summary>
        public static MySQLManager MySQLManager { get; private set; }

        /// <summary>
        /// Everything with the habbo-style.
        /// </summary>
        public static HabboSystem HabboSystem { get; private set; }

        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            IOStreamer = new IOStreamer {Title = "Project Tazqon - Revision 63A" };
            IOStreamer.Append(string.Empty);
            IOStreamer.AppendColor(ConsoleColor.DarkGreen);
            IOStreamer.Append("       ____            _           _     _____                          ");
            IOStreamer.AppendColor(ConsoleColor.DarkGreen);
            IOStreamer.Append("      |  _ \\ _ __ ___ (_) ___  ___| |_  |_   _|_ _ ______ _  ___  _ __  ");
            IOStreamer.AppendColor(ConsoleColor.DarkGreen);
            IOStreamer.Append("      | |_) | '__/ _ \\| |/ _ \\/ __| __|   | |/ _` |_  / _` |/ _ \\| '_ \\ ");
            IOStreamer.AppendColor(ConsoleColor.DarkGreen);
            IOStreamer.Append("      |  __/| | | (_) | |  __/ (__| |_    | | (_| |/ / (_| | (_) | | | |");
            IOStreamer.AppendColor(ConsoleColor.DarkGreen);
            IOStreamer.Append("      |_|   |_|  \\___// |\\___|\\___|\\__|   |_|\\__,_/___\\__, |\\___/|_| |_|");
            IOStreamer.AppendColor(ConsoleColor.DarkGreen);
            IOStreamer.Append("                    |__/                                 |_|    ");  
            IOStreamer.Append(string.Empty);
            IOStreamer.AppendColor(ConsoleColor.Yellow);
            IOStreamer.Append("   Project Tazqon :: Revision 63A :: [Mextur] :: [*-2-2012] :: [© Copyright]");
            IOStreamer.AppendColor(ConsoleColor.DarkGray);
            IOStreamer.Append("   _________________________________________________________________________");
            IOStreamer.Append(string.Empty);

            Configuration = new Configuration("Configuration.XML");

            NetworkSocket = new NetworkSocket(new IPEndPoint(IPAddress.Parse(Configuration.PopString("Sessions.Network.IP")), Configuration.PopInt32("Sessions.Network.Port")));
            NetworkSocket.Serialize();

            MySQLManager = new MySQLManager();
            HabboSystem = new HabboSystem();
            Configuration.CheckSettings();

            Garbage = new Garbage();
            PacketManager = new PacketManager();

            IOStreamer.Append(string.Empty);
            IOStreamer.AppendColor(ConsoleColor.Green);
            IOStreamer.AppendLine("Environment loaded successfully, players can now login!");
            IOStreamer.Append(string.Empty);

            Console.Beep();

            IOStreamer.PopLine();
        }

        /// <summary>
        /// Handles every error what the environment catches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StreamWriter FileStream = File.Exists("Exceptions.txt") ? new StreamWriter("Exceptions.txt") : File.AppendText("Exceptions.txt");

            using (StreamWriter Stream = FileStream)
            {
                Stream.WriteLine(e.ExceptionObject.ToString());
                Stream.WriteLine(Environment.NewLine);
            }

            IOStreamer.AppendColor(ConsoleColor.Red);
            IOStreamer.AppendLine(e.ExceptionObject.ToString());


            IOStreamer.PopLine();
        }

        /// <summary>
        /// Returns and integer of the Unix-time.
        /// </summary>
        /// <returns></returns>
        public static string GenerateUnixTime()
        {
            using (DateTimeAdapter Adapter = new DateTimeAdapter(DateTime.Now))
            {
                return Adapter.PopUnixTimestamp();
            }
        }
    }
}