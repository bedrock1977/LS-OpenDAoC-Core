﻿using DOL.AI.Brain;
using DOL.GS;

namespace DOL.GS
{
	public class SirGerenth : GameNPC
	{
		public SirGerenth() : base() { }

		public override bool AddToWorld()
		{
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(12123);
			LoadTemplate(npcTemplate);

			SirGerenthBrain sbrain = new SirGerenthBrain();
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
	public class SirGerenthBrain : StandardMobBrain
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public SirGerenthBrain() : base()
		{
			AggroLevel = 40;
			AggroRange = 400;
			ThinkInterval = 1000;
		}
		public override void Think()
		{
			if (HasAggro && Body.TargetObject != null)
			{
				if (Body.HealthPercent <= 66)
				{
					foreach (GameNPC npc in Body.GetNPCsInRadius(1500))
					{
						if (npc != null && npc.IsAlive && npc.PackageID == "SirGerenthBaf")
							AddAggroListTo(npc.Brain as StandardMobBrain);
					}
				}
			}
			base.Think();
		}
	}
}

