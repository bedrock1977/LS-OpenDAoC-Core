﻿using DOL.AI.Brain;
using DOL.GS;

namespace DOL.GS
{
	public class ScoutArgyle : GameNPC
	{
		public ScoutArgyle() : base() { }

		public override bool AddToWorld()
		{
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60165671);
			LoadTemplate(npcTemplate);

			ScoutArgyleBrain sbrain = new ScoutArgyleBrain();
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
	public class ScoutArgyleBrain : StandardMobBrain
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public ScoutArgyleBrain() : base()
		{
			AggroLevel = 100;
			AggroRange = 500;
			ThinkInterval = 1500;
		}
		public override void Think()
		{
			if(HasAggro && Body.TargetObject != null)
            {
				foreach (GameNPC npc in Body.GetNPCsInRadius(1000))
				{
					if (npc != null && npc.IsAlive && npc.PackageID == "ScoutArgyleBaf")
						AddAggroListTo(npc.Brain as StandardMobBrain);
				}
			}
			base.Think();
		}
	}
}
