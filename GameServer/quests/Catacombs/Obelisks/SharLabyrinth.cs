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
using System;
using System.Collections.Generic;
using System.Text;
using DOL.Database;

namespace DOL.GS.Quests.Catacombs.Obelisks
{
	/// <summary>
	/// Discovery credit for the Shar Labyrinth.
	/// </summary>
	/// <author>Aredhel</author>
	class SharLabyrinth : ObeliskCredit
	{
		public SharLabyrinth(GamePlayer questingPlayer)
			: base(questingPlayer) { }

		public SharLabyrinth(GamePlayer questingPlayer, DbQuest dbQuest)
			: base(questingPlayer, dbQuest) { }

		/// <summary>
		/// Name of the discovery quest.
		/// </summary>
		public override string Name
		{
			get { return "Shar Labyrinth Obelisk"; }
		}

		/// <summary>
		/// Only Hibernia can get credit.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public override bool CheckQuestQualification(GamePlayer player)
		{
			return (player != null && player.Realm == eRealm.Hibernia);
		}
	}
}