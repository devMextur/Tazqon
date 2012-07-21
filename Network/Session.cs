using System;
using System.Net.Sockets;
using Tazqon.Packets.Messages;
using Tazqon.Habbo.Characters;

namespace Tazqon.Network
{
    class Session
    {
        /// <summary>
        /// Integer to number this class.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Socket for traffic.
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// EventArgs for storing etc.
        /// </summary>
        public SocketAsyncEventArgs EventArgs { get; private set; }

        /// <summary>
        /// What did the session did.
        /// </summary>
        public ProgressType SessionProgress { get; private set; }

        /// <summary>
        /// Characterinformation of this ticket.
        /// </summary>
        public Character Character { get; private set; }

        /// <summary>
        /// Result of LatencyPing.
        /// </summary>
        public int LatencyPing { get; set; }

        /// <summary>
        /// Result of FPS.
        /// </summary>
        public int FramesPerSecond { get; set; }

        /// <summary>
        /// Sets the ProgressType to OnStack.
        /// </summary>
        public void Activate()
        {
            this.SessionProgress = ProgressType.OnStack;
        }

        /// <summary>
        /// Sets everything you need.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Socket"></param>
        /// <param name="EventArgs"></param>
        public void Update(int Id, Socket Socket, SocketAsyncEventArgs EventArgs)
        {
            this.Id = Id;
            this.Socket = Socket;
            this.EventArgs = EventArgs;
            this.SessionProgress = ProgressType.Connecting;
        }

        /// <summary>
        /// Updates the Sessions PING.
        /// </summary>
        /// <param name="LatencyPing"></param>
        public void UpdateLatencyPing(int LatencyPing)
        {
            this.LatencyPing = LatencyPing;
        }

        /// <summary>
        /// Updates the Sessions FPS.
        /// </summary>
        /// <param name="FramesPerSecond"></param>
        public void UpdateFramesPerSecond(int FramesPerSecond)
        {
            this.FramesPerSecond = FramesPerSecond;
        }

        /// <summary>
        /// Updates the session-progess.
        /// </summary>
        /// <param name="SessionProgress"></param>
        public void UpdateSessionProgress(ProgressType SessionProgress)
        {
            this.SessionProgress = SessionProgress;
        }

        /// <summary>
        /// Updates the character.
        /// </summary>
        /// <param name="Character"></param>
        public void UpdateCharacter(Character Character)
        {
            this.Character = Character;
            this.UpdateSessionProgress(ProgressType.Authenticated);
            this.Character.OnLogin();
        }

        /// <summary>
        /// Clears the class to be reused by another client.
        /// </summary>
        public void Clear()
        {
            this.Id = default(int);
            this.Socket = null;

            if (this.SessionProgress == ProgressType.Authenticated)
            {
                this.Character.OnLogoutWhileOffline();
            }

            this.SessionProgress = ProgressType.OnStack;
            this.Character = null;
            this.LatencyPing = default(int);
            this.FramesPerSecond = default(int);
        }

        /// <summary>
        /// Does the suppress stuff.
        /// </summary>
        public void Suppress()
        {
            if (this.SessionProgress == ProgressType.Authenticated)
            {
                this.Character.OnLogoutWhileOnline();
            }
        }

        /// <summary>
        /// Writes an buffer to the session.
        /// </summary>
        /// <param name="Composer"></param>
        public void WriteComposer(PacketComposer Composer)
        {
            if (Socket == null)
            {
                return;
            }

            try
            {
                var Buffer = Composer.GetOutput();
                var Result = default(SocketError);

                Socket.BeginSend(Buffer, 0, Buffer.Length, SocketFlags.None, out Result, new AsyncCallback(FinishSend), Socket);
            }
            catch (Exception e)
            {
                System.IOStreamer.AppendColor(ConsoleColor.Red);
                System.IOStreamer.AppendLine("[WriteComposer] {0}", e.Message);
            }
        }

        public void FinishSend(IAsyncResult Result)
        {
            if (!Result.IsCompleted)
            {
                // > FAIL
            }
        }
    }

    enum ProgressType
    {
        OnStack,
        Connecting,
        PolicyRequest,
        Authenticated
    }
}
