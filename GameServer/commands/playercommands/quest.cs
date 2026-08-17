/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System.Collections.Generic;
using System.Linq;
using DOL.GS.PacketHandler;
using DOL.GS.Quests;

namespace DOL.GS.Commands
{
    [CmdAttribute(
        "&quest",
        new string[] {"&quests"},
        ePrivLevel.Player,
        "Display a list of your ongoing and completed quests", "/quest")]
    public class QuestCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (IsSpammingCommand(client.Player, "quest"))
                return;

            GamePlayer player = client.Player;
            List<AbstractQuest> activeQuests = player.GetActiveQuests();
            List<AbstractQuest> finishedQuests = player.GetFinishedQuests();
            var lines = new List<string>();

            if (activeQuests.Count == 0)
                lines.Add("You have no active quests.");
            else
            {
                lines.Add($"Active quests ({activeQuests.Count}):");
                foreach (AbstractQuest quest in activeQuests)
                {
                    lines.Add(string.Empty);
                    lines.Add($"* {quest.Name} (step {quest.Step})");
                    string description = quest.Description;
                    if (!string.IsNullOrWhiteSpace(description))
                        lines.Add($"  {description.Replace("\n", " ")}");
                }
            }

            lines.Add(string.Empty);

            if (finishedQuests.Count == 0)
                lines.Add("You have not completed any quests yet.");
            else if (finishedQuests.Count <= 15)
            {
                lines.Add($"Completed quests ({finishedQuests.Count}):");
                foreach (AbstractQuest quest in finishedQuests)
                    lines.Add($"  - {quest.Name}");
            }
            else
                lines.Add($"Completed quests: {finishedQuests.Count} total.");

            lines.Add(string.Empty);
            lines.Add("Use /journal for the full in-game quest journal.");

            player.Out.SendCustomTextWindow("Quest Log", lines);
        }
    }
}
