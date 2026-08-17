using System;
using System.Reflection;
using DOL.Events;

namespace DOL.GS.GameEvents
{
	public static class AchievementTracker
	{
		private static readonly Logging.Logger Log =
			Logging.LoggerManager.Create(MethodBase.GetCurrentMethod().DeclaringType);

		[GameServerStartedEvent]
		public static void OnServerStart(DOLEvent e, object sender, EventArgs arguments)
		{
			AchievementMgr.Initialize();
			GameEventMgr.AddHandler(GamePlayerEvent.LevelUp, OnLevelUp);
			GameEventMgr.AddHandler(GamePlayerEvent.GameEntered, OnGameEntered);

			if (Log.IsInfoEnabled)
				Log.Info("Achievement tracker initialized.");
		}

		[GameServerStoppedEvent]
		public static void OnServerStop(DOLEvent e, object sender, EventArgs arguments)
		{
			GameEventMgr.RemoveHandler(GamePlayerEvent.LevelUp, OnLevelUp);
			GameEventMgr.RemoveHandler(GamePlayerEvent.GameEntered, OnGameEntered);
		}

		private static void OnLevelUp(DOLEvent e, object sender, EventArgs args)
		{
			if (sender is GamePlayer player)
				AchievementMgr.CheckLevelAchievements(player);
		}

		private static void OnGameEntered(DOLEvent e, object sender, EventArgs args)
		{
			if (sender is GamePlayer player)
				AchievementMgr.CheckLevelAchievements(player);
		}
	}
}
