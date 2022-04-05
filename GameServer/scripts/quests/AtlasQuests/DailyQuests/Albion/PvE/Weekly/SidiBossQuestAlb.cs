using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.API;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;
using DOL.GS.PlayerTitles;
using DOL.GS.Quests;
using log4net;

namespace DOL.GS.DailyQuest.Albion
{
    public class SidiBossQuestAlb : WeeklyQuest
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string questTitle = "[Weekly] Harder Adversaries";
        private const int minimumLevel = 45;
        private const int maximumLevel = 50;

        // Kill Goal
        private int _deadSidiBossMob = 0;
        private const int MAX_KILLGOAL = 3;

        private static GameNPC Hector = null; // Start NPC


        // Constructors
        public SidiBossQuestAlb() : base()
        {
        }

        public SidiBossQuestAlb(GamePlayer questingPlayer) : base(questingPlayer, 1)
        {
        }

        public SidiBossQuestAlb(GamePlayer questingPlayer, int step) : base(questingPlayer, step)
        {
        }

        public SidiBossQuestAlb(GamePlayer questingPlayer, DBQuest dbQuest) : base(questingPlayer, dbQuest)
        {
        }

        public override int Level
        {
            get
            {
                // Quest Level
                return minimumLevel;
            }
        }

        [ScriptLoadedEvent]
        public static void ScriptLoaded(DOLEvent e, object sender, EventArgs args)
        {
            if (!ServerProperties.Properties.LOAD_QUESTS)
                return;


            #region defineNPCs

            GameNPC[] npcs = WorldMgr.GetNPCsByName("Hector", eRealm.Albion);

            if (npcs.Length > 0)
                foreach (GameNPC npc in npcs)
                    if (npc.CurrentRegionID == 1 && npc.X == 583860 && npc.Y == 477619)
                    {
                        Hector = npc;
                        break;
                    }

            if (Hector == null)
            {
                if (log.IsWarnEnabled)
                    log.Warn("Could not find Hector , creating it ...");
                Hector = new GameNPC();
                Hector.Model = 716;
                Hector.Name = "Hector";
                Hector.GuildName = "Advisor to the King";
                Hector.Realm = eRealm.Albion;
                Hector.CurrentRegionID = 1;
                Hector.Size = 50;
                Hector.Level = 59;
                //Castle Sauvage Location
                Hector.X = 583860;
                Hector.Y = 477619;
                Hector.Z = 2600;
                Hector.Heading = 3111;
                Hector.AddToWorld();
                if (SAVE_INTO_DATABASE)
                {
                    Hector.SaveIntoDatabase();
                }
            }

            #endregion

            #region defineItems

            #endregion

            #region defineObject

            #endregion

            GameEventMgr.AddHandler(GamePlayerEvent.AcceptQuest, new DOLEventHandler(SubscribeQuest));
            GameEventMgr.AddHandler(GamePlayerEvent.DeclineQuest, new DOLEventHandler(SubscribeQuest));

            GameEventMgr.AddHandler(Hector, GameObjectEvent.Interact, new DOLEventHandler(TalkToHector));
            GameEventMgr.AddHandler(Hector, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToHector));

            /* Now we bring to Haszan the possibility to give this quest to players */
            Hector.AddQuestToGive(typeof(SidiBossQuestAlb));

            if (log.IsInfoEnabled)
                log.Info("Quest \"" + questTitle + "\" Alb initialized");
        }

        [ScriptUnloadedEvent]
        public static void ScriptUnloaded(DOLEvent e, object sender, EventArgs args)
        {
            //if not loaded, don't worry
            if (Hector == null)
                return;
            // remove handlers
            GameEventMgr.RemoveHandler(GamePlayerEvent.AcceptQuest, new DOLEventHandler(SubscribeQuest));
            GameEventMgr.RemoveHandler(GamePlayerEvent.DeclineQuest, new DOLEventHandler(SubscribeQuest));

            GameEventMgr.RemoveHandler(Hector, GameObjectEvent.Interact, new DOLEventHandler(TalkToHector));
            GameEventMgr.RemoveHandler(Hector, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToHector));

            /* Now we remove to Haszan the possibility to give this quest to players */
            Hector.RemoveQuestToGive(typeof(SidiBossQuestAlb));
        }

        protected static void TalkToHector(DOLEvent e, object sender, EventArgs args)
        {
            //We get the player from the event arguments and check if he qualifies		
            GamePlayer player = ((SourceEventArgs) args).Source as GamePlayer;
            if (player == null)
                return;

            if (Hector.CanGiveQuest(typeof(SidiBossQuestAlb), player) <= 0)
                return;

            //We also check if the player is already doing the quest
            SidiBossQuestAlb quest = player.IsDoingQuest(typeof(SidiBossQuestAlb)) as SidiBossQuestAlb;

            if (e == GameObjectEvent.Interact)
            {
                if (quest != null)
                {
                    switch (quest.Step)
                    {
                        case 1:
                            Hector.SayTo(player,
                                "Please, enter Caer Sidi and slay strong opponents. If you succeed come back for your reward.");
                            break;
                        case 2:
                            Hector.SayTo(player, "Hello " + player.Name + ", did you [succeed]?");
                            break;
                    }
                }
                else
                {
                    Hector.SayTo(player, "Hello " + player.Name + ", I am Hector. " +
                                         "An infiltrator has reported the forces in Caer Sidi are planning an attack. \n" +
                                         "We want to pre-empt them and [end their plotting] before they have the chance. Care to help?");
                }
            }
            // The player whispered to the NPC
            else if (e == GameLivingEvent.WhisperReceive)
            {
                WhisperReceiveEventArgs wArgs = (WhisperReceiveEventArgs) args;
                if (quest == null)
                {
                    switch (wArgs.Text)
                    {
                        case "end their plotting":
                            player.Out.SendQuestSubscribeCommand(Hector,
                                QuestMgr.GetIDForQuestType(typeof(SidiBossQuestAlb)),
                                "Will you help Dean " + questTitle + "");
                            break;
                    }
                }
                else
                {
                    switch (wArgs.Text)
                    {
                        case "succeed":
                            if (quest.Step == 2)
                            {
                                player.Out.SendMessage("Thank you for your contribution!", eChatType.CT_Chat,
                                    eChatLoc.CL_PopupWindow);
                                quest.FinishQuest();
                            }

                            break;
                        case "abort":
                            player.Out.SendCustomDialog(
                                "Do you really want to abort this quest, \nall items gained during quest will be lost?",
                                new CustomDialogResponse(CheckPlayerAbortQuest));
                            break;
                    }
                }
            }
        }

        public override bool CheckQuestQualification(GamePlayer player)
        {
            // if the player is already doing the quest his level is no longer of relevance
            if (player.IsDoingQuest(typeof(SidiBossQuestAlb)) != null)
                return true;

            // This checks below are only performed is player isn't doing quest already

            //if (player.HasFinishedQuest(typeof(Academy_47)) == 0) return false;

            //if (!CheckPartAccessible(player,typeof(CityOfCamelot)))
            //	return false;

            if (player.Level < minimumLevel || player.Level > maximumLevel)
                return false;

            return true;
        }

        private static void CheckPlayerAbortQuest(GamePlayer player, byte response)
        {
            SidiBossQuestAlb quest = player.IsDoingQuest(typeof(SidiBossQuestAlb)) as SidiBossQuestAlb;

            if (quest == null)
                return;

            if (response == 0x00)
            {
                SendSystemMessage(player, "Good, now go out there and finish your work!");
            }
            else
            {
                SendSystemMessage(player, "Aborting Quest " + questTitle + ". You can start over again if you want.");
                quest.AbortQuest();
            }
        }

        protected static void SubscribeQuest(DOLEvent e, object sender, EventArgs args)
        {
            QuestEventArgs qargs = args as QuestEventArgs;
            if (qargs == null)
                return;

            if (qargs.QuestID != QuestMgr.GetIDForQuestType(typeof(SidiBossQuestAlb)))
                return;

            if (e == GamePlayerEvent.AcceptQuest)
                CheckPlayerAcceptQuest(qargs.Player, 0x01);
            else if (e == GamePlayerEvent.DeclineQuest)
                CheckPlayerAcceptQuest(qargs.Player, 0x00);
        }

        private static void CheckPlayerAcceptQuest(GamePlayer player, byte response)
        {
            if (Hector.CanGiveQuest(typeof(SidiBossQuestAlb), player) <= 0)
                return;

            if (player.IsDoingQuest(typeof(SidiBossQuestAlb)) != null)
                return;

            if (response == 0x00)
            {
                player.Out.SendMessage("Thank you for helping Albion.", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
            }
            else
            {
                //Check if we can add the quest!
                if (!Hector.GiveQuest(typeof(SidiBossQuestAlb), player, 1))
                    return;

                Hector.SayTo(player, "Thank you " + player.Name + ", be an enrichment for our realm!");
            }
        }

        //Set quest name
        public override string Name
        {
            get { return questTitle; }
        }

        // Define Steps
        public override string Description
        {
            get
            {
                switch (Step)
                {
                    case 1:
                        return "Find a way to Caer Sidi and kill strong opponents. \nKilled: Bosses in Caer Sidi (" +
                               _deadSidiBossMob + " | "+ MAX_KILLGOAL +")";
                    case 2:
                        return "Return to Hector for your Reward.";
                }

                return base.Description;
            }
        }

        public override void Notify(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = sender as GamePlayer;

            if (player == null || player.IsDoingQuest(typeof(SidiBossQuestAlb)) == null)
                return;

            if (sender != m_questPlayer)
                return;

            if (Step != 1 || e != GameLivingEvent.EnemyKilled) return;
            EnemyKilledEventArgs gArgs = (EnemyKilledEventArgs) args;
                
            // check if a GameEpicBoss died + if its in Caer Sidi
            if (gArgs.Target.Realm != 0 || gArgs.Target is not GameEpicBoss ||
                gArgs.Target.CurrentRegionID != 60) return;
            _deadSidiBossMob++;
            player.Out.SendMessage(
                "[Weekly] Bosses killed in Caer Sidi: (" + _deadSidiBossMob + " | " + MAX_KILLGOAL + ")",
                eChatType.CT_ScreenCenter, eChatLoc.CL_SystemWindow);
            player.Out.SendQuestUpdate(this);

            if (_deadSidiBossMob >= MAX_KILLGOAL)
            {
                // FinishQuest or go back to Haszan
                Step = 2;
            }
        }

        public override string QuestPropertyKey
        {
            get => "SidiBossQuestAlb";
            set { ; }
        }

        public override void LoadQuestParameters()
        {
            _deadSidiBossMob = GetCustomProperty(QuestPropertyKey) != null ? int.Parse(GetCustomProperty(QuestPropertyKey)) : 0;
        }

        public override void SaveQuestParameters()
        {
            SetCustomProperty(QuestPropertyKey, _deadSidiBossMob.ToString());
        }

        public override void FinishQuest()
        {
            m_questPlayer.GainExperience(eXPSource.Quest, (m_questPlayer.ExperienceForNextLevel - m_questPlayer.ExperienceForCurrentLevel), false);
            m_questPlayer.AddMoney(Money.GetMoney(0, 0, m_questPlayer.Level * 5, 0, Util.Random(50)), "You receive {0} as a reward.");
            AtlasROGManager.GenerateOrbAmount(m_questPlayer, 1500);
            _deadSidiBossMob = 0;
            base.FinishQuest(); //Defined in Quest, changes the state, stores in DB etc ...
        }
    }
}