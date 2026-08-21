using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Housing;
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts
{
	/// <summary>
	/// Capital hub teleporter with a single flat menu covering all standard realm destinations.
	/// </summary>
	public class PvEWorldTeleporter : LiveTeleporter
	{
		public override bool AddToWorld()
		{
			if (!base.AddToWorld())
				return false;

			Name = "World Teleporter";
			GuildName = "Travel Hub";
			return true;
		}

		public override bool Interact(GamePlayer player)
		{
			if (!PrepareInteract(player))
				return false;

			SayTo(player, BuildFlatMenu(player));
			return true;
		}

		private static string BuildFlatMenu(GamePlayer player)
		{
			switch (Realm)
			{
				case eRealm.Albion:
					return "Greetings, " + player.Name + ". Whisper any destination below to travel:\n\n" +
					       "Frontiers:\n[Castle Sauvage] [Snowdonia Fortress] [Avalon Marsh]\n\n" +
					       "Shrouded Isles:\n[Gothwaite Harbor] [Gothwaite] [Wearyall Village] " +
					       "[Fort Gwyntell] [Caer Diogel]\n\n" +
					       "Capital:\n[Camelot]\n\n" +
					       "Housing:\n[Entrance] [Personal] [Guild] [Hearth]\n\n" +
					       "Towns:\n[Cotswold Village] [Prydwen Keep] [Caer Ulfwych] " +
					       "[Campacorentin Station] [Adribard's Retreat] [Yarley's Farm]\n\n" +
					       "Epic:\n[Caer Sidi]\n\n" +
					       "For classic and epic dungeons, speak with the [Dungeon Master] nearby.";
				case eRealm.Midgard:
					return "Greetings, " + player.Name + ". Whisper any destination below to travel:\n\n" +
					       "Frontiers:\n[Svasud Faste] [Vindsaul Faste] [Gotar]\n\n" +
					       "Shrouded Isles:\n[Aegirhamn] [Bjarken] [Hagall] [Knarr]\n\n" +
					       "Capital:\n[Jordheim]\n\n" +
					       "Housing:\n[Entrance] [Personal] [Guild] [Hearth]\n\n" +
					       "Towns:\n[Mularn] [Fort Veldon] [Audliten] [Huginfell] " +
					       "[Fort Atla] [West Skona]\n\n" +
					       "Epic:\n[Tuscaran Glacier]\n\n" +
					       "For classic and epic dungeons, speak with the [Dungeon Master] nearby.";
				case eRealm.Hibernia:
					return "Greetings, " + player.Name + ". Whisper any destination below to travel:\n\n" +
					       "Frontiers:\n[Druim Ligen] [Druim Cain] [Shannon Estuary]\n\n" +
					       "Shrouded Isles:\n[Domnann] [Droighaid] [Aalid Feie] [Necht]\n\n" +
					       "Capital:\n[Tir na Nog]\n\n" +
					       "Housing:\n[Entrance] [Personal] [Guild] [Hearth]\n\n" +
					       "Towns:\n[Mag Mell] [Tir na mBeo] [Ardagh] [Howth] " +
					       "[Connla] [Innis Carthaig]\n\n" +
					       "Epic:\n[Galladoria]\n\n" +
					       "For classic and epic dungeons, speak with the [Dungeon Master] nearby.";
				default:
					return "I have no realm set, so I cannot offer any destinations.";
			}
		}

		protected override bool OnWhisperTeleport(GamePlayer player, string text)
		{
			if (text.Equals("dungeon master", StringComparison.OrdinalIgnoreCase))
			{
				SayTo(player, "The Dungeon Master stands beside me and can send you to any dungeon in Albion, Midgard, or Hibernia.");
				return false;
			}

			if (text.Equals("world teleporter", StringComparison.OrdinalIgnoreCase))
			{
				SayTo(player, BuildFlatMenu(player));
				return false;
			}

			return GetTeleportLocation(player, NormalizeKeyword(text));
		}

		protected override bool GetTeleportLocation(GamePlayer player, string text)
		{
			text = NormalizeKeyword(text);

			if (text.Equals("towns", StringComparison.OrdinalIgnoreCase) ||
			    text.Equals("housing", StringComparison.OrdinalIgnoreCase) ||
			    text.Equals("shrouded isles", StringComparison.OrdinalIgnoreCase))
			{
				SayTo(player, BuildFlatMenu(player));
				return false;
			}

			if (text == "Entrance")
				text = text.ToLower();

			if (text.Equals("personal", StringComparison.OrdinalIgnoreCase))
			{
				House house = HouseMgr.GetHouseByPlayer(player);

				if (house == null)
				{
					text = "entrance";
				}
				else
				{
					IGameLocation location = house.OutdoorJumpPoint;
					DbTeleport teleport = new DbTeleport();
					teleport.TeleportID = "your house";
					teleport.Realm = (int)DestinationRealm;
					teleport.RegionID = location.RegionID;
					teleport.X = location.X;
					teleport.Y = location.Y;
					teleport.Z = location.Z;
					teleport.Heading = location.Heading;
					OnDestinationPicked(player, teleport);
					return false;
				}
			}

			if (text.Equals("hearth", StringComparison.OrdinalIgnoreCase))
			{
				if (!(player.BindHouseRegion > 0))
				{
					SayTo(player, "Sorry, you haven't set any house bind point yet.");
					return false;
				}

				var houses = HouseMgr.GetHousesCloseToSpot((ushort)player.BindHouseRegion,
					player.BindHouseXpos, player.BindHouseYpos, 700);
				if (houses.Count == 0)
				{
					SayTo(player, "I'm afraid I can't teleport you to your hearth since the house at your " +
					              "house bind location has been torn down.");
					return false;
				}

				House targetHouse = houses[0];
				var hookpointItems = targetHouse.HousePointItems;
				bool hasBindstone = false;

				foreach (KeyValuePair<uint, DbHouseHookPointItem> targetHouseItem in hookpointItems)
				{
					if (((GameObject)targetHouseItem.Value.GameObject).GetName(0, false).ToLower()
						    .EndsWith("bindstone"))
					{
						hasBindstone = true;
						break;
					}
				}

				if (!hasBindstone)
				{
					SayTo(player, "I'm sorry to tell that the bindstone of your current house bind location " +
					              "has been removed, so I'm not able to teleport you there.");
					return false;
				}

				if (!targetHouse.CanBindInHouse(player))
				{
					SayTo(player, "You're no longer allowed to bind at the house bindstone you've previously " +
					              "chosen, hence I'm not allowed to teleport you there.");
					return false;
				}

				DbTeleport teleport = new DbTeleport();
				teleport.TeleportID = "hearth";
				teleport.Realm = (int)DestinationRealm;
				teleport.RegionID = player.BindHouseRegion;
				teleport.X = player.BindHouseXpos;
				teleport.Y = player.BindHouseYpos;
				teleport.Z = player.BindHouseZpos;
				teleport.Heading = player.BindHouseHeading;
				OnDestinationPicked(player, teleport);
				return false;
			}

			if (text.Equals("guild", StringComparison.OrdinalIgnoreCase))
			{
				House house = HouseMgr.GetGuildHouseByPlayer(player);

				if (house == null)
				{
					SayTo(player, player.Guild != null
						? $"I'm sorry but {player.Guild.Name} doesn't own a Guild House."
						: "You are not in a guild that owns a guild house.");
					return false;
				}

				IGameLocation location = house.OutdoorJumpPoint;
				DbTeleport teleport = new DbTeleport();
				teleport.TeleportID = "guild house";
				teleport.Realm = (int)DestinationRealm;
				teleport.RegionID = location.RegionID;
				teleport.X = location.X;
				teleport.Y = location.Y;
				teleport.Z = location.Z;
				teleport.Heading = location.Heading;
				OnDestinationPicked(player, teleport);
				return false;
			}

			DbTeleport port = WorldMgr.GetTeleportLocation(DestinationRealm, string.Format("{0}:{1}", Type, text));
			if (port != null)
			{
				if (port.RegionID == 0 && port.X == 0 && port.Y == 0 && port.Z == 0)
				{
					SayTo(player, BuildFlatMenu(player));
				}
				else
				{
					OnDestinationPicked(player, port);
				}

				return false;
			}

			return true;
		}

		private static string NormalizeKeyword(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return text;

			switch (text.ToLower())
			{
				case "fort gwyntell":
				case "gwyntell":
					return "Fort Gwyntell";
				case "caer diogel":
				case "diogel":
					return "Caer Diogel";
				case "gothwaite harbor":
					return "Gothwaite Harbor";
				case "grove of domnann":
					return "Domnann";
				case "tir na mbeo":
					return "Tir na mBeo";
				case "tir na nog":
					return "Tir na Nog";
				case "spindelhalla":
					return "Spindelhalla";
				default:
					return text;
			}
		}
	}
}
