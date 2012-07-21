using System;
using System.Collections.Generic;
using System.Data;
using Tazqon.Commons.Adapters;
using Tazqon.Habbo.Achievements;
using Tazqon.Habbo.Messenger;
using Tazqon.Habbo.Rooms;
using Tazqon.Network;
using Tazqon.Packets.Composers;
using Tazqon.Storage.Querys;

namespace Tazqon.Habbo.Characters
{
    class Character
    {
        /// <summary>
        /// Id of the character.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Username of the character.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Motto of the character.
        /// </summary>
        public string Motto { get; private set; }

        /// <summary>
        /// Figure of the character.
        /// </summary>
        public string Figure { get; private set; }

        /// <summary>
        /// Registered in timestamp of the character.
        /// </summary>
        public string Registered { get; private set; }

        /// <summary>
        /// Last signed in timestamp of the character.
        /// </summary>
        public string LastSeen { get; private set; }

        /// <summary>
        /// Gender of the character.
        /// </summary>
        public Gender Gender { get; private set; }

        /// <summary>
        /// Muted value of the character.
        /// </summary>
        public bool Muted { get; private set; }

        /// <summary>
        /// Muted value of the character.
        /// </summary>
        public bool AllowNewFriends { get; private set; }

        /// <summary>
        /// Rank of the character.
        /// </summary>
        public CharacterRank Rank { get; private set; }

        /// <summary>
        /// Credits of the character.
        /// </summary>
        public int Credits { get; private set; }

        /// <summary>
        /// ActivityPoints of the character.
        /// </summary>
        public int ActivityPoints { get; private set; }

        /// <summary>
        /// SoundSettings of the character.
        /// </summary>
        public int SoundSettings { get; private set; }

        /// <summary>
        /// TimeOnline of the character.
        /// </summary>
        public int TimeOnline { get; private set; }

        /// <summary>
        /// Home Room of the character.
        /// </summary>
        public int HomeRoom { get; private set; }

        /// <summary>
        /// Respect of the character.
        /// </summary>
        public int RespectEarned { get; private set; }

        /// <summary>
        /// Respect giveb of the character.
        /// </summary>
        public int RespectGiven { get; private set; }

        /// <summary>
        /// Respect left for humans of the character.
        /// </summary>
        public int RespectLeftHuman { get; private set; }

        /// <summary>
        /// Respect left for animals of the character.
        /// </summary>
        public int RespectLeftAnimal { get; private set; }

        /// <summary>
        /// Current loading room.
        /// </summary>
        public int LoadingRoom { get; private set; }

        /// <summary>
        /// Current connected room.
        /// </summary>
        public int ConnectedRoom { get; private set; }

        /// <summary>
        /// Ignores of the character.
        /// </summary>
        public ICollection<int> Ignores { get; private set; }

        /// <summary>
        /// MessengerGroups of the character.
        /// </summary>
        public ICollection<MessengerGroup> MessengerGroups { get; private set; }

        /// <summary>
        /// MessengerFriends of the character.
        /// </summary>
        public ICollection<int> MessengerFriends { get; private set; }

        /// <summary>
        /// MessengerRequests of the character.
        /// </summary>
        public ICollection<int> MessengerRequests { get; private set; }

        /// <summary>
        /// Achievements of the character.
        /// </summary>
        public Dictionary<int, int> Achievements { get; private set; }

        /// <summary>
        /// Logging of authentications
        /// </summary>
        public ICollection<DateTime> Logs { get; private set; }

        /// <summary>
        /// Is character in room?
        /// </summary>
        public bool IsInRoom { get { return ConnectedRoom > 0; } }

        public Character(DataRow Row)
        {
            using (RowAdapter Adapter = new RowAdapter(Row))
            {
                this.Id = Adapter.PopInt32("id");
                this.Username = Adapter.PopString("username");
                this.Motto = Adapter.PopString("motto");
                this.Figure = Adapter.PopString("figure");
                this.Registered = Adapter.PopString("registered_stamp");
                this.LastSeen = Adapter.PopString("last_seen");
                this.Gender = Adapter.PopEnum<Gender>("gender");
                this.Muted = Adapter.PopBoolean("muted");
                this.AllowNewFriends = Adapter.PopBoolean("allow_new_friends");
                this.Rank = System.HabboSystem.CharacterManager.GetRank(Adapter.PopInt32("rank"));
                this.Credits = Adapter.PopInt32("credits");
                this.ActivityPoints = Adapter.PopInt32("activity_points");
                this.SoundSettings = Adapter.PopInt32("sound_settings");
                this.TimeOnline = Adapter.PopInt32("time_online");
                this.HomeRoom = Adapter.PopInt32("home_room");
                this.RespectEarned = Adapter.PopInt32("respect_earned");
                this.RespectGiven = Adapter.PopInt32("respect_given");
                this.RespectLeftHuman = Adapter.PopInt32("respect_left_human");
                this.RespectLeftAnimal = Adapter.PopInt32("respect_left_animal");

                this.LoadingRoom = 0;
                this.ConnectedRoom = 0;
            }
        }

        #region Events
        /// <summary>
        /// Action called when logging in.
        /// </summary>
        public void OnLogin()
        {
            switch (Rank.Id)
            {
                case 1:
                    System.IOStreamer.AppendColor(ConsoleColor.Magenta);
                    break;
                case 2:
                case 3:
                    System.IOStreamer.AppendColor(ConsoleColor.Blue);
                    break;
                case 4:
                case 5:
                    System.IOStreamer.AppendColor(ConsoleColor.Yellow);
                    break;
            }

            System.IOStreamer.AppendLine("{0} {1} signed in.", Rank.Caption, Username);

            System.HabboSystem.MessengerManager.ParseStatus(Id);

            System.MySQLManager.InvokeQuery(new CharacterLogAddQuery(Id));

            if (System.HabboSystem.CharacterManager.WeakSQLCache.ContainsKey(Id))
            {
                System.HabboSystem.CharacterManager.WeakSQLCache.Remove(Id);
            }
        }

        /// <summary>
        /// Action called when logging out.
        /// </summary>
        public void OnLogoutWhileOffline()
        {
            switch (Rank.Id)
            {
                case 1:
                    System.IOStreamer.AppendColor(ConsoleColor.Magenta);
                    break;
                case 2:
                case 3:
                    System.IOStreamer.AppendColor(ConsoleColor.Blue);
                    break;
                case 4:
                case 5:
                    System.IOStreamer.AppendColor(ConsoleColor.Yellow);
                    break;
            }

            System.IOStreamer.AppendLine("{0} {1} signed out.", Rank.Caption, Username);

            System.HabboSystem.MessengerManager.ParseStatus(Id);

            System.MySQLManager.InvokeQuery(new SetCharacterLastSeenQuery(Id));

            /// Update memory for the offline users to show.
            if (!System.HabboSystem.CharacterManager.WeakSQLCache.ContainsKey(Id))
            {
                System.HabboSystem.CharacterManager.WeakSQLCache.Add(Id, this);
            }
            else System.HabboSystem.CharacterManager.WeakSQLCache[Id] = this;
        }

        /// <summary>
        /// Atcion called when logging out.
        /// </summary>
        public void OnLogoutWhileOnline()
        {
            if (IsInRoom)
            {
                RoomAdapter Adapter;

                if (System.HabboSystem.CharacterManager.GetCurrentRoom(Id, out Adapter))
                {
                    RoomUnit Unit;

                    if (Adapter.GetUnit(Id, out Unit))
                    {
                        if (Adapter.RemovePlayerByUnitId(Unit.Id))
                        {
                            Adapter.WriteComposer(new UserRemoveMessageComposer(Unit.Id));
                        }
                    }
                }
            }
        }
        #endregion

        #region Information Handling
        /// <summary>
        /// Updates the ignores of the user.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateIgnores(ICollection<int> Obj)
        {
            //if (Ignores == null)
            this.Ignores = Obj;
        }

        /// <summary>
        /// Updates the messengergroups of the user.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateMessengerGroups(ICollection<MessengerGroup> Obj)
        {
            //if (MessengerGroups == null)
            this.MessengerGroups = Obj;
        }

        /// <summary>
        /// Updates the messengerfriends of the user.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateMessengerFriends(ICollection<int> Obj)
        {
           // if (MessengerFriends == null)
            this.MessengerFriends = Obj;
        }

        /// <summary>
        /// Updates the messengerrequests of the user.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateMessengerRequests(ICollection<int> Obj)
        {
            // if (MessengerRequests == null)
            this.MessengerRequests = Obj;
        }

        /// <summary>
        /// Updates the achievements of the user.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateAchievements(Dictionary<int, int> Obj)
        {
            //if (Achievements === null)
            this.Achievements = Obj;
        }

        /// <summary>
        /// Updates the logs of the user.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateLogs(ICollection<DateTime> Obj)
        {
            //if (Achievements === null)
            this.Logs = Obj;
        }

        /// <summary>
        /// Updates the activity-points in-game.
        /// </summary>
        public void UpdateActivityPoints(bool InDatabase = false)
        {
            Session Session = System.NetworkSocket.GetSession(Id);

            if (Session == null)
            {
                return;
            }

            Session.WriteComposer(new HabboActivityPointNotificationMessageComposer(ActivityPoints));

            if (!InDatabase)
            {
                return;
            }

            System.MySQLManager.InvokeQuery(new CharacterActivityPointsQuery(Id, ActivityPoints));
        }

        /// <summary>
        /// Updates the TimeOnline.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateTimeOnline(int Obj)
        {
            this.TimeOnline += Obj;
        }

        /// <summary>
        /// Updates the current loading room.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateLoadingRoom(int Obj)
        {
            if (IsInRoom && Obj > 0)
            {
                RoomAdapter Adapter;

                if (System.HabboSystem.CharacterManager.GetCurrentRoom(Id, out Adapter))
                {
                    RoomUnit Unit;

                    if (Adapter.GetUnit(Id, out Unit))
                    {
                        if (Adapter.RemovePlayerByUnitId(Unit.Id))
                        {
                            Adapter.WriteComposer(new UserRemoveMessageComposer(Unit.Id));
                        }
                    }
                }
            }

            this.LoadingRoom = Obj;
        }

        /// <summary>
        /// Updates the current connected room.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateConnectedRoom(int Obj)
        {
            this.ConnectedRoom = Obj;
        }

        /// <summary>
        /// Updates the current soudns settings.
        /// </summary>
        /// <param name="Obj"></param>
        public void UpdateSoundSettings(int Obj)
        {
            this.SoundSettings = Obj;

            System.MySQLManager.InvokeQuery(new CharacterVolumeQuery(Id, Obj));
        }
        #endregion

        /// <summary>
        /// Returns the progress of an achievement
        /// </summary>
        /// <param name="AchievementId"></param>
        /// <returns></returns>
        public int GetAchievementProgress(int AchievementId)
        {
            using (DictionaryAdapter<int, int> DA = new DictionaryAdapter<int, int>(Achievements))
            {
                return DA.TryPopValue(AchievementId);
            }
        }

        /// <summary>
        /// Returns the progress-limit of an achievement.
        /// </summary>
        /// <param name="AchievementId"></param>
        /// <returns></returns>
        public int GetAchievementProgessLimit(int AchievementId)
        {
            switch (AchievementId)
            {
                default:
                    return default(int);
                case 2:
                    return System.HabboSystem.CharacterManager.GetDaysInARow(Logs);
                case 3: // RegistrationDuration
                    using (DateTimeAdapter Adapter = new DateTimeAdapter(Registered))
                    {
                        return (int)(DateTime.Now - Adapter.PopDateTime()).TotalDays;
                    }
                case 5 :
                    return TimeOnline;
                case 6:
                    return (GetAchievementProgessLimit(5) >= 60 && GetAchievementProgessLimit(3) >= 3) ? 1 : 0;
                case 10: // HappyHour
                    return ((DateTime.Now.Hour >= 14 && DateTime.Now.Hour <= 15) || Achievements.ContainsKey(10)) ? 1 : 0;
                case 11:
                    return RespectGiven;
                    // TODO more achievements
            }
        }

        /// <summary>
        /// Sends update data to the character.
        /// </summary>
        /// <param name="AchievementId"></param>
        public void ComposeAchievement(int AchievementId)
        {            
            Achievement Achievement = System.HabboSystem.AchievementManager.GetAchievement(AchievementId);

            if (Achievement == null)
            {
                return; // Invalid move.
            }

            Session Session = System.NetworkSocket.GetSession(Id);

            if (Session == null)
            {
                return; // Invalid move.
            }

            Session.WriteComposer(new AchievementComposer(this, Achievement));

            CheckAchievement(AchievementId);
        }

        /// <summary>
        /// Checks the achievement if need to unlock.
        /// </summary>
        /// <param name="AchievementId"></param>
        public void CheckAchievement(int AchievementId)
        {
            Achievement Achievement = System.HabboSystem.AchievementManager.GetAchievement(AchievementId);

            if (Achievement == null)
            {
                return; // Invalid move.
            }

            int CurrentLevel = GetAchievementProgress(AchievementId);

            if (CurrentLevel >= Achievement.Levels)
            {
                return; // Maxed out. 
            }

            if (GetAchievementProgessLimit(AchievementId) < Achievement.GetRequired(CurrentLevel + 1))
            {
                return; // Not enough progress.
            }

            Session Session = System.NetworkSocket.GetSession(Id);

            if (Session == null)
            {
                return; // Invalid move.
            }

            if (Achievements.ContainsKey(AchievementId))
            {
                Achievements[AchievementId] = CurrentLevel + 1;
            }
            else
            {
                Achievements.Add(AchievementId, CurrentLevel + 1);
            }

            CurrentLevel = GetAchievementProgress(AchievementId);

            this.ActivityPoints += Achievement.GetPixelReward(CurrentLevel);

            Session.WriteComposer(new HabboAchievementNotificationMessageComposer(GetAchievementProgress(AchievementId), Achievement));
            Session.WriteComposer(new AchievementsScoreComposer(System.HabboSystem.AchievementManager.GetAchievementScore(this)));

            System.MySQLManager.InvokeQuery(new AchievementCharacterProgressQuery(Id, AchievementId, CurrentLevel));

            this.UpdateActivityPoints(true);
        }
    }

    enum Gender
    {
         M, F
    }

    enum Membership
    {
        Default, Club, Vip
    }
}
