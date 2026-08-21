using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.GS.Spells;

namespace DOL.GS.Scripts
{
	/// <summary>
	/// Teleports players to classic, epic, and shared dungeon entrances across all realms.
	/// </summary>
	public class PvEDungeonMaster : GameNPC
	{
		private readonly struct DungeonDestination
		{
			public DungeonDestination(string keyword, ushort regionId, int x, int y, int z, ushort heading)
			{
				Keyword = keyword;
				RegionId = regionId;
				X = x;
				Y = y;
				Z = z;
				Heading = heading;
			}

			public string Keyword { get; }
			public ushort RegionId { get; }
			public int X { get; }
			public int Y { get; }
			public int Z { get; }
			public ushort Heading { get; }
		}

		private static readonly DungeonDestination[] Destinations =
		{
			// Albion
			new("Tomb of Mithra", 21, 33150, 32732, 16480, 2069),
			new("Keltoi Fogou", 22, 30116, 31234, 16523, 3099),
			new("Tepok's Mine", 24, 32476, 34737, 15179, 2044),
			new("Catacombs of Cardova", 23, 31120, 29939, 16239, 3180),
			new("Stonehenge Barrows", 20, 31424, 34307, 16496, 1161),
			new("Krondon", 61, 33013, 31379, 15698, 270),
			new("Avalon City", 50, 31159, 47380, 8307, 2011),
			new("Caer Sidi", 60, 31666, 35982, 18639, 4077),

			// Midgard
			new("Nisse's Lair", 129, 34693, 33173, 16447, 1039),
			new("Cursed Tomb", 128, 30116, 31234, 16523, 3029),
			new("Vendo Caverns", 126, 32737, 33173, 16624, 2037),
			new("Varulvhamn", 127, 35342, 30885, 14992, 1060),
			new("Spindelhalla", 125, 32152, 31841, 16375, 90),
			new("Iarnvidiur's Lair", 161, 34991, 37707, 17231, 326),
			new("Trollheim", 150, 28284, 47911, 16000, 1904),
			new("Tuscaran Glacier", 160, 35612, 17899, 19050, 244),

			// Hibernia
			new("Muire Tomb", 221, 31120, 29939, 16239, 90),
			new("Spraggon Den", 222, 32722, 34761, 15179, 1918),
			new("Koalinth Caverns", 223, 27318, 32267, 17266, 3064),
			new("Treibh Caillte", 224, 35393, 30865, 14990, 1036),
			new("Coruscating Mine", 220, 33534, 33660, 16049, 1063),
			new("Tur Suil", 190, 38038, 27675, 13247, 180),
			new("Fomor", 180, 33801, 24157, 16084, 611),
			new("Galladoria", 191, 32072, 29524, 17051, 184),

			// Catacombs frontier dungeons
			new("Marfach Caverns", 276, 32915, 29238, 16268, 2038),
			new("Hall of the Corrupt", 277, 32395, 31847, 16158, 180),
			new("Doden's Gruva", 246, 32259, 30624, 16045, 91),

			// Shared
			new("Darkness Falls Albion", 249, 31211, 27924, 22893, 3072),
			new("Darkness Falls Midgard", 249, 18798, 18667, 22892, 1022),
			new("Darkness Falls Hibernia", 249, 46325, 40969, 21357, 2045),
		};

		private static readonly Dictionary<string, DungeonDestination> DestinationMap =
			new(StringComparer.OrdinalIgnoreCase);

		static PvEDungeonMaster()
		{
			foreach (DungeonDestination destination in Destinations)
				DestinationMap[destination.Keyword] = destination;

			DestinationMap["tuscaren glacier"] = DestinationMap["tuscaran glacier"];
		}

		public override bool AddToWorld()
		{
			Name = "Dungeon Master";
			GuildName = "Travel Hub";

			switch (Realm)
			{
				case eRealm.Albion:
					Model = 61;
					GameNpcInventoryTemplate templateAlb = new GameNpcInventoryTemplate();
					templateAlb.AddNPCEquipment(eInventorySlot.Cloak, 57, 66);
					templateAlb.AddNPCEquipment(eInventorySlot.TorsoArmor, 1005, 86);
					templateAlb.AddNPCEquipment(eInventorySlot.LegsArmor, 140, 6);
					templateAlb.AddNPCEquipment(eInventorySlot.ArmsArmor, 141, 6);
					templateAlb.AddNPCEquipment(eInventorySlot.HandsArmor, 142, 6);
					templateAlb.AddNPCEquipment(eInventorySlot.FeetArmor, 143, 6);
					templateAlb.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 1166);
					Inventory = templateAlb.CloseTemplate();
					break;
				case eRealm.Midgard:
					Model = 215;
					GameNpcInventoryTemplate templateMid = new GameNpcInventoryTemplate();
					templateMid.AddNPCEquipment(eInventorySlot.Cloak, 57, 26);
					templateMid.AddNPCEquipment(eInventorySlot.TorsoArmor, 245, 26);
					templateMid.AddNPCEquipment(eInventorySlot.LegsArmor, 246, 26);
					templateMid.AddNPCEquipment(eInventorySlot.HandsArmor, 248, 26);
					templateMid.AddNPCEquipment(eInventorySlot.FeetArmor, 249, 26);
					Inventory = templateMid.CloseTemplate();
					break;
				case eRealm.Hibernia:
					Model = 342;
					GameNpcInventoryTemplate templateHib = new GameNpcInventoryTemplate();
					templateHib.AddNPCEquipment(eInventorySlot.TorsoArmor, 1008);
					templateHib.AddNPCEquipment(eInventorySlot.HandsArmor, 396);
					templateHib.AddNPCEquipment(eInventorySlot.FeetArmor, 402);
					templateHib.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 468);
					Inventory = templateHib.CloseTemplate();
					break;
			}

			Level = 60;
			Size = 50;
			Flags |= eFlags.PEACE;
			return base.AddToWorld();
		}

		public override bool ShowTeleporterIndicator => true;

		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player) || GameRelic.IsPlayerCarryingRelic(player))
				return false;

			TurnTo(player, 10000);
			SayTo(player, BuildMenu());
			return true;
		}

		public override bool WhisperReceive(GameLiving source, string str)
		{
			if (!base.WhisperReceive(source, str))
				return false;

			GamePlayer player = source as GamePlayer;
			if (player == null)
				return false;

			if (GameRelic.IsPlayerCarryingRelic(player))
				return false;

			if (str.Equals("dungeons", StringComparison.OrdinalIgnoreCase) ||
			    str.Equals("menu", StringComparison.OrdinalIgnoreCase) ||
			    str.Equals("world teleporter", StringComparison.OrdinalIgnoreCase))
			{
				if (str.Equals("world teleporter", StringComparison.OrdinalIgnoreCase))
				{
					SayTo(player, "The World Teleporter stands beside me and can send you anywhere in your realm.");
					return false;
				}

				SayTo(player, BuildMenu());
				return false;
			}

			if (str.Equals("darkness falls", StringComparison.OrdinalIgnoreCase) ||
			    str.Equals("df", StringComparison.OrdinalIgnoreCase))
			{
				switch (player.Realm)
				{
					case eRealm.Albion:
						str = "Darkness Falls Albion";
						break;
					case eRealm.Midgard:
						str = "Darkness Falls Midgard";
						break;
					case eRealm.Hibernia:
						str = "Darkness Falls Hibernia";
						break;
				}
			}

			if (!DestinationMap.TryGetValue(str, out DungeonDestination destination))
			{
				SayTo(player, "I don't recognize that dungeon. Say [dungeons] to see the full list.");
				return false;
			}

			TeleportPlayer(player, destination);
			return false;
		}

		private static string BuildMenu()
		{
			return "I can send you to any dungeon in the realms. Whisper a destination:\n\n" +
			       "Albion:\n[Tomb of Mithra] [Keltoi Fogou] [Tepok's Mine] [Catacombs of Cardova]\n" +
			       "[Stonehenge Barrows] [Krondon] [Avalon City] [Caer Sidi]\n\n" +
			       "Midgard:\n[Nisse's Lair] [Cursed Tomb] [Vendo Caverns] [Varulvhamn]\n" +
			       "[Spindelhalla] [Iarnvidiur's Lair] [Trollheim] [Tuscaran Glacier]\n\n" +
			       "Hibernia:\n[Muire Tomb] [Spraggon Den] [Koalinth Caverns] [Treibh Caillte]\n" +
			       "[Coruscating Mine] [Tur Suil] [Fomor] [Galladoria]\n\n" +
			       "Catacombs:\n[Marfach Caverns] [Hall of the Corrupt] [Doden's Gruva]\n\n" +
			       "Shared:\n[Darkness Falls] (your realm's entrance)\n" +
			       "[Darkness Falls Albion] [Darkness Falls Midgard] [Darkness Falls Hibernia]\n\n" +
			       "Need a town or frontier port? Speak with the [World Teleporter] nearby.";
		}

		private void TeleportPlayer(GamePlayer player, DungeonDestination destination)
		{
			Region region = WorldMgr.GetRegion(destination.RegionId);
			if (region == null || region.IsDisabled)
			{
				player.Out.SendMessage("This destination is not available.", eChatType.CT_System,
					eChatLoc.CL_SystemWindow);
				return;
			}

			if (player.InCombat || GameRelic.IsPlayerCarryingRelic(player))
			{
				SayTo(player, "You cannot travel while in combat or carrying a relic.");
				return;
			}

			SayTo(player, $"I'm now teleporting you to {destination.Keyword}.");
			player.LeaveHouse();

			SpellLine spellLine = SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells);
			Spell spell = SkillBase.GetSpellByID(5999);
			if (spell != null)
			{
				DbTeleport teleport = new()
				{
					TeleportID = destination.Keyword,
					Realm = (int)player.Realm,
					RegionID = destination.RegionId,
					X = destination.X,
					Y = destination.Y,
					Z = destination.Z,
					Heading = destination.Heading
				};

				UniPortal portalHandler = new UniPortal(this, spell, spellLine, teleport);
				portalHandler.StartSpell(player);
				return;
			}

			GameLocation currentLocation =
				new GameLocation("TeleportStart", player.CurrentRegionID, player.X, player.Y, player.Z);
			DbTeleport fallback = new()
			{
				TeleportID = destination.Keyword,
				Realm = (int)player.Realm,
				RegionID = destination.RegionId,
				X = destination.X,
				Y = destination.Y,
				Z = destination.Z,
				Heading = destination.Heading
			};
			player.MoveTo(destination.RegionId, destination.X, destination.Y, destination.Z, destination.Heading);
			GameServer.ServerRules.OnPlayerTeleport(player, currentLocation, fallback);
		}
	}
}
