using System;
using System.Reflection;
using DOL.Events;

namespace DOL.GS.Scripts
{
	/// <summary>
	/// Spawns capital hub teleporters and dungeon masters when the server starts.
	/// </summary>
	public static class PvETeleporterSpawn
	{
		private const string WorldTeleporterName = "World Teleporter";
		private const string DungeonMasterName = "Dungeon Master";

		private static readonly Logging.Logger Log =
			Logging.LoggerManager.Create(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly struct SpawnPoint
		{
			public SpawnPoint(eRealm realm, ushort regionId, int worldX, int worldY, int worldZ, ushort heading,
				int dungeonMasterOffsetX)
			{
				Realm = realm;
				RegionId = regionId;
				WorldX = worldX;
				WorldY = worldY;
				WorldZ = worldZ;
				Heading = heading;
				DungeonMasterOffsetX = dungeonMasterOffsetX;
			}

			public eRealm Realm { get; }
			public ushort RegionId { get; }
			public int WorldX { get; }
			public int WorldY { get; }
			public int WorldZ { get; }
			public ushort Heading { get; }
			public int DungeonMasterOffsetX { get; }
		}

		private static readonly SpawnPoint[] CapitalSpawns =
		{
			new(eRealm.Albion, 10, 36780, 29580, 7970, 2048, 350),
			new(eRealm.Midgard, 101, 32380, 27280, 8800, 2048, 350),
			new(eRealm.Hibernia, 201, 25432, 23623, 7999, 204, 350),
		};

		[GameServerStartedEvent]
		public static void OnServerStart(DOLEvent e, object sender, EventArgs arguments)
		{
			if (!ServerProperties.Properties.PVE_TELEPORTERS_ENABLED)
			{
				if (Log.IsInfoEnabled)
					Log.Info("PvE capital teleporters are disabled (pve_teleporters_enabled = false).");
				return;
			}

			foreach (SpawnPoint spawn in CapitalSpawns)
			{
				SpawnNpcIfMissing(new PvEWorldTeleporter
				{
					Realm = spawn.Realm,
					CurrentRegionID = spawn.RegionId,
					X = spawn.WorldX,
					Y = spawn.WorldY,
					Z = spawn.WorldZ,
					Heading = spawn.Heading
				}, WorldTeleporterName, spawn.RegionId, spawn.Realm);

				SpawnNpcIfMissing(new PvEDungeonMaster
				{
					Realm = spawn.Realm,
					CurrentRegionID = spawn.RegionId,
					X = spawn.WorldX + spawn.DungeonMasterOffsetX,
					Y = spawn.WorldY,
					Z = spawn.WorldZ,
					Heading = spawn.Heading
				}, DungeonMasterName, spawn.RegionId, spawn.Realm);
			}

			if (Log.IsInfoEnabled)
				Log.Info("PvE capital teleporters and dungeon masters spawned in Camelot, Jordheim, and Tir na Nog.");
		}

		private static void SpawnNpcIfMissing(GameNPC npc, string name, ushort regionId, eRealm realm)
		{
			GameNPC[] existing = WorldMgr.GetNPCsByNameFromRegion(name, regionId, realm);
			if (existing != null && existing.Length > 0)
				return;

			if (!npc.AddToWorld())
			{
				Log.Warn($"Failed to spawn {name} in region {regionId} for realm {realm}.");
				return;
			}

			if (Log.IsDebugEnabled)
				Log.Debug($"Spawned {name} in region {regionId} at ({npc.X}, {npc.Y}, {npc.Z}).");
		}
	}
}
