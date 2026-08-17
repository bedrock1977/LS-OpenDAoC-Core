using System.Collections.Generic;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&achievements",
		new[] { "&ach", "&achievement" },
		ePrivLevel.Player,
		"Show your achievement progress",
		"/achievements")]
	public class AchievementsCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "achievements"))
				return;

			GamePlayer player = client.Player;
			if (player == null)
				return;

			IList<string> lines = AchievementMgr.BuildAchievementReport(player);
			player.Out.SendCustomTextWindow($"{player.Name}'s Achievements", lines);
		}
	}
}
