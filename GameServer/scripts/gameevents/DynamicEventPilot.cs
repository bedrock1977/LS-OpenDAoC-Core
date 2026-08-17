using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.PacketHandler;

namespace DOL.GS.GameEvents
{
	/// <summary>
	/// Minimal WAR-style public quest pilot: one circular area, one mob wave, XP reward on clear.
	/// Disabled by default via server properties.
	/// </summary>
	public static class DynamicEventPilot
	{
		private const string MobPackageId = "DynamicEventPilot";
		private const string AreaName = "dynamic event pilot";

		private static readonly Logging.Logger log =
			Logging.LoggerManager.Create(MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly Lock Sync = new();
		private static readonly HashSet<GamePlayer> Participants = new();
		private static readonly List<GameNPC> ActiveMobs = new();

		private static AbstractArea _eventArea;
		private static bool _active;
		private static int _remaining;
		private static DateTime _cooldownEndsUtc = DateTime.MinValue;

		[GameServerStartedEvent]
		public static void OnServerStart(DOLEvent e, object sender, EventArgs arguments)
		{
			if (!ServerProperties.Properties.DYNAMIC_EVENT_PILOT_ENABLED)
			{
				if (log.IsInfoEnabled)
					log.Info("Dynamic event pilot is disabled (dynamic_event_pilot_enabled = false).");
				return;
			}

			Region region = WorldMgr.GetRegion(ServerProperties.Properties.DYNAMIC_EVENT_PILOT_REGION);
			if (region == null)
			{
				log.Error($"Dynamic event pilot: region {ServerProperties.Properties.DYNAMIC_EVENT_PILOT_REGION} not found.");
				return;
			}

			_eventArea = new Area.Circle(
				AreaName,
				ServerProperties.Properties.DYNAMIC_EVENT_PILOT_X,
				ServerProperties.Properties.DYNAMIC_EVENT_PILOT_Y,
				ServerProperties.Properties.DYNAMIC_EVENT_PILOT_Z,
				ServerProperties.Properties.DYNAMIC_EVENT_PILOT_RADIUS);
			_eventArea.CanBroadcast = false;
			_eventArea.DisplayMessage = false;
			region.AddArea(_eventArea);
			_eventArea.RegisterPlayerEnter(OnPlayerEnterArea);

			if (log.IsInfoEnabled)
				log.Info(
					$"Dynamic event pilot registered in region {ServerProperties.Properties.DYNAMIC_EVENT_PILOT_REGION} " +
					$"({ServerProperties.Properties.DYNAMIC_EVENT_PILOT_X}, {ServerProperties.Properties.DYNAMIC_EVENT_PILOT_Y}, " +
					$"{ServerProperties.Properties.DYNAMIC_EVENT_PILOT_Z}), radius {ServerProperties.Properties.DYNAMIC_EVENT_PILOT_RADIUS}.");
		}

		[GameServerStoppedEvent]
		public static void OnServerStop(DOLEvent e, object sender, EventArgs arguments)
		{
			lock (Sync)
			{
				CleanupWave();
				_active = false;
				Participants.Clear();
			}

			if (_eventArea == null)
				return;

			_eventArea.UnRegisterPlayerEnter(OnPlayerEnterArea);
			Region region = WorldMgr.GetRegion(ServerProperties.Properties.DYNAMIC_EVENT_PILOT_REGION);
			region?.RemoveArea(_eventArea);
			_eventArea = null;
		}

		private static void OnPlayerEnterArea(DOLEvent e, object sender, EventArgs args)
		{
			if (args is not AreaEventArgs areaArgs)
				return;

			GamePlayer player = areaArgs.GameObject as GamePlayer;
			if (player == null || player.ObjectState != eObjectState.Active)
				return;

			lock (Sync)
			{
				if (_active)
				{
					Participants.Add(player);
					return;
				}

				if (DateTime.UtcNow < _cooldownEndsUtc)
					return;

				StartEvent(player);
			}
		}

		private static void StartEvent(GamePlayer trigger)
		{
			_active = true;
			Participants.Clear();
			AddPlayersInArea();

			int mobCount = Math.Max(1, ServerProperties.Properties.DYNAMIC_EVENT_PILOT_MOB_COUNT);
			_remaining = mobCount;

			BroadcastToArea(
				$"Restless spirits rise near {trigger.Name}! Defeat {mobCount} invaders to claim the reward.",
				eChatType.CT_System);

			for (int i = 0; i < mobCount; i++)
				SpawnMob();

			if (log.IsInfoEnabled)
				log.Info($"Dynamic event pilot started ({mobCount} mobs) near {trigger.Name}.");
		}

		private static void AddPlayersInArea()
		{
			foreach (GamePlayer player in ClientService.Instance.GetPlayers())
			{
				if (_eventArea != null && _eventArea.IsContaining(player))
					Participants.Add(player);
			}
		}

		private static void SpawnMob()
		{
			int offset = ServerProperties.Properties.DYNAMIC_EVENT_PILOT_RADIUS / 3;
			int x = ServerProperties.Properties.DYNAMIC_EVENT_PILOT_X + Util.Random(-offset, offset);
			int y = ServerProperties.Properties.DYNAMIC_EVENT_PILOT_Y + Util.Random(-offset, offset);
			int z = ServerProperties.Properties.DYNAMIC_EVENT_PILOT_Z;

			Region region = WorldMgr.GetRegion(ServerProperties.Properties.DYNAMIC_EVENT_PILOT_REGION);
			if (region == null)
				return;

			GameNPC mob = new()
			{
				Name = "Restless Spirit",
				Model = 346,
				Level = (byte)Math.Clamp(ServerProperties.Properties.DYNAMIC_EVENT_PILOT_MOB_LEVEL, 1, 255),
				Size = 50,
				X = x,
				Y = y,
				Z = z,
				CurrentRegion = region,
				Realm = eRealm.None,
				RespawnInterval = -1,
				PackageID = MobPackageId,
				MaxSpeedBase = 200
			};

			StandardMobBrain brain = new();
			mob.SetOwnBrain(brain);
			brain.AggroRange = 800;
			brain.AggroLevel = 100;

			mob.AddToWorld();
			GameEventMgr.AddHandler(mob, GameLivingEvent.Dying, OnWaveMobDied);
			ActiveMobs.Add(mob);
		}

		private static void OnWaveMobDied(DOLEvent e, object sender, EventArgs args)
		{
			if (sender is not GameNPC npc || npc.PackageID != MobPackageId)
				return;

			GameEventMgr.RemoveHandler(npc, GameLivingEvent.Dying, OnWaveMobDied);
			ActiveMobs.Remove(npc);

			if (args is DyingEventArgs dyingArgs)
			{
				if (dyingArgs.Killer is GamePlayer killer)
					Participants.Add(killer);

				if (dyingArgs.PlayerKillers != null)
				{
					foreach (GamePlayer player in dyingArgs.PlayerKillers)
						Participants.Add(player);
				}
			}

			lock (Sync)
			{
				if (!_active)
					return;

				_remaining--;
				if (_remaining > 0)
					return;

				CompleteEvent();
			}
		}

		private static void CompleteEvent()
		{
			long xp = Math.Max(0, ServerProperties.Properties.DYNAMIC_EVENT_PILOT_XP_REWARD);
			int rewarded = 0;

			foreach (GamePlayer player in Participants)
			{
				if (player == null || player.ObjectState != eObjectState.Active)
					continue;

				if (xp > 0)
					player.ForceGainExperience(xp);

				player.Out.SendMessage(
					"The restless spirits are banished. You receive your share of the event reward.",
					eChatType.CT_Important,
					eChatLoc.CL_SystemWindow);
				rewarded++;
			}

			BroadcastToArea(
				$"The dynamic event is complete! {rewarded} participant(s) rewarded.",
				eChatType.CT_System);

			_cooldownEndsUtc = DateTime.UtcNow.AddSeconds(Math.Max(0, ServerProperties.Properties.DYNAMIC_EVENT_PILOT_COOLDOWN_SECONDS));
			_active = false;
			Participants.Clear();
			_remaining = 0;

			if (log.IsInfoEnabled)
				log.Info($"Dynamic event pilot completed; rewarded {rewarded} player(s). Cooldown until {_cooldownEndsUtc:u}.");
		}

		private static void CleanupWave()
		{
			GameNPC[] mobs = ActiveMobs.ToArray();
			ActiveMobs.Clear();

			foreach (GameNPC mob in mobs)
			{
				GameEventMgr.RemoveHandler(mob, GameLivingEvent.Dying, OnWaveMobDied);
				if (mob.ObjectState == eObjectState.Active)
					mob.RemoveFromWorld();
			}
		}

		private static void BroadcastToArea(string message, eChatType chatType)
		{
			if (_eventArea == null)
				return;

			foreach (GamePlayer player in ClientService.Instance.GetPlayers())
			{
				if (!_eventArea.IsContaining(player))
					continue;

				player.Out.SendMessage(message, chatType, eChatLoc.CL_SystemWindow);
			}
		}
	}
}
