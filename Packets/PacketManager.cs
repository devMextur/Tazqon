using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tazqon.Commons.Adapters;
using Tazqon.Packets.Interfaces;
using Tazqon.Packets.Composers;
using Tazqon.Packets.Messages;
using Tazqon.Network;
using System.IO;

namespace Tazqon.Packets
{
    class PacketManager
    {
        /// <summary>
        /// Storage of names from Info Events.
        /// </summary>
        public Dictionary<string, short> InfoEvents;

        /// <summary>
        /// Storage of Invokers from Message Events
        /// </summary>
        public Dictionary<short, IMessageEvent> MessageEvents;

        public PacketManager()
        {
            InfoEvents = new Dictionary<string, short>();

            foreach (var Packet in typeof(Headers.MessageEventIds).GetFields())
            {
                var PacketId = (short)Packet.GetValue(0);
                var PacketName = Packet.Name;

                if (!InfoEvents.ContainsValue(PacketId))
                {
                    InfoEvents.Add(PacketName, PacketId);
                }
            }

            MessageEvents = new Dictionary<short, IMessageEvent>();

            foreach (Type Type in Assembly.GetCallingAssembly().GetTypes())
            {
                if (Type == null)
                {
                    continue; 
                }

                if (Type.GetInterfaces().Contains(typeof(IMessageEvent)))
                {
                    var ConstructorInfo = Type.GetConstructor(new Type[] {});

                    if (ConstructorInfo != null)
                    {
                        var Constructed = ConstructorInfo.Invoke(new object[] { }) as IMessageEvent;

                        short Header = GetHeader(Constructed);

                        if (!MessageEvents.ContainsKey(Header) && Header > 0)
                        {
                            MessageEvents.Add(Header, Constructed);
                        }
                    }
                }
            }

            int ComposerIds = typeof(Headers.MessageComposerIds).GetFields().Count();

            System.IOStreamer.AppendLine("Loaded {0}/{1} MessageEvent(s), and {2} MessageComposer(s).", MessageEvents.Count, InfoEvents.Count, ComposerIds);
        }

        /// <summary>
        /// Returns an header from an InfoEvent.
        /// </summary>
        /// <param name="Event"></param>
        /// <returns></returns>
        public short GetHeader(IMessageEvent Event)
        {
            using (DictionaryAdapter<string, short> DA = new DictionaryAdapter<string, short>(InfoEvents))
            {
                return DA.TryPopValue(Event.GetType().Name);
            }
        }

        /// <summary>
        /// Returns an name of an InfoEvent.
        /// </summary>
        /// <param name="Header"></param>
        /// <returns></returns>
        public string GetName(short Header)
        {
            using (DictionaryAdapter<string, short> DA = new DictionaryAdapter<string, short>(InfoEvents))
            {
                return DA.TryPopKey(Header);
            }
        }

        /// <summary>
        /// Handles the bytes from an Session.
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Bytes"></param>
        public void ProcessBytes(Session Session, ref byte[] Bytes)
        {
            try
            {
                if (Bytes[0] == 64)
                {
                    using (BinaryReader ReaderStream = new BinaryReader(new MemoryStream(Bytes)))
                    {
                        for (int i = 0; i < Bytes.Length; )
                        {
                            PacketEvent Stream = new PacketEvent();
                            int Length;

                            if (Stream.TryGetInfo(ref Bytes, ReaderStream, out Length))
                            {
                                i += Length;

                                if (MessageEvents.ContainsKey(Stream.Id))
                                {
                                    IMessageEvent Event = MessageEvents[Stream.Id];

                                    if (Session != null)
                                    {
                                        Event.Invoke(Session, Stream);

                                        if (System.Configuration.PopBoolean("Packets.Events.Logging"))
                                        {
                                            System.IOStreamer.AppendColor(ConsoleColor.Green);
                                            System.IOStreamer.AppendLine("({2}) ({0}) {1}", Stream.Id,
                                                                         GetName(Stream.Id), Session.Id);
                                        }
                                    }
                                }
                                else if (System.Configuration.PopBoolean("Packets.Events.Logging") && Session != null)
                                {
                                    System.IOStreamer.AppendColor(ConsoleColor.Red);
                                    System.IOStreamer.AppendLine("({2}) ({0}) {1}", Stream.Id, GetName(Stream.Id), Session.Id);
                                }
                            }
                        }
                    }
                }
                else if (Bytes[0] == 60)
                {
                    Session.WriteComposer(new PolicyFileRequestComposer("<?xml version=\"1.0\"?>\r\n" +
                       "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                       "<cross-domain-policy>\r\n" +
                       "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                       "</cross-domain-policy>\x0"));
                }
            }
            catch (Exception e)
            {
                System.IOStreamer.AppendColor(ConsoleColor.Red);
                System.IOStreamer.AppendLine("[ProcessBytes] {0}", e.ToString());
            }
        }
    }
}
