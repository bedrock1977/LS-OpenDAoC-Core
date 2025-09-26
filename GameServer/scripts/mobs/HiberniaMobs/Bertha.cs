﻿using DOL.AI.Brain;
using DOL.GS;

namespace DOL.GS
{
	public class Bertha : GameNPC
	{
		public Bertha() : base() { }

		public override bool AddToWorld()
		{
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(50016);
			LoadTemplate(npcTemplate);

			BerthaBrain sbrain = new BerthaBrain();
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
	public class BerthaBrain : StandardMobBrain
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public BerthaBrain() : base()
		{
			AggroLevel = 0;
			AggroRange = 400;
			ThinkInterval = 1000;
		}
		public override void Think()
		{
			if (HasAggro && Body.TargetObject != null)
			{
				foreach (GameNPC npc in Body.GetNPCsInRadius(1500))
				{
					if (npc != null && npc.IsAlive && npc.PackageID == "BerthaBaf")
						AddAggroListTo(npc.Brain as StandardMobBrain);
				}
			}
			base.Think();
		}
	}
}

