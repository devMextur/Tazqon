namespace Tazqon.Habbo.Messenger
{
    class MessengerUpdate
    {
        /// <summary>
        /// Id of the updater
        /// </summary>
        public int UpdateId { get; set; }

        /// <summary>
        /// Id of the target
        /// </summary>
        public int TargetId { get; set; }

        /// <summary>
        /// Mode of the updaterequest.
        /// </summary>
        public MessengerUpdateType Mode { get; set; }
    }

    enum MessengerUpdateType
    {
        RemoveBuddy = -1,
        EnterOrLeave = 0,
        RoomChange = 0,
        NewFriendship = 1,
    }
}
