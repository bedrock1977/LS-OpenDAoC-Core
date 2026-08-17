using System.Collections.Generic;
using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&sellgreys",
		new[] { "&selljunk" },
		ePrivLevel.Player,
		"Sells low-quality backpack items to a targeted merchant",
		"/sellgreys — target a merchant first")]
	public class SellGreysCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "sellgreys"))
				return;

			GamePlayer player = client.Player;
			if (player?.Inventory == null)
				return;

			if (player.TargetObject is not GameMerchant merchant && player.TargetObject is not GameGuardMerchant guardMerchant)
			{
				player.Out.SendMessage("You must target a merchant.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			int maxQuality = ServerProperties.Properties.BULK_SELL_MAX_QUALITY;
			List<DbInventoryItem> items = player.Inventory.GetItemRange(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack);
			if (items == null || items.Count == 0)
			{
				player.Out.SendMessage("Your backpack is empty.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			int sold = 0;
			foreach (DbInventoryItem item in items.ToArray())
			{
				if (item == null || !ShouldSell(item, maxQuality))
					continue;

				if (merchant != null)
					merchant.OnPlayerSell(player, item);
				else
					guardMerchant.OnPlayerSell(player, item);

				sold++;
			}

			player.Out.SendMessage(
				sold > 0
					? $"Sold {sold} low-quality item(s) (quality {maxQuality}% or below)."
					: $"No sellable items at quality {maxQuality}% or below.",
				eChatType.CT_System,
				eChatLoc.CL_SystemWindow);
		}

		private static bool ShouldSell(DbInventoryItem item, int maxQuality)
		{
			if (item.Quality > maxQuality)
				return false;

			if (!item.IsDropable || item.IsIndestructible)
				return false;

			if (item.PackageID is "AtlasXPItem" or "atlas_orbs_item" or "atlas_potion")
				return false;

			return true;
		}
	}
}
