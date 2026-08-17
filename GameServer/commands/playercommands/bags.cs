using System.Collections.Generic;
using System.Linq;
using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute("&bags", ePrivLevel.Player, "Show backpack space and item list", "/bags")]
	public class BagsCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		private const int MaxBackpackSlots = 40;
		private const int MaxListedItems = 35;

		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "bags"))
				return;

			GamePlayer player = client.Player;
			if (player?.Inventory == null)
				return;

			List<DbInventoryItem> items = player.Inventory.GetItemRange(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack);
			int used = items?.Count ?? 0;
			int free = MaxBackpackSlots - used;
			int weight = player.Inventory.InventoryWeight;
			int capacity = player.MaxCarryingCapacity;
			int weightPercent = capacity > 0 ? weight * 100 / capacity : 0;

			var lines = new List<string>
			{
				$"Backpack: {used}/{MaxBackpackSlots} slots used ({free} free)",
				$"Weight: {weight}/{capacity} ({weightPercent}%)",
				string.Empty
			};

			if (items == null || items.Count == 0)
			{
				lines.Add("Your backpack is empty.");
			}
			else
			{
				lines.Add("Items:");
				foreach (DbInventoryItem item in items.OrderBy(i => i.Name).Take(MaxListedItems))
					lines.Add($"  [{item.Quality}%] {item.Name} x{item.Count}");

				if (items.Count > MaxListedItems)
					lines.Add($"  ... and {items.Count - MaxListedItems} more (use /bags again after sorting in-game)");
			}

			player.Out.SendCustomTextWindow($"{player.Name}'s Backpack", lines);
		}
	}
}
