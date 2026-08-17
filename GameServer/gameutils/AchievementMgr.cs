using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS
{
	public static class AchievementMgr
	{
		private static readonly Logging.Logger Log =
			Logging.LoggerManager.Create(MethodBase.GetCurrentMethod().DeclaringType);

		private static Dictionary<string, DbAchievement> _definitions = new();
		private static bool _loaded;

		public static void Initialize()
		{
			_definitions = new Dictionary<string, DbAchievement>(StringComparer.OrdinalIgnoreCase);

			if (!ServerProperties.Properties.ACHIEVEMENTS_ENABLED)
			{
				if (Log.IsInfoEnabled)
					Log.Info("Achievements disabled (achievements_enabled = false).");
				return;
			}

			try
			{
				IList<DbAchievement> rows = GameServer.Database.SelectAllObjects<DbAchievement>();
				foreach (DbAchievement row in rows)
					_definitions[row.AchievementKey] = row;

				_loaded = true;

				if (Log.IsInfoEnabled)
					Log.Info($"Loaded {_definitions.Count} achievement definition(s).");
			}
			catch (Exception ex)
			{
				Log.Error("Failed to load achievements — run opendaoc-db-core/patches/achievements.sql", ex);
			}
		}

		public static bool IsEnabled => ServerProperties.Properties.ACHIEVEMENTS_ENABLED && _loaded;

		public static void CheckLevelAchievements(GamePlayer player)
		{
			if (!IsEnabled || player == null)
				return;

			foreach (DbAchievement definition in _definitions.Values.Where(d => d.MinLevel > 0))
			{
				if (player.Level >= definition.MinLevel)
					TryUnlock(player, definition.AchievementKey);
			}
		}

		public static bool TryUnlock(GamePlayer player, string achievementKey)
		{
			if (!IsEnabled || player == null || string.IsNullOrEmpty(achievementKey))
				return false;

			if (!_definitions.TryGetValue(achievementKey, out DbAchievement definition))
				return false;

			if (HasUnlock(player.QuestPlayerID, achievementKey))
				return false;

			var record = new DbCharacterAchievement
			{
				Character_ID = player.QuestPlayerID,
				AchievementKey = definition.AchievementKey,
				UnlockedAt = DateTime.UtcNow
			};

			GameServer.Database.AddObject(record);
			NotifyUnlock(player, definition);
			return true;
		}

		public static bool HasUnlock(string characterId, string achievementKey)
		{
			return DOLDB<DbCharacterAchievement>.SelectObject(
				       DB.Column("Character_ID").IsEqualTo(characterId)
					       .And(DB.Column("AchievementKey").IsEqualTo(achievementKey))) != null;
		}

		public static IList<string> BuildAchievementReport(GamePlayer player)
		{
			var lines = new List<string>();
			if (!IsEnabled)
			{
				lines.Add("Achievements are disabled on this shard.");
				return lines;
			}

			IList<DbCharacterAchievement> unlocked = DOLDB<DbCharacterAchievement>.SelectObjects(
				DB.Column("Character_ID").IsEqualTo(player.QuestPlayerID));

			var unlockedKeys = new HashSet<string>(unlocked.Select(u => u.AchievementKey), StringComparer.OrdinalIgnoreCase);
			int totalPoints = 0;

			lines.Add($"Unlocked: {unlockedKeys.Count} / {_definitions.Count}");
			lines.Add(string.Empty);

			foreach (IGrouping<string, DbAchievement> group in _definitions.Values.GroupBy(d => d.Category).OrderBy(g => g.Key))
			{
				lines.Add($"== {group.Key} ==");

				foreach (DbAchievement definition in group.OrderBy(d => d.MinLevel).ThenBy(d => d.Name))
				{
					if (definition.Hidden && !unlockedKeys.Contains(definition.AchievementKey))
					{
						lines.Add("  [???] Hidden achievement");
						continue;
					}

					if (unlockedKeys.Contains(definition.AchievementKey))
					{
						totalPoints += definition.Points;
						lines.Add($"  [X] {definition.Name} (+{definition.Points})");
						if (!string.IsNullOrWhiteSpace(definition.Description))
							lines.Add($"      {definition.Description}");
					}
					else if (definition.MinLevel > 0)
					{
						lines.Add($"  [ ] {definition.Name} — reach level {definition.MinLevel}");
					}
					else
					{
						lines.Add($"  [ ] {definition.Name}");
					}
				}

				lines.Add(string.Empty);
			}

			lines.Add($"Total achievement points: {totalPoints}");
			return lines;
		}

		private static void NotifyUnlock(GamePlayer player, DbAchievement definition)
		{
			player.Out.SendMessage(
				$"Achievement unlocked: {definition.Name} (+{definition.Points} points)!",
				eChatType.CT_Important,
				eChatLoc.CL_SystemWindow);

			if (!string.IsNullOrWhiteSpace(definition.Description))
			{
				player.Out.SendMessage(
					definition.Description,
					eChatType.CT_System,
					eChatLoc.CL_SystemWindow);
			}

			player.Out.SendMessage(
				$"[LSACH] unlock|{definition.AchievementKey}|{definition.Name}|{definition.Points}",
				eChatType.CT_System,
				eChatLoc.CL_SystemWindow);
		}
	}
}
