﻿using DOL.AI.Brain;
using DOL.GS;
using System;

namespace DOL.GS
{
	public class FallenOne : GameNPC
	{
		public FallenOne() : base() { }
		public override bool AddToWorld()
		{
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60160689);
			LoadTemplate(npcTemplate);

			FallenOneBrain sbrain = new FallenOneBrain();
			SetOwnBrain(sbrain);
			LoadedFromScript = false;//load from database
			SaveIntoDatabase();
			base.AddToWorld();
			return true;
		}
	}
}
namespace DOL.AI.Brain
{
	public class FallenOneBrain : StandardMobBrain
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public FallenOneBrain() : base()
		{
			AggroLevel = 100;
			AggroRange = 300;
			ThinkInterval = 1000;
		}
		ushort oldModel;
		GameNPC.eFlags oldFlags;
		bool changed;
		public override void Think()
		{
			MakeFallenOneVisible();
			base.Think();
		}
		private void MakeFallenOneVisible()
        {
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60160689);
			if (Body.CurrentRegion.IsNightTime == false)
			{
				if (changed == false)
				{
					oldFlags = Body.Flags;
					Body.Flags ^= GameNPC.eFlags.CANTTARGET;
					Body.Flags ^= GameNPC.eFlags.DONTSHOWNAME;
					Body.Flags ^= GameNPC.eFlags.PEACE;

					if (oldModel == 0)
						oldModel = Body.Model;

					Body.Model = 1;
					changed = true;
				}
			}
			if (Body.CurrentRegion.IsNightTime)
			{
				if (changed)
				{
					Body.Flags = (GameNPC.eFlags)npcTemplate.Flags;
					Body.Model = Convert.ToUInt16(npcTemplate.Model);
					changed = false;
				}
			}
		}
	}
}


